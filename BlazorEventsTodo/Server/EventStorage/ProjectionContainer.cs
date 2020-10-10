namespace BlazorEventsTodo.EventStorage
{
    public class ProjectionContainer<TProjection> : IDomainEventListener, IProjectionState<TProjection>
        where TProjection : IProjection<TProjection>, new()
    {
        TProjection _state = new TProjection();
        public TProjection State { get => _state; }

        public void Handle(IDomainEvent<IDomainEventData> evnt)
        {
            _state = State.NextState(evnt);
        }
    }
}
