namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventListener<TEvent> where TEvent : IDomainEvent
    {
        void Handle(IDomainEventContainer<TEvent> evnt);
    }
}
