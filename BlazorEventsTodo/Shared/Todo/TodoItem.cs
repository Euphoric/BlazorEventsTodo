using System;

namespace BlazorEventsTodo.Todo
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsFinished { get; set; }
    }
}
