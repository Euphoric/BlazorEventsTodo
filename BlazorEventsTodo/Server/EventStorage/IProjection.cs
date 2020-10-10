namespace BlazorEventsTodo.EventStorage
{
    public interface IProjection<TEvent, TProjection>
        where TEvent : IDomainEventData
        where TProjection : new()
    {
        TProjection NextState(IDomainEvent<TEvent> evnt);
    }
}
