﻿using EventStore.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    /// <summary>
    /// Requires EventStore v20.6.0, run with --dev parameter at the moment.
    /// </summary>
    /// <remarks>
    /// TODO : Correct event versions.
    /// ??? : Subscription delay.
    /// </remarks>
    public class PersistentEventStore : IEventStore, IDisposable
    {
        private DomainEventSender _sender;
        private EventStoreClient _client;
        private DomainEventFactory _eventFactory;
        private ILogger _logger;

        public PersistentEventStore(ILoggerFactory loggerFactory, DomainEventSender sender, DomainEventFactory eventFactory)
        {
            _sender = sender;
            _logger = loggerFactory.CreateLogger<PersistentEventStore>();
            _client = CreateClientWithConnection(loggerFactory);
            _eventFactory = eventFactory;
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

        public async Task Store(IDomainEvent<IDomainEventData> @event)
        {
            var dataJson = _eventFactory.SerializeToData(@event);
            var data = Encoding.UTF8.GetBytes(dataJson);
            var metadata = Encoding.UTF8.GetBytes("{}");

            var evt = new EventData(Uuid.FromGuid(@event.Id), @event.EventName, data, metadata);
            var result = await _client.AppendToStreamAsync(@event.AggregateKey, StreamState.Any, new List<EventData>() { evt });
            _logger.LogDebug("Appended event {position}|{type}.", result.LogPosition, evt.Type);
        }

        private Task HandleNewEvent(StreamSubscription subscription, ResolvedEvent evnt, CancellationToken token)
        {
            if (evnt.Event.EventStreamId.StartsWith('$'))
            {
                // skip system events
                return Task.CompletedTask;
            }

            IDomainEvent<IDomainEventData> @event = ParseEvent(evnt);

            _logger.LogDebug("Processed event {position}|{type}.", evnt.OriginalPosition, evnt.Event.EventType);

            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }

        private IDomainEvent<IDomainEventData> ParseEvent(ResolvedEvent resolvedEvent)
        {
            EventRecord evnt = resolvedEvent.Event;
            var dataJson = Encoding.UTF8.GetString(evnt.Data.Span);
            var @event = _eventFactory.DeserializeFromData(evnt.EventId.ToGuid(), evnt.EventNumber.ToUInt64(), evnt.EventType, dataJson);
            return @event;
        }

        public IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TEvent>(string aggregateKey) where TEvent : IDomainEventData
        {
            var events = _client.ReadStreamAsync(Direction.Forwards, aggregateKey, StreamPosition.Start);
            
            return events.Select(ParseEvent).Cast<IDomainEvent<TEvent>>();
        }
    }
}
