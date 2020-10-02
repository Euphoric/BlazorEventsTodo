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

        public TodoController(IEventStore eventStore, TodoListProjection todoListProjection)
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
        public async Task<Guid> Post(CreateTodo create)
        {
            var newId = Guid.NewGuid();
            await _eventStore.Store(DomainEvent<IDomainEventData>.Create(new TodoItemCreated(newId, create.Title)));
            return newId;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _eventStore.Store(DomainEvent<IDomainEventData>.Create(new TodoItemDeleted(id)));
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }
            await _eventStore.Store(DomainEvent<IDomainEventData>.Create(new TodoItemFinished(id)));

            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            if (!_todoListProjection.TodoExists(id))
            {
                return BadRequest();
            }

            await _eventStore.Store(DomainEvent<IDomainEventData>.Create(new TodoItemStarted(id)));

            return Ok();
        }
    }
}
