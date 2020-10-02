using System;

namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventData
    {
        string AggregateKey { get; }
    }

    public interface IDomainEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        TEvent Data { get; }
    }

    public class DomainEvent<TData> : IDomainEvent<TData>
        where TData : IDomainEventData
    {
        public static IDomainEvent<IDomainEventData> Create(IDomainEventData @event)
        {
            var eventType = @event.GetType();
            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { @event });
        }

        public DomainEvent(TData data)
        {
            Data = data;
        }

        public TData Data { get; }
    }
}
