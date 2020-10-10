namespace BlazorEventsTodo.EventStorage
{
    public class ProjectionContainer<TEvent, TState> : IDomainEventListener<TEvent>, IProjectionState<TState>
        where TEvent : IDomainEventData
        where TState : new()
    {
        private readonly IProjection<TEvent, TState> _projection;
        TState _state = new TState();
        public TState State { get => _state; }

        public ProjectionContainer(IProjection<TEvent, TState> projection)
        {
            _projection = projection;
        }

        public void Handle(IDomainEvent<TEvent> evnt)
        {
            _state = _projection.UpdateState(_state, evnt);
        }
    }
}
