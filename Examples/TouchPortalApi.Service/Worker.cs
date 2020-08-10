using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.Service {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IActionService _actionService;
    private readonly IChoiceService _choiceService;
    private readonly IStateService _stateService;

    public Worker(ILogger<Worker> logger, IMessageProcessor messageProcessor,
      IActionService actionService, IChoiceService choiceService, IStateService stateService) {
      _logger = logger;
      _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
      _actionService = actionService ?? throw new ArgumentNullException(nameof(actionService));
      _choiceService = choiceService ?? throw new ArgumentNullException(nameof(choiceService));
      _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      // Register Events
      // Register event callbacks with ID of the button or choice id from your plugin, returned data is a list of action IDs and values from your plugin
      _actionService.RegisterActionEvent("TouchPortal.SnoopPlugin.DCS.Action.UFC.Keypad", (obj) => {
        Console.WriteLine($"{DateTime.Now} DCS Action Event Fired.");
        foreach (var o in obj) {
          Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
        }
      });

      // Register Choice Events - Returned data is the new value
      _choiceService.RegisterChoiceEvent("choice test", (obj) => {
        Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
      });

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
