using BlazorEventsTodo.Todo;
using System;
using System.Collections.Generic;

namespace BlazorEventsTodo.Server.Controllers
{
    public class TodoListProjection
    {
        private EventStore _eventStore;

        public TodoListProjection(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public IEnumerable<TodoItem> TodoList()
        {
            Dictionary<Guid, TodoItem> items = CreateTodoTable();

            return items.Values;
        }

        private Dictionary<Guid, TodoItem> CreateTodoTable()
        {
            Dictionary<Guid, TodoItem> items = new Dictionary<Guid, TodoItem>();

            foreach (var evnt in _eventStore.Events)
            {
                switch (evnt)
                {
                    case TodoItemCreated created:
                        items[created.Id] = new TodoItem() { Id = created.Id, Title = created.Title };
                        break;
                    case TodoItemDeleted deleted:
                        items.Remove(deleted.Id);
                        break;
                    case TodoItemFinished finished:
                        items[finished.Id].IsFinished = true;
                        break;
                    case TodoItemStarted started:
                        items[started.Id].IsFinished = false;
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        internal bool TodoExists(Guid id)
        {
            return CreateTodoTable().ContainsKey(id);
        }
    }
}
