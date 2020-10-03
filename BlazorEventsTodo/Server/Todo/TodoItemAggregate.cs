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

        #endregion

        #region Modify

        public static (TodoItemAggregate, TodoItemDomainEvent) New(string Title)
        {
            Guid newId = Guid.NewGuid();
            return (new TodoItemAggregate(newId, 0, Title, false, false), new TodoItemCreated(newId, Title));
        }

        public (TodoItemAggregate, TodoItemDomainEvent) Delete()
        {
            return (this with { IsDeleted = true }, new TodoItemDeleted(Id));
        }

        public (TodoItemAggregate, TodoItemDomainEvent) Finish()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot finish deleted item.");
            }
            return (this with { IsFinished = true }, new TodoItemFinished(Id));
        }

        public (TodoItemAggregate, TodoItemDomainEvent) Start()
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot start deleted item.");
            }
            return (this with { IsFinished = false }, new TodoItemStarted(Id));
        }

        #endregion
    }
}
