namespace BlazorEventsTodo.EventStorage
{
    public interface IAggregateKey
    {
        string Value { get; }
    }
}
