using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server.Controllers
{
    public class TodoItemCreated : IDomainEvent
    {
        public TodoItemCreated(Guid id, string title)
        {
            Id = id;
            Title = title;
        }

        public Guid Id { get; }
        public string Title { get; }
    }

    public class TodoItemDeleted : IDomainEvent
    {
        public TodoItemDeleted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class TodoItemFinished : IDomainEvent
    {
        public TodoItemFinished(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class TodoItemStarted : IDomainEvent
    {
        public TodoItemStarted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
