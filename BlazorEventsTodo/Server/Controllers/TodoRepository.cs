using BlazorEventsTodo.Todo;
using System.Collections.Generic;

namespace BlazorEventsTodo.Server.Controllers
{
    public class TodoRepository
    {
        public List<TodoItem> Items { get; } = new List<TodoItem>();
    }
}
