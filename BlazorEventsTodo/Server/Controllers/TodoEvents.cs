using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server.Controllers
{
    public abstract class TodoItemDomainEvent : IDomainEvent
    {
    }

    public class TodoItemCreated : TodoItemDomainEvent
    {
        public TodoItemCreated(Guid id, string title)
        {
            Id = id;
            Title = title;
        }

        public Guid Id { get; }
        public string Title { get; }
    }

    public class TodoItemDeleted : TodoItemDomainEvent
    {
        public TodoItemDeleted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class TodoItemFinished : TodoItemDomainEvent
    {
        public TodoItemFinished(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class TodoItemStarted : TodoItemDomainEvent
    {
        public TodoItemStarted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
