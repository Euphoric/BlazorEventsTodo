namespace BlazorEventsTodo.EventStorage
{
    public interface IAggregateKey<TAggregate>
        where TAggregate: Aggregate
    {
        string Value { get; }
    }
}
