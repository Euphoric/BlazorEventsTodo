using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public abstract class TodoItemDomainEvent : IDomainEventData
    {
        public TodoItemDomainEvent(Guid id)
        {
            Id = id;
        }

        public string GetAggregateKey()
        {
            return $"todo-{Id}";
        }

        public Guid Id { get; }
    }

    [DomainEvent("todo-item-created")]
    public class TodoItemCreated : TodoItemDomainEvent
    {
        public TodoItemCreated(Guid id, string title)
            :base(id)
        {
            Title = title;
        }
        public string Title { get; }
    }

    [DomainEvent("todo-item-deleted")]
    public class TodoItemDeleted : TodoItemDomainEvent
    {
        public TodoItemDeleted(Guid id):base(id)
        {
        }
    }

    [DomainEvent("todo-item-finished")]
    public class TodoItemFinished : TodoItemDomainEvent
    {
        public TodoItemFinished(Guid id):base(id)
        {
        }
    }

    [DomainEvent("todo-item-started")]
    public class TodoItemStarted : TodoItemDomainEvent
    {
        public TodoItemStarted(Guid id):base(id)
        {
        }
    }
}
