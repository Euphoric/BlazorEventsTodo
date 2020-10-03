using System;

namespace BlazorEventsTodo.Todo
{
    public class AggregateChangeException : Exception
    {
        public AggregateChangeException(string message)
            :base(message)
        {

        }
    }
}
