﻿namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventListener
    {
        void Handle(IDomainEvent evnt);
    }
}
