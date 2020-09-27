using System.Collections.Generic;
using System.Collections.Immutable;

namespace BlazorEventsTodo.Server.Controllers
{
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
