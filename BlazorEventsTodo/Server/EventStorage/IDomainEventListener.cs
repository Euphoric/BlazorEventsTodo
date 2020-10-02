namespace BlazorEventsTodo.EventStorage
{
    public interface IDomainEventListener
    {
        void Handle(IDomainEventContainer<IDomainEvent> evnt);
    }
}
