namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventData
    {
        string AggregateKey { get; }
    }

    public interface IDomainEvent<out TEvent>
        where TEvent : IDomainEventData
    {
        string AggregateKey { get; }
        string EventName { get; }
        TEvent Data { get; }
    }
}
