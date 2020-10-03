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
            var (aggregate, evnt) = TodoItemAggregate.New(_eventFactory, create.Title);
            await _eventStore.Store(evnt);
            return aggregate.Id;
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            var aggregate = TodoItemAggregate.Rebuild(await _eventStore.GetAggregateEvents<TodoItemDomainEvent>("todo-" + id).ToListAsync());
            var (_, evnt) = aggregate.Delete(_eventFactory);
            await _eventStore.Store(evnt);
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(Guid id)
        {
            try
            {
                var aggregate = TodoItemAggregate.Rebuild(await _eventStore.GetAggregateEvents<TodoItemDomainEvent>("todo-" + id).ToListAsync());
                var (_, evnt) = aggregate.Finish(_eventFactory);
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
                var (_, evnt) = aggregate.Start(_eventFactory);
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
