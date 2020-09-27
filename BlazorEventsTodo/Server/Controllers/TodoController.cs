using BlazorEventsTodo.Todo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlazorEventsTodo.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private EventStore _eventStore;
        private TodoListProjection _todoListProjection;

        public TodoController(EventStore eventStore, TodoListProjection todoListProjection)
        {
            _eventStore = eventStore;
            _todoListProjection = todoListProjection;
        }

        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            return _todoListProjection.TodoList();
        }

        [HttpPost]
        public Guid Post(CreateTodo create)
        {
            var newId = Guid.NewGuid();
            _eventStore.Store(new TodoItemCreated(newId, create.Title));
            return newId;
        }

        [HttpPost("{id}/finish")]
        public void Finish(Guid id)
        {
            _eventStore.Store(new TodoItemFinished(id));
        }

        [HttpPost("{id}/start")]
        public void Start(Guid id)
        {
            _eventStore.Store(new TodoItemStarted(id));
        }
    }
}
