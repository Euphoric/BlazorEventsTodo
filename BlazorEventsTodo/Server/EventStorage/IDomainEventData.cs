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
}
