using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public interface IStateService {
    void UpdateState(StateUpdate stateUpdate);
  }
}