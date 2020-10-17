namespace BlazorEventsTodo.Todo
{
    public record EntityId(string Value)
    {
        public override string ToString()
        {
            return Value;
        }
    }
}
