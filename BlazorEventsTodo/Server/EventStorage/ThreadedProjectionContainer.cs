using System.Collections.Concurrent;
using System.Threading;

namespace BlazorEventsTodo.EventStorage
{
    public class ThreadedProjectionContainer<TEvent, TProjection> : IDomainEventListener<TEvent>, IProjectionState<TProjection>
        where TEvent : IDomainEventData
        where TProjection : IProjection<TEvent, TProjection>, new()
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<IDomainEvent<TEvent>> _eventQueue = new BlockingCollection<IDomainEvent<TEvent>>(new ConcurrentQueue<IDomainEvent<TEvent>>());

        TProjection _state = new TProjection();

        public TProjection State { get => _state; }

        public ThreadedProjectionContainer()
        {
            _thread = new Thread(RunOnThread);
            _thread.Start();
        }

        private void RunOnThread()
        {
            while (true)
            {
                var evnt = _eventQueue.Take();
                _state = State.NextState(evnt);
            }
        }

        public void Handle(IDomainEvent<TEvent> evnt)
        {
            _eventQueue.Add(evnt);
        }
    }
}
