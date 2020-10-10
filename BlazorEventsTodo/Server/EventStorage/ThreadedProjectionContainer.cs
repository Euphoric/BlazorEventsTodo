using System.Collections.Concurrent;
using System.Threading;

namespace BlazorEventsTodo.EventStorage
{
    public class ThreadedProjectionContainer<TProjection> : IDomainEventListener, IProjectionState<TProjection>
        where TProjection : IProjection<TProjection>, new()
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<IDomainEvent<IDomainEventData>> _eventQueue = new BlockingCollection<IDomainEvent<IDomainEventData>>(new ConcurrentQueue<IDomainEvent<IDomainEventData>>());

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

        public void Handle(IDomainEvent<IDomainEventData> evnt)
        {
            _eventQueue.Add(evnt);
        }
    }
}
