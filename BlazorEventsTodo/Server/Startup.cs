using BlazorEventsTodo.EventStorage;
using BlazorEventsTodo.Todo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System;
using System.Threading.Tasks;

namespace BlazorEventsTodo.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options=>{
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                });
            services.AddRazorPages();

            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddSingleton<IEventStore, PersistentEventStore>();
            services.AddSingleton<DomainEventSender>();
            services.AddSingleton<DomainEventFactory>();

            services.AddSingleton<IProjectionContainerFactory, ThreadedProjectionContainerFactory>();

            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<TodoListProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<TodoListProjection>());

            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionState<TodoHistoryProjection>());
            services.AddSingleton(sp => sp.GetRequiredService<IProjectionContainerFactory>().CreateProjectionListener<TodoHistoryProjection>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        internal static async Task InitializeServices(IServiceProvider services)
        {
            var eventStore = services.GetService<IEventStore>();

            if (eventStore is PersistentEventStore persistentEventStore)
            {
                await persistentEventStore.SubscribeClient();
            }
        }
    }
}
