using BlazorEventsTodo.EventStorage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Todo
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private IEventStore _eventStore;
        private TodoListProjection _todoListProjection;
        private DomainEventFactory _eventFactory;

        public TodoController(IEventStore eventStore, TodoListProjection todoListProjection, DomainEventFactory eventFactory)
        {
            _eventStore = eventStore;
            _todoListProjection = todoListProjection;
            _eventFactory = eventFactory;
        }

        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            return _todoListProjection.TodoList();
        }

        [HttpPost]
        public async Task<Guid> Post(CreateTodo create)
        {
            var newId = Guid.NewGuid();
            await _eventStore.Store(_eventFactory.Create(new TodoItemCreated(newId, create.Title)));
            return newId;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _eventStore.Store(_eventFactory.Create(new TodoItemDeleted(id)));
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }
            await _eventStore.Store(_eventFactory.Create(new TodoItemFinished(id)));

            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }

            await _eventStore.Store(_eventFactory.Create(new TodoItemStarted(id)));

            return Ok();
        }
    }
}
