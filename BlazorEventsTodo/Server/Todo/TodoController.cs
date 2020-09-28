using BlazorEventsTodo.EventStorage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlazorEventsTodo.Todo
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

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _eventStore.Store(new TodoItemDeleted(id));
        }

        [HttpPost("{id}/finish")]
        public IActionResult Finish(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }
            _eventStore.Store(new TodoItemFinished(id));

            return Ok();
        }

        [HttpPost("{id}/start")]
        public IActionResult Start(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }

            _eventStore.Store(new TodoItemStarted(id));

            return Ok();
        }
    }
}
