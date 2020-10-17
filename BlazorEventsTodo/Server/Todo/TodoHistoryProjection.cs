using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public record TodoHistoryProjection : IProjection
    {
        ImmutableList<TodoHistoryItem> HistoryItems { get; init; } = ImmutableList<TodoHistoryItem>.Empty;
        ImmutableDictionary<TodoItemKey, string> TodoTitles { get; init; } = ImmutableDictionary<TodoItemKey, string>.Empty;

        public IEnumerable<TodoHistoryItem> History()
        {
            return HistoryItems.ToList();
        }

        public IProjection NextState(IDomainEvent<IDomainEventData> evnt)
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
