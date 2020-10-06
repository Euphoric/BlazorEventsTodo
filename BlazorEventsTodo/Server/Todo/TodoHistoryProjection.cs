using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorEventsTodo.Todo
{
    public class TodoHistoryProjection : IDomainEventListener<TodoItemDomainEvent>
    {
        List<TodoHistoryItem> _items = new List<TodoHistoryItem>();
        Dictionary<Guid, string> _itemTitles = new Dictionary<Guid, string>();

        public void Handle(IDomainEvent<TodoItemDomainEvent> evnt)
        {
            switch (evnt.Data)
            {
                case TodoItemCreated created:
                    _items.Add(new TodoHistoryItem("Item created: " + created.Title, evnt.Created));
                    _itemTitles.Add(created.Id, created.Title);
                    break;
                case TodoItemDeleted deleted:
                    _items.Add(new TodoHistoryItem("Item deleted: " + _itemTitles[deleted.Id], evnt.Created));
                    break;
                case TodoItemFinished finished:
                    _items.Add(new TodoHistoryItem("Item finished: " + _itemTitles[finished.Id], evnt.Created));
                    break;
                case TodoItemStarted started:
                    _items.Add(new TodoHistoryItem("Item restarted: " + _itemTitles[started.Id], evnt.Created));
                    break;
                default:
                    break;
            }
        }

        internal IEnumerable<TodoHistoryItem> History()
        {
            return _items.ToList();
        }
    }
}
