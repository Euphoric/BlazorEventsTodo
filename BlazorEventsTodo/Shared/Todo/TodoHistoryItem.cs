﻿using NodaTime;

namespace BlazorEventsTodo.Todo
{
    public record TodoHistoryItem
    {
        public TodoHistoryItem(string description, Instant changed)
        {
            Description = description;
            Changed = changed;
        }

        public string Description { get; }
        public Instant Changed { get; }
    }
}
