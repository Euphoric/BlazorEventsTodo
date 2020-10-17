using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemAggregate(TodoItemKey Id, string Title, bool IsDeleted, bool IsFinished) : Aggregate
    {
        #region Rehydrate

        public static TodoItemAggregate Initialize(IDomainEvent<TodoItemCreated> createdEvent)
        {
            return new TodoItemAggregate(createdEvent.Data.Id, createdEvent.Data.Title, false, false);
        }

        public TodoItemAggregate Update(IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemDeleted:
                    return this with { IsDeleted = true };
                case TodoItemFinished:
                    return this with { IsFinished = true };
                case TodoItemStarted:
                    return this with { IsFinished = false };
                default:
                    throw new NotSupportedException("Unknown event type.");
            }
        }

        #endregion

        #region Modify

        public static ICreateEvent<TodoItemDomainEvent> New(string Title)
        {
            TodoItemKey newId = TodoItemKey.New();
            return new TodoItemCreated(newId, Title).AsNewAggregate();
        }

        public ICreateEvent<TodoItemDomainEvent> Delete()
        {
            return new TodoItemDeleted(Id).AsVersioned(Version);
        }

        public ICreateEvent<TodoItemDomainEvent> Finish()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot finish deleted item.");
            }
            return new TodoItemFinished(Id).AsVersioned(Version);
        }

        public ICreateEvent<TodoItemDomainEvent> Start()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot start deleted item.");
            }
            return new TodoItemStarted(Id).AsVersioned(Version);
        }

        #endregion
    }
}
