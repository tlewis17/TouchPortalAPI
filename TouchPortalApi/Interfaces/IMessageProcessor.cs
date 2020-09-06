using System.Collections.Generic;
using System.Threading.Tasks;
using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public delegate void ActionEventHandler(string actionId, List<ActionData> dataList);
  public delegate void ListChangeEventHandler(string actionId, string value);

  public interface IMessageProcessor {
    event ActionEventHandler OnActionEvent;
    event ListChangeEventHandler OnListChangeEventHandler;
    Task Listen();
    Task TryPairAsync();
  }
}