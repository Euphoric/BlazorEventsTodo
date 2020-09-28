namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventListener<TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent evnt);
    }
}
