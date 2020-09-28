namespace BlazorEventsTodo.Server.Controllers
{
    public interface IDomainEventListener
    {
        void Handle(IDomainEvent evnt);
    }
}
