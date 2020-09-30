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
    /// TODO : Correct stream names.
    /// TODO : Correct event versions.
    /// ??? : Subscription delay.
    /// </remarks>
    public class PersistentEventStore : IEventStore, IDisposable
    {
        private DomainEventSender _sender;
        private EventStoreClient _client;
        private EventTypeLocator _eventTypeLocator;
        private ILogger _logger;

        public PersistentEventStore(ILoggerFactory loggerFactory, DomainEventSender sender)
        {
            _sender = sender;
            _logger = loggerFactory.CreateLogger<PersistentEventStore>();
            _client = CreateClientWithConnection(loggerFactory);
            _eventTypeLocator = new EventTypeLocator();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task SubscribeClient()
        {
            await _client.SubscribeToAllAsync(HandleNewEvent);
            _logger.LogInformation("Subscribed to events.");
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

        public async Task Store(IDomainEvent @event)
        {
            var eventType = @event.GetType();
            var dataJson = JsonSerializer.Serialize(@event, eventType);
            var data = Encoding.UTF8.GetBytes(dataJson);

            var metadata = Encoding.UTF8.GetBytes("{}");

            var eventTypeString = _eventTypeLocator.GetTypeString(eventType);
            var evt = new EventData(Uuid.NewUuid(), eventTypeString, data, metadata);
            var result = await _client.AppendToStreamAsync("newstream", StreamState.Any, new List<EventData>() { evt });
            _logger.LogDebug("Appended event {position}|{type}.", result.LogPosition, evt.Type);
        }

        private Task HandleNewEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken token)
        {
            if (evnt.Event.EventStreamId.StartsWith('$'))
            {
                // skip system events
                return Task.CompletedTask;
            }

            var eventType = _eventTypeLocator.GetClrType(evnt.Event.EventType);

            if (eventType == null)
            {
                _logger.LogDebug("Received unknown type {position}|{type}.", evnt.OriginalPosition, evnt.Event.EventType);
                return Task.CompletedTask;
            }

            _logger.LogDebug("Processed event {position}|{type}.", evnt.OriginalPosition, evnt.Event.EventType);

            var dataJson = Encoding.UTF8.GetString(evnt.Event.Data.Span);
            var data = JsonSerializer.Deserialize(dataJson, eventType);

            _sender.SendEvent((IDomainEvent)data);

            return Task.CompletedTask;
        }
    }
}
