using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BlazorEventsTodo.Todo
{
    public class TodoListProjection : IDomainEventListener<TodoItemDomainEvent>
    {
        record State
        {
            public ImmutableDictionary<Guid, TodoItem> TodoItems { get; init; } = ImmutableDictionary<Guid, TodoItem>.Empty;
        }

        State _state = new State();

        public void Handle(IDomainEvent<TodoItemDomainEvent> evntCont)
        {
            var previousState = _state;

            State newState = UpdateState(previousState, evntCont);

            _state = newState;
        }

        private static State UpdateState(State previousState, IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    var newItem = new TodoItem() { Id = created.Id, Title = created.Title };
                    return previousState with { TodoItems = previousState.TodoItems.Add(created.Id, newItem) };
                case TodoItemDeleted deleted:
                    return previousState with { TodoItems = previousState.TodoItems.Remove(deleted.Id) };
                case TodoItemFinished finished:
                    return previousState with { TodoItems = previousState.TodoItems.Update(finished.Id, item => item with { IsFinished = true }) };
                case TodoItemStarted started:
                    return previousState with { TodoItems = previousState.TodoItems.Update(started.Id, item => item with { IsFinished = false }) };
                default:
                    return previousState;
            }
        }

        public IEnumerable<TodoItem> TodoList()
        {
            return _state.TodoItems.Values;
        }
    }
}
