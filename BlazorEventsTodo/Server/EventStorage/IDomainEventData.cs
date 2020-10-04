using System;

namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventData
    {
        /// <summary>
        /// Returns domain's aggregate key.
        /// </summary>
        /// <remarks>
        /// Keep as method so it is not serialized.
        /// </remarks>
        string GetAggregateKey();
    }

    public interface IDomainEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        Guid Id { get; }
        ulong Version { get; }
        string AggregateKey { get; }
        string EventName { get; }
        TEvent Data { get; }
    }

    public interface ICreateEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        TEvent Data { get; }
    }

    public static class CreateEventExtension
    {
        private record CreateEvent<TEvent>(TEvent Data) : ICreateEvent<TEvent>
            where TEvent : IDomainEventData;

        public static ICreateEvent<TEvent> Create<TEvent>(this TEvent data)
            where TEvent : IDomainEventData
        {
            return new CreateEvent<TEvent>(data);
        }
    }
}
