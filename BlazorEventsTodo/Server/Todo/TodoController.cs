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
        private readonly IEventStore _eventStore;
        private readonly IProjectionState<TodoListProjection> _todoListProjection;
        private readonly IProjectionState<TodoHistoryProjection> _todoHistoryProjection;

        public TodoController(IEventStore eventStore, IProjectionState<TodoListProjection> todoListProjection, IProjectionState<TodoHistoryProjection> todoHistoryProjection)
        {
            _eventStore = eventStore;
            _todoListProjection = todoListProjection;
            _todoHistoryProjection = todoHistoryProjection;
        }

        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            return _todoListProjection.State.TodoList();
        }

        [HttpPost]
        public async Task<EntityId> Post(CreateTodo create)
        {
            var evnt = TodoItemAggregate.New(create.Title);
            await _eventStore.Store(evnt);
            return evnt.Data.Id;
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var itemId = new EntityId(id);
            TodoItemKey aggregateKey = TodoItemKey.Parse(itemId.Value);
            var aggregate = await _eventStore.RetrieveAggregate(aggregateKey);
            var evnt = aggregate.Delete();
            await _eventStore.Store(evnt);
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(string id)
        {
            try
            {
                var itemId = new EntityId(id);
                TodoItemKey aggregateKey = TodoItemKey.Parse(itemId.Value);
                var aggregate = await _eventStore.RetrieveAggregate(aggregateKey);
                var evnt = aggregate.Finish();
                await _eventStore.Store(evnt);
            }
            catch (AggregateChangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(string id)
        {
            try
            {
                var itemId = new EntityId(id);
                TodoItemKey aggregateKey = TodoItemKey.Parse(itemId.Value);
                var aggregate = await _eventStore.RetrieveAggregate(aggregateKey);
                var evnt = aggregate.Start();
                await _eventStore.Store(evnt);
            }
            catch (AggregateChangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet("history")]
        public IEnumerable<TodoHistoryItem> History()
        {
            return _todoHistoryProjection.State.History();
        }
    }
}
