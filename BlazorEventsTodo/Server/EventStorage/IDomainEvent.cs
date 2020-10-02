using System;

namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEvent
    {
        string AggregateKey { get; }
    }

    public interface IDomainEventContainer<out TEvent>
        where TEvent : IDomainEvent
    {
        TEvent Event { get; }
    }

    public class DomainEventContainer<TEvent> : IDomainEventContainer<TEvent>
        where TEvent : IDomainEvent
    {
        public static IDomainEventContainer<IDomainEvent> Create(IDomainEvent @event)
        {
            var eventType = @event.GetType();
            var domainEventContainerType = typeof(DomainEventContainer<>).MakeGenericType(eventType);
            return (IDomainEventContainer<IDomainEvent>)Activator.CreateInstance(domainEventContainerType, args: new object[] { @event });
        }

        public DomainEventContainer(TEvent @event)
        {
            Event = @event;
        }

        public TEvent Event { get; }
    }
}
