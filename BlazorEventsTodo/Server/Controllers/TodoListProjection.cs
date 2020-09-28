using BlazorEventsTodo.Todo;
using System;
using System.Collections.Generic;

namespace BlazorEventsTodo.Server.Controllers
{
    public class TodoListProjection : IDomainEventListener<TodoItemDomainEvent>
    {
        Dictionary<Guid, TodoItem> _todoItems = new Dictionary<Guid, TodoItem>();

        public void Handle(TodoItemDomainEvent evnt)
        {
            switch (evnt)
            {
                case TodoItemCreated created:
                    _todoItems[created.Id] = new TodoItem() { Id = created.Id, Title = created.Title };
                    break;
                case TodoItemDeleted deleted:
                    _todoItems.Remove(deleted.Id);
                    break;
                case TodoItemFinished finished:
                    _todoItems[finished.Id].IsFinished = true;
                    break;
                case TodoItemStarted started:
                    _todoItems[started.Id].IsFinished = false;
                    break;
                default:
                    break;
            }
        }

        public IEnumerable<TodoItem> TodoList()
        {
            return _todoItems.Values;
        }

        internal bool TodoExists(Guid id)
        {
            return _todoItems.ContainsKey(id);
        }
    }
}
