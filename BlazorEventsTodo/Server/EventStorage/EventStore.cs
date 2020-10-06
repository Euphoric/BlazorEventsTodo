using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(ICreateEvent<IDomainEventData> eventData);

        IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TEvent>(string aggregateKey)
            where TEvent : IDomainEventData;
    }

    public class EventStore : IEventStore
    {
        private readonly List<IDomainEvent<IDomainEventData>> _events = new List<IDomainEvent<IDomainEventData>>();
        private readonly DomainEventSender _sender;
        private readonly DomainEventFactory _eventFactory;
        private readonly IClock _clock;

        public EventStore(DomainEventSender sender, DomainEventFactory eventFactory, IClock clock)
        {
            _sender = sender;
            _eventFactory = eventFactory;
            _clock = clock;
        }

        public IAsyncEnumerable<IDomainEvent<TEvent>> GetAggregateEvents<TEvent>(string aggregateKey) where TEvent : IDomainEventData
        {
            return _events.OfType<IDomainEvent<TEvent>>().Where(x => x.AggregateKey == aggregateKey).ToAsyncEnumerable();
        }

        public Task Store(ICreateEvent<IDomainEventData> newEvent)
        {
            var eventData = newEvent.Data;
            var eventVersion = _events.Where(x=>x.AggregateKey == eventData.GetAggregateKey()).Select(x=>(ulong?)x.Version).Max(x=>x) ?? 0;
            Instant created = _clock.GetCurrentInstant();
            var @event = _eventFactory.CreateEvent(eventVersion, created, eventData);

            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
