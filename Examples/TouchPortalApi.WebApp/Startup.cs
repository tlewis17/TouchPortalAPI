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
      var actionService = app.ApplicationServices.GetRequiredService<IActionService>();
      var choiceService = app.ApplicationServices.GetRequiredService<IChoiceService>();

      // Register event callbacks with ID of the button or choice id from your plugin, returned data is a list of action IDs and values from your plugin
      actionService.RegisterActionEvent("TouchPortal.SnoopPlugin.DCS.Action.UFC.Keypad", (obj) => {
        Console.WriteLine($"{DateTime.Now} DCS Action Event Fired.");
        foreach (var o in obj) {
          Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
        }
      });

      // Register Choice Events - Returned data is the new value
      choiceService.RegisterChoiceEvent("choice test", (obj) => {
        Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
      });

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
