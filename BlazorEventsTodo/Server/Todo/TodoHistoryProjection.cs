using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public record TodoHistoryProjection : IProjection<TodoItemDomainEvent, TodoHistoryProjection>
    {
        ImmutableList<TodoHistoryItem> HistoryItems { get; init; } = ImmutableList<TodoHistoryItem>.Empty;
        ImmutableDictionary<Guid, string> TodoTitles { get; init; } = ImmutableDictionary<Guid, string>.Empty;

        public IEnumerable<TodoHistoryItem> History()
        {
            return HistoryItems.ToList();
        }

        public TodoHistoryProjection NextState(IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    return this with
                    {
                        HistoryItems = HistoryItems.Add(new TodoHistoryItem("Item created: " + created.Title, evnt.Created)),
                        TodoTitles = TodoTitles.Add(created.Id, created.Title)
                    };
                case TodoItemDeleted deleted:
                    return this with
                    {
                        HistoryItems = HistoryItems.Add(new TodoHistoryItem("Item deleted: " + TodoTitles[deleted.Id], evnt.Created)),
                    };
                case TodoItemFinished finished:
                    return this with
                    {
                        HistoryItems = HistoryItems.Add(new TodoHistoryItem("Item finished: " + TodoTitles[finished.Id], evnt.Created)),
                    };
                case TodoItemStarted started:
                    return this with
                    {
                        HistoryItems = HistoryItems.Add(new TodoHistoryItem("Item restarted: " + TodoTitles[started.Id], evnt.Created)),
                    };
                default:
                    return this;
            }
        }
    }
}
