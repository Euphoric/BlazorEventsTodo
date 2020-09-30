namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEvent
    {
        string AggregateKey { get; }
    }
}
