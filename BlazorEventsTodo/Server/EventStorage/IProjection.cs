namespace BlazorEventsTodo.EventStorage
{
    public interface IProjection<TEvent, TState>
        where TEvent : IDomainEventData
        where TState : new()
    {
        TState UpdateState(TState previousState, IDomainEvent<TEvent> evnt);
    }
}
