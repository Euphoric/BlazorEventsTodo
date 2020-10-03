using BlazorEventsTodo.EventStorage;
using System;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemAggregate(Guid Id, string Title, bool IsDeleted, bool IsFinished) : Aggregate
    {
        #region Rehydrate

        public static TodoItemAggregate Initialize(IDomainEvent<TodoItemCreated> createdEvent)
        {
            return new TodoItemAggregate(createdEvent.Data.Id, createdEvent.Data.Title, false, false);
        }

        public TodoItemAggregate Update(IDomainEvent<TodoItemDomainEvent> evnt)
        {
            TodoItemAggregate aggr = this;
            switch (evnt.Data)
            {
                case TodoItemDeleted:
                    aggr = aggr with { IsDeleted = true };
                    break;
                case TodoItemFinished:
                    aggr = aggr with { IsFinished = true };
                    break;
                case TodoItemStarted:
                    aggr = aggr with { IsFinished = false };
                    break;
                default:
                    throw new NotSupportedException("Unknown event type.");
            }

            return aggr;
        }

        #endregion

        #region Modify

        public static TodoItemDomainEvent New(string Title)
        {
            Guid newId = Guid.NewGuid();
            return new TodoItemCreated(newId, Title);
        }

        public TodoItemDomainEvent Delete()
        {
            return new TodoItemDeleted(Id);
        }

        public TodoItemDomainEvent Finish()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot finish deleted item.");
            }
            return new TodoItemFinished(Id);
        }

        public TodoItemDomainEvent Start()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot start deleted item.");
            }
            return new TodoItemStarted(Id);
        }

        #endregion
    }
}
