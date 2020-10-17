using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.EventStorage
{
    public interface IEventStore
    {
        public Task Store(ICreateEvent<IDomainEventData> eventData);

        IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents(string aggregateKey);
    }

    public static class EventStoreExtensions
    {
        public static IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents<TAggregate>(this IEventStore eventStore, IAggregateKey<TAggregate> aggregateKey)
            where TAggregate : Aggregate
        {
            return eventStore.GetAggregateEvents(aggregateKey.Value);
        }

        public static async Task<TAggregate> RetrieveAggregate<TAggregate>(this IEventStore eventStore, IAggregateKey<TAggregate> aggregateKey)
            where TAggregate : Aggregate
        {
            return AggregateBuilder<TAggregate>.Rehydrate(await eventStore.GetAggregateEvents(aggregateKey).ToListAsync());
        }
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

        public IAsyncEnumerable<IDomainEvent<IDomainEventData>> GetAggregateEvents(string aggregateKey)
        {
            return _events.Where(x => x.AggregateKey == aggregateKey).ToAsyncEnumerable();
        }

        public Task Store(ICreateEvent<IDomainEventData> newEvent)
        {
            var eventData = newEvent.Data;
            var eventVersion = _events.Where(x => x.AggregateKey == eventData.GetAggregateKey()).Select(x => (ulong?)x.Version).Max(x => x) ?? 0;
            Instant created = _clock.GetCurrentInstant();
            var @event = _eventFactory.CreateEvent(eventVersion, created, eventData);

            _events.Add(@event);
            _sender.SendEvent(@event);

            return Task.CompletedTask;
        }
    }
}
