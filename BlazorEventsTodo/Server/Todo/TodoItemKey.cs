using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemKey(Guid id) : IAggregateKey<TodoItemAggregate, TodoItemDomainEvent>
    {
        public static PrefixedGuidKey<TodoItemKey> Format { get; } = new PrefixedGuidKey<TodoItemKey>("todo");

        public string Value => Format.ToValue(id);

        public static implicit operator EntityId(TodoItemKey key) => new EntityId(key.Value);
        public static explicit operator TodoItemKey(EntityId id) => Format.Parse(id.Value);
    }
}
