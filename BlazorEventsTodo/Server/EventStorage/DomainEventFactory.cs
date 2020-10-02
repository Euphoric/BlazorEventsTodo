using System;
using System.Text;
using System.Text.Json;

namespace BlazorEventsTodo.EventStorage
{
    public class DomainEventFactory
    {
        private readonly EventTypeLocator _eventTypeLocator;

        public DomainEventFactory()
        {
            _eventTypeLocator = new EventTypeLocator();
        }

        public IDomainEvent<TEventData> Create<TEventData>(TEventData eventData)
            where TEventData : IDomainEventData
        {
            var eventName = _eventTypeLocator.GetTypeString(eventData.GetType());
            return new DomainEvent<TEventData>(eventData, eventName);
        }

        private class DomainEvent<TData> : IDomainEvent<TData>
            where TData : IDomainEventData
        {
            public DomainEvent(TData data, string eventName)
            {
                Data = data;
                EventName = eventName;
            }

            public TData Data { get; }
            public string EventName { get; }

            public string AggregateKey => Data.GetAggregateKey();
        }

        public IDomainEvent<IDomainEventData> Deserialize(string eventName, ReadOnlySpan<byte> dataSpan)
        {
            var eventType = _eventTypeLocator.GetClrType(eventName);

            if (eventType == null)
            {
                throw new Exception("Unknown event name: " + eventName);
            }

            var dataJson = Encoding.UTF8.GetString(dataSpan);
            var data = JsonSerializer.Deserialize(dataJson, eventType);

            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { data, eventName });
        }

        public byte[] Serialize(IDomainEvent<IDomainEventData> @event)
        {
            var eventData = @event.Data;
            var eventType = eventData.GetType();
            var dataJson = JsonSerializer.Serialize(eventData, eventType);
            return Encoding.UTF8.GetBytes(dataJson);
        }
    }
}
