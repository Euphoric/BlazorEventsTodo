using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(IDomainEvent @event);
    }

    public class EventStore : IEventStore
    {
        List<IDomainEvent> _events = new List<IDomainEvent>();
        private DomainEventSender _sender;

        public EventStore(DomainEventSender sender)
        {
            _sender = sender;
        }

        public Task Store(IDomainEvent @event)
        {
            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
