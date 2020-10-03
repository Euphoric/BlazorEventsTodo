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

        public static (TodoItemAggregate, IDomainEvent<TodoItemDomainEvent>) New(DomainEventFactory eventFactory, string Title)
        {
            Guid newId = Guid.NewGuid();
            return (new TodoItemAggregate(newId, 0, Title, false, false), eventFactory.Create(new TodoItemCreated(newId, Title)));
        }

        public (TodoItemAggregate, IDomainEvent<TodoItemDomainEvent>) Delete(DomainEventFactory eventFactory)
        {
            return (this with { IsDeleted = true }, eventFactory.Create(new TodoItemDeleted(Id)));
        }

        public (TodoItemAggregate, IDomainEvent<TodoItemDomainEvent>) Finish(DomainEventFactory eventFactory)
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot finish deleted item.");
            }
            return (this with { IsFinished = true }, eventFactory.Create(new TodoItemFinished(Id)));
        }

        public (TodoItemAggregate, IDomainEvent<TodoItemDomainEvent>) Start(DomainEventFactory eventFactory)
        {
            if (IsDeleted)
            {
                throw new AggregateChangeException("Cannot start deleted item.");
            }
            return (this with { IsFinished = false }, eventFactory.Create(new TodoItemStarted(Id)));
        }

        #endregion
    }
}
