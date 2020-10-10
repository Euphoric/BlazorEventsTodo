using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BlazorEventsTodo.Todo
{
    public record TodoListProjection : IProjection<TodoListProjection>
    {
        ImmutableDictionary<Guid, TodoItem> TodoItems { get; init; } = ImmutableDictionary<Guid, TodoItem>.Empty;

        public IEnumerable<TodoItem> TodoList()
        {
            return TodoItems.Values;
        }

        public TodoListProjection NextState(IDomainEvent<IDomainEventData> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    var newItem = new TodoItem() { Id = created.Id, Title = created.Title };
                    return this with { TodoItems = TodoItems.Add(created.Id, newItem) };
                case TodoItemDeleted deleted:
                    return this with { TodoItems = TodoItems.Remove(deleted.Id) };
                case TodoItemFinished finished:
                    return this with { TodoItems = TodoItems.Update(finished.Id, item => (item with { IsFinished = true })) };
                case TodoItemStarted started:
                    return this with { TodoItems = TodoItems.Update(started.Id, item => (item with { IsFinished = false })) };
                default:
                    return this;
            }
        }
    }
}
