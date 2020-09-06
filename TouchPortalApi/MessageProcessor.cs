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

    public MessageProcessor(IOptionsMonitor<TouchPortalApiOptions> options, ITPClient tPClient, IProcessQueueingService processQueueingService,
      CancellationToken cancellationToken = default) {
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
      _processQueueingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
      _cancellationToken = cancellationToken;
    }

    public async Task Listen() {
      //Task.Run(() => {
      //  while (!_cancellationToken.IsCancellationRequested) {
      //    ProcessLine(_processQueueingService.GetNextFromQueue());
      //  }
      //});

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

    public void ProcessLine(ReadOnlySequence<byte> line) {
      if (line.Length == 0) {
        return;
      }

      var result = line.ParseAsUTF8String();

      object obj = null;
      try {
        var responseModel = JsonConvert.DeserializeObject<TPResponseBase>(result);

        switch (responseModel.Type.ToLower().Trim()) {
          case "info":
            obj = JsonConvert.DeserializeObject<PairResponse>(result);
            break;
          case "action":
            obj = JsonConvert.DeserializeObject<TPAction>(result);
            break;
          case "listChange":
            obj = JsonConvert.DeserializeObject<TPListChange>(result);
            break;
          case "closePlugin":
            obj = JsonConvert.DeserializeObject<TPClosePlugin>(result);
            break;
        }

        if (obj == null) {
          return;
        }

        //Handle Pair
        if (obj is PairResponse) {
          // Good pairing message returned
          if (!string.IsNullOrEmpty((obj as PairResponse)?.PluginVersion)) {
            //_IsPaired = true;
          }
        }

        //Handle Actions
        if (obj is TPAction) {
          var action = obj as TPAction;
          OnActionEvent(action.ActionId, action.Data);
        }

        // Handle List Changes
        if (obj is TPListChange) {
          var change = obj as TPListChange;
          OnListChangeEventHandler(change.ActionId, change.Value);
        }
      } catch (Exception err) {
        Console.WriteLine(err.Message);
      }

    }

    public void UpdateChoice(ChoiceUpdate choiceUpdate) {
      _tPClient.SendAsync(choiceUpdate);
    }
  }
}
