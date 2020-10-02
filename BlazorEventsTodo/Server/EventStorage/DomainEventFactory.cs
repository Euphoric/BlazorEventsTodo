using System;

namespace BlazorEventsTodo.EventStorage
{
    public class DomainEventFactory
    {
        public IDomainEvent<TEventData> Create<TEventData>(TEventData eventData)
            where TEventData : IDomainEventData
        {
            return new DomainEvent<TEventData>(eventData);
        }

        public IDomainEvent<IDomainEventData> CreateFromBase(IDomainEventData @event)
        {
            var eventType = @event.GetType();
            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { @event });
        }

        private class DomainEvent<TData> : IDomainEvent<TData>
            where TData : IDomainEventData
        {
            public DomainEvent(TData data)
            {
                Data = data;
            }

            public TData Data { get; }
        }
    }
}
