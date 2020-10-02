using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(IDomainEventContainer<IDomainEvent> @event);
    }

    public class EventStore : IEventStore
    {
        List<IDomainEventContainer<IDomainEvent>> _events = new List<IDomainEventContainer<IDomainEvent>>();
        private DomainEventSender _sender;

        public EventStore(DomainEventSender sender)
        {
            _sender = sender;
        }

        public Task Store(IDomainEventContainer<IDomainEvent> @event)
        {
            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
