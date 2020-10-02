namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventListener<TEvent> where TEvent : IDomainEventData
    {
        void Handle(IDomainEvent<TEvent> evnt);
    }
}
