using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.ConsoleApp {
  public static class Program {
    public static void Main(string[] args) {
      // Setup DI with configured options
      var serviceProvider = new ServiceCollection()
        .ConfigureTouchPointApi((opts) => {
          opts.PluginId = "TouchPortal.SnoopPlugin";
          opts.ServerIp = "127.0.0.1";
          opts.ServerPort = 12136;
        }).BuildServiceProvider();

      // Our services, can be retrieved through DI in constructors
      var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();

      // On Plugin Connect Event
      messageProcessor.OnConnectEventHandler += () => {
        messageProcessor.CreateState(new StateCreate() { Id = "CreatedStateId", Desc = "State Description", DefaultValue = "default value" });
      };

      // On Action Event
      messageProcessor.OnActionEvent += (actionId, dataList) => {
      };

      // On List Change Event
      messageProcessor.OnListChangeEventHandler += (actionId, value) => {
      };

      // On Plugin Disconnect
      messageProcessor.OnCloseEventHandler += () => {
        messageProcessor.RemoveState(new StateRemove { Id = "CreatedStateId" });
      };

      // Send State Update
      messageProcessor.UpdateState(new StateUpdate { Id = "SomeStateId", Value = "New Value" });

      // Run Listen and pairing
      Task.WhenAll(new Task[] {
        messageProcessor.Listen(),
        messageProcessor.TryPairAsync()
      });

      Console.ReadLine();
    }
  }
}
