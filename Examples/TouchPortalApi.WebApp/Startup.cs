using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

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
      var stateService = app.ApplicationServices.GetRequiredService<IStateService>();

      // On Action Event
      messageProcessor.OnActionEvent += (actionId, dataList) => {
        Console.WriteLine($"{DateTime.Now} DCS Action Event Fired.");
        foreach (var o in dataList) {
          Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
        }
      };

      // On List Change Event
      messageProcessor.OnListChangeEventHandler += (actionId, value) => {
        Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
      };

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
