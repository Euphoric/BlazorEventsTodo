using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public class TodoHistoryProjection : IDomainEventListener<TodoItemDomainEvent>
    {
        record State
        {
            public ImmutableList<TodoHistoryItem> HistoryItems { get; init; } = ImmutableList<TodoHistoryItem>.Empty;
            public ImmutableDictionary<Guid, string> TodoTitles { get; init; } = ImmutableDictionary<Guid, string>.Empty;
        }

        State _state = new State();

        public void Handle(IDomainEvent<TodoItemDomainEvent> evnt)
        {
            var previousState = _state;

            State newState = UpdateState(previousState, evnt);

            _state = newState;
        }

        private State UpdateState(State previousState, IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item created: " + created.Title, evnt.Created)),
                        TodoTitles = previousState.TodoTitles.Add(created.Id, created.Title)
                    };
                case TodoItemDeleted deleted:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item deleted: " + _state.TodoTitles[deleted.Id], evnt.Created)),
                    };
                case TodoItemFinished finished:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item finished: " + _state.TodoTitles[finished.Id], evnt.Created)),
                    };
                case TodoItemStarted started:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item restarted: " + _state.TodoTitles[started.Id], evnt.Created)),
                    };
                default:
                    return previousState;
            }
        }

        internal IEnumerable<TodoHistoryItem> History()
        {
            return _state.HistoryItems.ToList();
        }
    }
}
