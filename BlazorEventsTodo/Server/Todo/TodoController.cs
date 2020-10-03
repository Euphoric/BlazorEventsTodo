using BlazorEventsTodo.EventStorage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var (aggregate, evnt) = TodoItemAggregate.New(create.Title);
            await _eventStore.Store(evnt);
            return aggregate.Id;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var aggregate = TodoItemAggregate.Rebuild(await _eventStore.GetAggregateEvents<TodoItemDomainEvent>("todo-" + id).ToListAsync());
            var (_, evnt) = aggregate.Delete();
            await _eventStore.Store(evnt);
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(Guid id)
        {
            try
            {
                var aggregate = TodoItemAggregate.Rebuild(await _eventStore.GetAggregateEvents<TodoItemDomainEvent>("todo-" + id).ToListAsync());
                var (_, evnt) = aggregate.Finish();
                await _eventStore.Store(evnt);
            }
            catch (AggregateChangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            try
            {
                var aggregate = TodoItemAggregate.Rebuild(await _eventStore.GetAggregateEvents<TodoItemDomainEvent>("todo-" + id).ToListAsync());
                var (_, evnt) = aggregate.Start();
                await _eventStore.Store(evnt);
            }
            catch (AggregateChangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
