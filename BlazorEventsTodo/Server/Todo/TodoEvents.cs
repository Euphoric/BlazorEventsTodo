using BlazorEventsTodo.EventStorage;

namespace BlazorEventsTodo.Todo
{
    public abstract record TodoItemDomainEvent(TodoItemKey Id) : IDomainEventData
    {
        public string GetAggregateKey()
        {
            return Id.Value;
        }
    }

    [DomainEvent("todo-item-created")]
    public record TodoItemCreated(TodoItemKey Id, string Title) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-deleted")]
    public record TodoItemDeleted(TodoItemKey Id) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-finished")]
    public record TodoItemFinished(TodoItemKey Id) : TodoItemDomainEvent(Id)
    {
    }

    [DomainEvent("todo-item-started")]
    public record TodoItemStarted(TodoItemKey Id) : TodoItemDomainEvent(Id)
    {
    }
}
