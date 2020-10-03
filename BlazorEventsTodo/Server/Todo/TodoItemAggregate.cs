using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public record TodoItemAggregate(Guid Id, ulong Version, string Title, bool IsDeleted, bool IsFinished)
    {
        #region Rebuild

        public static TodoItemAggregate Rebuild(IEnumerable<IDomainEvent<TodoItemDomainEvent>> events)
        {
            return events.Aggregate((TodoItemAggregate)null, Apply);
        }

        private static TodoItemAggregate Apply(TodoItemAggregate aggr, IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    return new TodoItemAggregate(created.Id, evnt.Version, created.Title, false, false);
                case TodoItemDeleted:
                    return aggr with { Version = evnt.Version, IsDeleted = true };
                case TodoItemFinished:
                    return aggr with { Version = evnt.Version, IsFinished = true };
                case TodoItemStarted:
                    return aggr with { Version = evnt.Version, IsFinished = false };
            }

            throw new NotSupportedException("Unknown event type.");
        }

        public TodoItemAggregate Update(IDomainEvent<TodoItemDomainEvent> domainEvent)
        {
            return Apply(this, domainEvent);
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
