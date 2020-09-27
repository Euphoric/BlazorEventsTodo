using BlazorEventsTodo.Todo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private TodoRepository _todoRepository;

        public TodoController(TodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [HttpGet]
        public List<TodoItem> Get()
        {
            return _todoRepository.Items;
        }

        [HttpPost]
        public Guid Post(CreateTodo create)
        {
            var newId = Guid.NewGuid();
            _todoRepository.Items.Add(new TodoItem() { Id = newId, Title = create.Title });
            return newId;
        }

        [HttpPost("{id}/finish")]
        public void Finish(Guid id)
        {
            _todoRepository.Items.Single(x => x.Id == id).IsFinished = true;
        }

        [HttpPost("{id}/start")]
        public void Start(Guid id)
        {
            _todoRepository.Items.Single(x => x.Id == id).IsFinished = false;
        }
    }
}
