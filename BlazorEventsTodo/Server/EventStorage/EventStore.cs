using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(IDomainEvent<IDomainEventData> @event);

        IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TEvent>(string aggregateKey)
            where TEvent : IDomainEventData;
    }

    public class EventStore : IEventStore
    {
        List<IDomainEvent<IDomainEventData>> _events = new List<IDomainEvent<IDomainEventData>>();
        private DomainEventSender _sender;

        public EventStore(DomainEventSender sender)
        {
            _sender = sender;
        }

        public IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TEvent>(string aggregateKey) where TEvent : IDomainEventData
        {
            return _events.OfType<IDomainEvent<TEvent>>().Where(x=>x.AggregateKey == aggregateKey).ToAsyncEnumerable();
        }

        public Task Store(IDomainEvent<IDomainEventData> @event)
        {
            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
