using System;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.Services {
  public class ChoiceService : IChoiceService {
    public readonly ITPClient _tPClient;
    public readonly IProcessQueueingService _processQueuingService;

    public ChoiceService(ITPClient tPClient, IProcessQueueingService processQueueingService) {
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
      _processQueuingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
    }

    public void RegisterChoiceEvent(string id, Action<string> eventCallback) {
      _processQueuingService.AddEvent<string>(id, eventCallback);
    }

    public void UpdateChoice(ChoiceUpdate choiceUpdate) {
      _tPClient.SendAsync(choiceUpdate);
    }
  }
}
