using System;
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
            return new DomainEvent<TEventData>(Guid.NewGuid(), 0, eventData, eventName);
        }

        private class DomainEvent<TData> : IDomainEvent<TData>
            where TData : IDomainEventData
        {
            public DomainEvent(Guid id, ulong version, TData data, string eventName)
            {
                Id = id;
                Version = version;
                Data = data;
                EventName = eventName;
            }

            public Guid Id { get; }
            public ulong Version { get; }
            public TData Data { get; }
            public string EventName { get; }

            public string AggregateKey => Data.GetAggregateKey();
        }

        public IDomainEvent<IDomainEventData> DeserializeFromData(Guid id, ulong version, string eventName, string dataJson)
        {
            var eventType = _eventTypeLocator.GetClrType(eventName);

            if (eventType == null)
            {
                throw new Exception("Unknown event name: " + eventName);
            }

            var data = JsonSerializer.Deserialize(dataJson, eventType);

            var domainEventContainerType = typeof(DomainEvent<>).MakeGenericType(eventType);
            return (IDomainEvent<IDomainEventData>)Activator.CreateInstance(domainEventContainerType, args: new object[] { id, version, data, eventName });
        }

        public string SerializeToData(IDomainEvent<IDomainEventData> @event)
        {
            var eventData = @event.Data;
            var eventType = eventData.GetType();
            return JsonSerializer.Serialize(eventData, eventType);
        }
    }
}
