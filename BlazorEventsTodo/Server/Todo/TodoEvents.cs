using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Todo
{
    public abstract class TodoItemDomainEvent : IDomainEvent
    {
    }

    [DomainEvent("todo-item-created")]
    public class TodoItemCreated : TodoItemDomainEvent
    {
        public TodoItemCreated(Guid id, string title)
        {
            Id = id;
            Title = title;
        }

        public Guid Id { get; }
        public string Title { get; }
    }

    [DomainEvent("todo-item-deleted")]
    public class TodoItemDeleted : TodoItemDomainEvent
    {
        public TodoItemDeleted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    [DomainEvent("todo-item-finished")]
    public class TodoItemFinished : TodoItemDomainEvent
    {
        public TodoItemFinished(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    [DomainEvent("todo-item-started")]
    public class TodoItemStarted : TodoItemDomainEvent
    {
        public TodoItemStarted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
