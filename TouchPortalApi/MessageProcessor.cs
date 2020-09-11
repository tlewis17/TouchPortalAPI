using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Configuration;
using TouchPortalApi.Extensions;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;
using TouchPortalApi.Models.Initialization;
using TouchPortalApi.Models.TouchPortal.Responses;

namespace TouchPortalApi {
  public class MessageProcessor : IMessageProcessor {
    private readonly IOptionsMonitor<TouchPortalApiOptions> _options;
    private readonly ITPClient _tPClient;
    private readonly IProcessQueueingService _processQueueingService;
    private readonly CancellationToken _cancellationToken;

    public event ActionEventHandler OnActionEvent;
    public event ListChangeEventHandler OnListChangeEventHandler;
    public event CloseEventHandler OnCloseEventHandler;
    public event ConnectEventHandler OnConnectEventHandler;

    public bool IsPaired = false;

    public MessageProcessor(IOptionsMonitor<TouchPortalApiOptions> options, ITPClient tPClient, IProcessQueueingService processQueueingService,
      CancellationToken cancellationToken = default) {
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
      _processQueueingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
      _cancellationToken = cancellationToken;
    }

    public async Task Listen() {
      _processQueueingService.SetupChannel(ProcessLine);

      while (!_cancellationToken.IsCancellationRequested) {
        try {
          await _tPClient.ProcessPipes();
        } catch (Exception ex) {
          Console.WriteLine(ex);
        }
      }
    }

    public async Task TryPairAsync() {
      await _tPClient.SendAsync(new PairRequest() { Id = _options.CurrentValue.PluginId });
    }

    private void ProcessLine(ReadOnlySequence<byte> line) {
      if (line.Length == 0) {
        return;
      }

      var result = line.ParseAsUTF8String();

      try {
        var responseModel = JsonConvert.DeserializeObject<TPResponseBase>(result);

        switch (responseModel.Type.ToLower().Trim()) {
          case "info":
            HandlePairEvent(JsonConvert.DeserializeObject<PairResponse>(result));
            break;
          case "action":
            HandleActionEvent(JsonConvert.DeserializeObject<TPAction>(result));
            break;
          case "listChange":
            HandleListChangeEvent(JsonConvert.DeserializeObject<TPListChange>(result));
            break;
          case "closePlugin":
            HandleCloseEvent(JsonConvert.DeserializeObject<TPClosePlugin>(result));
            break;
        }
      } catch (Exception err) {
        Console.WriteLine(err.Message);
      }
    }

    /// <summary>
    /// Handle connect event
    /// </summary>
    /// <param name="response">The TP Pair Response</param>
    private void HandlePairEvent(PairResponse response) {
      // Good pairing message returned
      if (!string.IsNullOrEmpty(response?.PluginVersion)) {
        IsPaired = true;
        OnConnectEventHandler();
      }
    }

    /// <summary>
    /// Handle an action event
    /// </summary>
    /// <param name="action">The action being triggered</param>
    private void HandleActionEvent(TPAction action) {
      OnActionEvent(action.ActionId, action.Data);
    }

    /// <summary>
    /// Handle a list change event
    /// </summary>
    /// <param name="change">The list change event</param>
    private void HandleListChangeEvent(TPListChange change) {
      OnListChangeEventHandler(change.ActionId, change.Value);
    }

    /// <summary>
    /// Handle a plugin close event
    /// </summary>
    /// <param name="closeEvent"></param>
    private void HandleCloseEvent(TPClosePlugin closeEvent) {
      OnCloseEventHandler();
    }
    public void UpdateChoice(ChoiceUpdate choiceUpdate) {
      _tPClient.SendAsync(choiceUpdate);
    }

    public void UpdateState(StateUpdate stateUpdate) {
      _tPClient.SendAsync(stateUpdate);
    }
  }
}
