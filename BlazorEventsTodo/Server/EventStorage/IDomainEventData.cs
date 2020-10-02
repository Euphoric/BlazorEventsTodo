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
        string AggregateKey { get; }
        string EventName { get; }
        TEvent Data { get; }
    }
}
