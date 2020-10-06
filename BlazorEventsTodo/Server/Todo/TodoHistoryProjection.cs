using BlazorEventsTodo.EventStorage;
using System;
using System.Collections.Generic;
using System.Linq;
//using KellermanSoftware.CompareNetObjects;

namespace BlazorEventsTodo.Todo
{
    public class TodoHistoryProjection : IDomainEventListener<TodoItemDomainEvent>
    {
        List<TodoHistoryItem> _items = new List<TodoHistoryItem>();
        Dictionary<Guid, string> _itemTitles = new Dictionary<Guid, string>();

        public void Handle(IDomainEvent<TodoItemDomainEvent> evntCont)
        {
            switch (evntCont.Data)
            {
                case TodoItemCreated created:
                    _items.Add(new TodoHistoryItem("Item created: " + created.Title));
                    _itemTitles.Add(created.Id, created.Title);
                    break;
                case TodoItemDeleted deleted:
                    _items.Add(new TodoHistoryItem("Item deleted: " + _itemTitles[deleted.Id]));
                    break;
                case TodoItemFinished finished:
                    _items.Add(new TodoHistoryItem("Item finished: " + _itemTitles[finished.Id]));
                    break;
                case TodoItemStarted started:
                    _items.Add(new TodoHistoryItem("Item restarted: " + _itemTitles[started.Id]));
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
