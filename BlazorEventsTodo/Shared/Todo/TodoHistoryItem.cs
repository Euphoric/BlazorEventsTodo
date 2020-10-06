namespace BlazorEventsTodo.Todo
{
    public class TodoHistoryItem
    {
        public TodoHistoryItem(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
