using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.Service {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IMessageProcessor _messageProcessor;

    public Worker(ILogger<Worker> logger, IMessageProcessor messageProcessor) {
      _logger = logger;
      _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      // On Plugin Connect Event
      _messageProcessor.OnConnectEventHandler += () => {
        _logger.LogInformation($"{DateTime.Now} Plugin Connected to TouchPortal");
        _messageProcessor.CreateState(new CreateState() { Id = "CreatedStateId", Desc = "State Description", DefaultValue = "default value" });
      };

      // On Action Event
      _messageProcessor.OnActionEvent += (actionId, dataList) => {
        _logger.LogInformation($"{DateTime.Now} Action Event Fired.");
        foreach (var o in dataList) {
          _logger.LogInformation($"Id: {o.Id} Value: {o.Value}");
        }
      };

      // On List Change Event
      _messageProcessor.OnListChangeEventHandler += (actionId, value) => {
        _logger.LogInformation($"{DateTime.Now} Choice Event Fired.");
      };

      // On Plugin Disconnect
      _messageProcessor.OnCloseEventHandler += () => {
        _logger.LogInformation($"{DateTime.Now} Plugin Quit Command");
        _messageProcessor.RemoveState(new RemoveState() { Id = "CreatedStateId" });
      };

      // Send State Update
      _messageProcessor.UpdateState(new StateUpdate() { Id = "SomeStateId", Value = "New Value" });

      // Run Listen and pairing
      _ = Task.WhenAll(new Task[] {
        _messageProcessor.Listen(),
        _messageProcessor.TryPairAsync()
      });

      // Do whatever you want in here
      while (!stoppingToken.IsCancellationRequested) {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        await Task.Delay(1000, stoppingToken);
      }
    }
  }
}
