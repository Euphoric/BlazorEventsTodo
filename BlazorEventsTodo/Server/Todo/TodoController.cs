﻿using BlazorEventsTodo.EventStorage;
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
            var storedEvent = await _eventStore.Store(evnt);
            var aggregate = AggregateBuilder<TodoItemAggregate>.Rehydrate(new [] { storedEvent });
            return aggregate.Id;
        }

        [HttpDelete("{id}")]
        public async Task Delete(EntityId id)
        {
            var aggregate = await _eventStore.RetrieveAggregate((TodoItemKey)id);
            var evnt = aggregate.Delete();
            var storedEvent = await _eventStore.Store(evnt);
            aggregate = aggregate.Update(new [] {storedEvent });
        }

        [HttpPost("{id}/finish")]
        public async Task<IActionResult> Finish(EntityId id)
        {
            try
            {
                var aggregate = await _eventStore.RetrieveAggregate((TodoItemKey)id);
                var evnt = aggregate.Finish();
                var storedEvent = await _eventStore.Store(evnt);
                aggregate = aggregate.Update(new [] {storedEvent });
            }
            catch (AggregateChangeException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(EntityId id)
        {
            try
            {
                var aggregate = await _eventStore.RetrieveAggregate((TodoItemKey)id);
                var evnt = aggregate.Start();
                var storedEvent = await _eventStore.Store(evnt);
                aggregate = aggregate.Update(new [] {storedEvent });
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
