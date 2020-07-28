using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.ConsoleApp {
  class Program {
    static void Main(string[] args) {
      // Setup DI with configured options
      var serviceProvider = new ServiceCollection()
        .ConfigureTouchPointApi((opts) => {
          opts.PluginId = "TouchPortal.SnoopPlugin";
          opts.ServerIp = "127.0.0.1";
          opts.ServerPort = 12136;
        }).BuildServiceProvider();

      // Our services, can be retrieved through DI in constructors
      var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();
      var stateService = serviceProvider.GetRequiredService<IStateService>();
      var actionService = serviceProvider.GetRequiredService<IActionService>();
      var choiceService = serviceProvider.GetRequiredService<IChoiceService>();

      // Register event callbacks with ID of the button or choice id from your plugin, returned data is a list of action IDs and values from your plugin
      actionService.RegisterActionEvent("TouchPortal.SnoopPlugin.DCS.Action.UFC.Keypad", async (obj) => {
        Console.WriteLine($"{DateTime.Now} DCS Action Event Fired.");
        foreach (var o in obj) {
          Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
        }
      });

      // Register Choice Events - Returned data is the new value
      choiceService.RegisterChoiceEvent("choice test", (obj) => {
        Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
      });

      // Run Listen and pairing
      Task.WhenAll(new Task[] {
        messageProcessor.Listen(),
        messageProcessor.TryPairAsync()
      });

      Console.ReadLine();
    }
  }
}
