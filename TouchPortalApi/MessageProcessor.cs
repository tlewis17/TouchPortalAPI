using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
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
    #region private vars

    private readonly IOptionsMonitor<TouchPortalApiOptions> _options;
    private readonly ITPClient _tPClient;
    private readonly IProcessQueueingService _processQueueingService;
    private readonly CancellationToken _cancellationToken;

    #endregion

    #region Event Handlers

    public event ActionEventHandler OnActionEvent;
    public event ListChangeEventHandler OnListChangeEventHandler;
    public event CloseEventHandler OnCloseEventHandler;
    public event ConnectEventHandler OnConnectEventHandler;
    public event SettingEventHandler OnSettingEventHandler;

    #endregion

    /// <summary>
    /// If the TP Client is paired already or not.
    /// </summary>
    public bool IsPaired { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    /// <param name="tPClient"></param>
    /// <param name="processQueueingService"></param>
    /// <param name="cancellationToken"></param>
    public MessageProcessor(IOptionsMonitor<TouchPortalApiOptions> options, ITPClient tPClient, IProcessQueueingService processQueueingService,
      CancellationToken cancellationToken = default) {
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
      _processQueueingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
      _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Listens for new messages from Touch Portal
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Tries to connect to Touch Portal
    /// </summary>
    /// <returns></returns>
    public async Task TryPairAsync() {
      await _tPClient.SendAsync(new PairRequest { Id = _options.CurrentValue.PluginId });
    }

    /// <summary>
    /// Process an incoming message
    /// </summary>
    /// <param name="line">Incoming <see cref="ReadOnlySequence{byte}"/></param>
    private void ProcessLine(ReadOnlySequence<byte> line) {
      if (line.Length == 0) {
        return;
      }

      var result = line.ParseAsUTF8String();

      try {
        var responseModel = JsonConvert.DeserializeObject<TPResponseBase>(result);

        switch (responseModel.Type.ToLower().Trim()) {
          case "info":
            PairResponse pairResponse = JsonConvert.DeserializeObject<PairResponse>(result);
            HandlePairEvent(pairResponse);
            if(pairResponse.Settings != null)
              HandleSettingEvent(pairResponse.Settings);
            break;
          case "action":
            HandleActionEvent(JsonConvert.DeserializeObject<TPAction>(result));
            break;
          case "listchange":
            HandleListChangeEvent(JsonConvert.DeserializeObject<TPListChange>(result));
            break;
          case "closeplugin":
            HandleCloseEvent();
            break;
          case "settings":
            HandleSettingEvent(JsonConvert.DeserializeObject<TPSettingChange>(result).Values);
            break;
          default:
            Console.WriteLine($"No operation defined for: {responseModel.Type.ToLower().Trim()}");
            break;
        }
      } catch (Exception err) {
        Console.WriteLine($"Exception - {err.Message}");
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

        OnConnectEventHandler?.Invoke();
      }
    }

    /// <summary>
    /// Handle connect event
    /// </summary>
    /// <param name="response">The TP Pair Response</param>
    private void HandleSettingEvent(List<Dictionary<string, dynamic>> settings) {
      // Good pairing message returned
      OnSettingEventHandler?.Invoke(settings);
    }

    /// <summary>
    /// Handle an action event
    /// </summary>
    /// <param name="action">The action being triggered</param>
    private void HandleActionEvent(TPAction action) {
      OnActionEvent?.Invoke(action.ActionId, action.Data);
    }

    /// <summary>
    /// Handle a list change event
    /// </summary>
    /// <param name="change">The list change event</param>
    private void HandleListChangeEvent(TPListChange change) {
      OnListChangeEventHandler?.Invoke(change.ActionId, change.ListId, change.InstanceId, change.Value);
    }

    /// <summary>
    /// Handle a plugin close event
    /// </summary>
    private void HandleCloseEvent() {
      OnCloseEventHandler?.Invoke();
    }

    /// <summary>
    /// Updates a choice
    /// </summary>
    /// <param name="choiceUpdate"></param>
    public void UpdateChoice(ChoiceUpdate choiceUpdate) {
      _tPClient.SendAsync(choiceUpdate);
    }

    /// <summary>
    /// Creates a new state
    /// </summary>
    /// <param name="createState"></param>
    public void CreateState(StateCreate stateCreate) {
      _tPClient.SendAsync(stateCreate);
    }

    /// <summary>
    /// Removes a state
    /// </summary>
    /// <param name="removeState"></param>
    public void RemoveState(StateRemove stateRemove) {
      _tPClient.SendAsync(stateRemove);
    }

    /// <summary>
    /// Updates the value of a state
    /// </summary>
    /// <param name="stateUpdate"></param>
    public void UpdateState(StateUpdate stateUpdate) {
      _tPClient.SendAsync(stateUpdate);
    }
  }
}
