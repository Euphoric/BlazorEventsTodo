using Microsoft.AspNetCore.Connections.Features;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public void Store(IDomainEvent @event);
    }

    /// <summary>
    /// Stores events.
    /// </summary>
    /// <remarks>
    /// Design considerations:
    /// Single event can be written into multiple streams. Read ordering is guaranteed  
    /// Only guarantees ordering of events with same key. This is based on it's version.
    /// No guaranteed ordering of all events.
    /// Transactions only with events with same key.
    /// </remarks>
    public class EventStore : IEventStore
    {
        List<IDomainEvent> _events = new List<IDomainEvent>();
        private DomainEventSender _sender;

        public EventStore(DomainEventSender sender)
        {
            _sender = sender;
        }

        public void Store(IDomainEvent @event)
        {
            _events.Add(@event);
            _sender.SendEvent(@event);
        }
    }
}
