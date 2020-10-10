namespace BlazorEventsTodo.EventStorage
{
    public class ProjectionContainer<TEvent, TProjection> : IDomainEventListener<TEvent>, IProjectionState<TProjection>
        where TEvent : IDomainEventData
        where TProjection : IProjection<TEvent, TProjection>, new()
    {
        TProjection _state = new TProjection();
        public TProjection State { get => _state; }

        public void Handle(IDomainEvent<TEvent> evnt)
        {
            _state = State.NextState(evnt);
        }
    }
}
