using BlazorEventsTodo.EventStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;

namespace BlazorEventsTodo
{
    public class TestServerFactory : WebApplicationFactory<Server.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IEventStore, EventStorage.EventStore>();
                services.AddSingleton<IClock>(new FakeClock(Instant.FromUtc(2020, 2, 3, 4, 5)));
            });
        }

        public FakeClock Clock
        {
            get
            {
                return Services.GetRequiredService<IClock>() as FakeClock;
            }
        }
    }
}
