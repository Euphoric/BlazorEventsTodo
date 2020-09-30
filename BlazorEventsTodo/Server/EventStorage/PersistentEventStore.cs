using EventStore.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    /// <summary>
    /// Requires EventStore v20.6.0, run with --dev parameter at the moment.
    /// </summary>
    /// <remarks>
    /// TODO : Dispose client.
    /// TODO : Correct stream names.
    /// TODO : Correct event types.
    /// TODO : Correct event versions.
    /// </remarks>
    public class PersistentEventStore : IEventStore
    {
        private DomainEventSender _sender;
        private EventStoreClient _client;

        public PersistentEventStore(ILoggerFactory loggerFactory, DomainEventSender sender)
        {
            _sender = sender;
            _client = CreateClientWithConnection(loggerFactory);
        }

        public async Task SubscribeClient()
        {
            await _client.SubscribeToAllAsync(HandleNewEvent);
        }

        private static EventStoreClient CreateClientWithConnection(ILoggerFactory loggerFactory)
        {
            /** https://discuss.eventstore.com/t/basic-eventstoredb-v20-example/2553
             *  settings workaround for certificate issues when trying out the client
             *  I didn't have this problem but if you are running event store in --dev mode this might be an issue'
             */
            var settingsWorkAround = new EventStoreClientSettings
            {
                CreateHttpMessageHandler = () =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (message, certificate2, x509Chain, sslPolicyErrors) => true
                    },
                ConnectivitySettings = {
                    Address = new Uri("https://localhost:2113")
                },
                LoggerFactory = loggerFactory,
                DefaultCredentials = new UserCredentials("admin", "changeit")
            };

            return new EventStoreClient(settingsWorkAround);
        }

        private class Metadata
        {
            public string TypeFullName {get;set;}
        }

        public async Task Store(IDomainEvent @event)
        {
            var eventType = @event.GetType();
            var dataJson = JsonSerializer.Serialize(@event, eventType);
            var data = Encoding.UTF8.GetBytes(dataJson);

            var metadataJson = JsonSerializer.Serialize(new Metadata{ TypeFullName = eventType.FullName });
            var metadata = Encoding.UTF8.GetBytes(metadataJson);

            var evt = new EventData(Uuid.NewUuid(), "event-type", data, metadata);
            await _client.AppendToStreamAsync("newstream", StreamState.Any, new List<EventData>() { evt });
        }

        private Task HandleNewEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken token)
        {
            if (evnt.Event.EventStreamId.StartsWith('$'))
            {
                // skip system events
                return Task.CompletedTask;
            }

            var metadataJson = Encoding.UTF8.GetString(evnt.Event.Metadata.Span);
            var metadata = JsonSerializer.Deserialize<Metadata>(metadataJson);

            if (string.IsNullOrEmpty(metadata.TypeFullName))
            {
                return Task.CompletedTask;
            }

            var eventType = Type.GetType(metadata.TypeFullName);
            var dataJson = Encoding.UTF8.GetString(evnt.Event.Data.Span);
            var data = JsonSerializer.Deserialize(dataJson, eventType);

            _sender.SendEvent((IDomainEvent)data);

            return Task.CompletedTask;
        }
    }
}
