using System.Collections.Generic;
using System.Collections.Immutable;

namespace BlazorEventsTodo.Server.Controllers
{
    /// <summary>
    /// Stores events.
    /// </summary>
    /// <remarks>
    /// Design considerations:
    /// Only guarantees ordering of events with same key. This is based on it's version.
    /// </remarks>
    public class EventStore
    {
        List<IDomainEvent> _events = new List<IDomainEvent>();

        public void Store(IDomainEvent @event)
        {
            _events.Add(@event);
        }

        public IEnumerable<IDomainEvent> Events => _events.ToImmutableList();
    }
}
