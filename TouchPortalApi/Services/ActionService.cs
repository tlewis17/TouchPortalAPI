using System;
using System.Collections.Generic;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.Services {
  public class ActionService : IActionService {
    public readonly ITPClient _tPClient;
    public readonly IProcessQueueingService _processQueuingService;

    public ActionService(ITPClient tPClient, IProcessQueueingService processQueueingService) {
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
      _processQueuingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
    }

    public void RegisterActionEvent(string id, Action<List<ActionData>> eventCallback) {
      _processQueuingService.AddEvent<List<ActionData>>(id, eventCallback);
    }
  }
}
