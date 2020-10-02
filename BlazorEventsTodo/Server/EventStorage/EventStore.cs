using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(IDomainEvent<IDomainEventData> @event);
    }

    public class EventStore : IEventStore
    {
        List<IDomainEvent<IDomainEventData>> _events = new List<IDomainEvent<IDomainEventData>>();
        private DomainEventSender _sender;

        public EventStore(DomainEventSender sender)
        {
            _sender = sender;
        }

        public Task Store(IDomainEvent<IDomainEventData> @event)
        {
            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
