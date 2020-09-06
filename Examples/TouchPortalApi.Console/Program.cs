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

      // Run Listen and pairing
      Task.WhenAll(new Task[] {
        messageProcessor.Listen(),
        messageProcessor.TryPairAsync()
      });

      Console.ReadLine();
    }
  }
}
