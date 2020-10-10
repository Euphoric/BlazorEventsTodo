using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public class TodoHistoryProjection : IProjection<TodoItemDomainEvent, TodoHistoryProjection.State>
    {
        public record State
        {
            public ImmutableList<TodoHistoryItem> HistoryItems { get; init; } = ImmutableList<TodoHistoryItem>.Empty;
            public ImmutableDictionary<Guid, string> TodoTitles { get; init; } = ImmutableDictionary<Guid, string>.Empty;

            internal IEnumerable<TodoHistoryItem> History()
            {
                return HistoryItems.ToList();
            }
        }

        public State UpdateState(State previousState, IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item created: " + created.Title, evnt.Created)),
                        TodoTitles = previousState.TodoTitles.Add(created.Id, created.Title)
                    };
                case TodoItemDeleted deleted:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item deleted: " + previousState.TodoTitles[deleted.Id], evnt.Created)),
                    };
                case TodoItemFinished finished:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item finished: " + previousState.TodoTitles[finished.Id], evnt.Created)),
                    };
                case TodoItemStarted started:
                    return previousState with
                    {
                        HistoryItems = previousState.HistoryItems.Add(new TodoHistoryItem("Item restarted: " + previousState.TodoTitles[started.Id], evnt.Created)),
                    };
                default:
                    return previousState;
            }
        }
    }
}
