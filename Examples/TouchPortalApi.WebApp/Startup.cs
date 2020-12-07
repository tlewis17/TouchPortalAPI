using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.WebApp {
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {
      services.ConfigureTouchPointApi((options) => {
        options.ServerIp = "127.0.0.1";
        options.ServerPort = 12136;
        options.PluginId = "TouchPortal.SnoopPlugin";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints => {
        endpoints.MapGet("/", async context => {
          await context.Response.WriteAsync("Hello World!");
        });
      });

      // Our services, can be retrieved through DI in constructors
      var messageProcessor = app.ApplicationServices.GetRequiredService<IMessageProcessor>();

      // On Plugin Connect Event
      messageProcessor.OnConnectEventHandler += () => {
        messageProcessor.CreateState(new CreateState() { Id = "CreatedStateId", Desc = "State Description", DefaultValue = "default value" });
      };

      // On Action Event - Foreach event in dataList
      messageProcessor.OnActionEvent += (actionId, dataList) => {
      };

      // On List Change Event
      messageProcessor.OnListChangeEventHandler += (actionId, value) => {
      };

      // On Plugin Disconnect
      messageProcessor.OnCloseEventHandler += () => {
        messageProcessor.RemoveState(new RemoveState() { Id = "CreatedStateId" });
      };

      // Send State Update
      messageProcessor.UpdateState(new StateUpdate() { Id = "SomeStateId", Value = "New Value" });

      // Listen/Pair on app startup
      lifetime.ApplicationStarted.Register(() => {
        Task.WhenAll(new Task[] {
          messageProcessor.Listen(),
          messageProcessor.TryPairAsync()
        });
      });
    }
  }
}
