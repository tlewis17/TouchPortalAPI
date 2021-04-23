using System.Collections.Generic;
using System.Threading.Tasks;
using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public delegate void ActionEventHandler(string actionId, List<ActionData> dataList);
  public delegate void ListChangeEventHandler(string actionId, string listId, string instanceId, string value);
  public delegate void CloseEventHandler();
  public delegate void ConnectEventHandler();
  public delegate void SettingEventHandler(List<Dictionary<string, dynamic>> settings);

  public interface IMessageProcessor {
    event ActionEventHandler OnActionEvent;
    event ListChangeEventHandler OnListChangeEventHandler;
    event CloseEventHandler OnCloseEventHandler;
    event ConnectEventHandler OnConnectEventHandler;
    event SettingEventHandler OnSettingEventHandler;

    Task Listen();
    Task TryPairAsync();
    void UpdateChoice(ChoiceUpdate choiceUpdate);
    void CreateState(StateCreate stateCreate);
    void RemoveState(StateRemove stateRemove);
    void UpdateState(StateUpdate stateUpdate);
    }
}