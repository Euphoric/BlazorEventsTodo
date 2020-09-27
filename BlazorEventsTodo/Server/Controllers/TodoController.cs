using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server.Controllers
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class TodoRepository
    {
        public List<TodoItem> Items { get; } = new List<TodoItem>();
    }

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

        public class CreateTodo
        {
            public string Title { get; set; }
        }

        [HttpPost]
        public Guid Post(CreateTodo create)
        {
            var newId = Guid.NewGuid();
            _todoRepository.Items.Add(new TodoItem() { Id = newId, Title = create.Title });
            return newId;
        }
    }
}
