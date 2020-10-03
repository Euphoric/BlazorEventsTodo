using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public abstract record TodoItemDomainEvent(Guid Id) : IDomainEventData
    {
        public string GetAggregateKey()
        {
            return $"todo-{Id}";
        }
    }

    [DomainEvent("todo-item-created")]
    public record TodoItemCreated(Guid Id, string Title) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-deleted")]
    public record TodoItemDeleted(Guid Id) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-finished")]
    public record TodoItemFinished(Guid Id) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-started")]
    public record TodoItemStarted(Guid Id) : TodoItemDomainEvent(Id)
    {
    }
}
