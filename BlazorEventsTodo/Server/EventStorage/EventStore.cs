using System.Collections.Generic;
using System.Collections.Immutable;

namespace BlazorEventsTodo.EventStorage
{
    /// <summary>
    /// Stores events.
    /// </summary>
    /// <remarks>
    /// Design considerations:
    /// Only guarantees ordering of events with same key. This is based on it's version.
    /// No guaranteed ordering of all events.
    /// Transactions only with events with same key.
    /// </remarks>
    public class EventStore
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

        public IEnumerable<IDomainEvent> Events => _events.ToImmutableList();
    }
}
