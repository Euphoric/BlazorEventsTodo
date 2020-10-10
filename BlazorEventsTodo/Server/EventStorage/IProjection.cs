namespace BlazorEventsTodo.EventStorage
{
    public interface IProjection<TProjection>
        where TProjection : new()
    {
        TProjection NextState(IDomainEvent<IDomainEventData> evnt);
    }
}
