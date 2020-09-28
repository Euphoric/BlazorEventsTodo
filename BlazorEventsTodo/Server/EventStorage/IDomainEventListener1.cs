namespace BlazorEventsTodo.Server.Controllers
{
    public interface IDomainEventListener<TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent evnt);
    }
}
