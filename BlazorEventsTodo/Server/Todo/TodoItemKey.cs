using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemKey(Guid id) : IAggregateKey<TodoItemAggregate, TodoItemDomainEvent>
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
}
