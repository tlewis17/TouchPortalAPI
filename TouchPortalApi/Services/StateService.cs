using System;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace TouchPortalApi.Services {
  public class StateService : IStateService {
    private readonly ITPClient _tPClient;

    public StateService(ITPClient tPClient) {
      _tPClient = tPClient ?? throw new ArgumentNullException(nameof(tPClient));
    }

    public void UpdateState(StateUpdate stateUpdate) {
      _tPClient.SendAsync(stateUpdate);
    }
  }
}
