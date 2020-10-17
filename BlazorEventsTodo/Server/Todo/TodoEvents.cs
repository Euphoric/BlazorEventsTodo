using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemKey(Guid id)
    {
        const string Prefix = "todo-";

        public string Value => Prefix + id;

        public static TodoItemKey Parse(string value)
        {
            if (!value.StartsWith(Prefix))
            {
                throw new Exception("Invalid aggregate key format");
            }
            var guidId = value.Substring(Prefix.Length);

            return new TodoItemKey(Guid.Parse(guidId));
        }

        public static implicit operator EntityId(TodoItemKey key) => new EntityId(key.Value);
        public static explicit operator TodoItemKey(EntityId id) => Parse(id.Value);

        internal static TodoItemKey New()
        {
            return new TodoItemKey(Guid.NewGuid());
        }
    }

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
