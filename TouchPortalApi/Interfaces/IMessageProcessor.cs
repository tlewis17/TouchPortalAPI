﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public delegate void ActionEventHandler(string actionId, List<ActionData> dataList);
  public delegate void ListChangeEventHandler(string actionId, string value);
  public delegate void CloseEventHandler();
  public delegate void ConnectEventHandler();

  public interface IMessageProcessor {
    event ActionEventHandler OnActionEvent;
    event ListChangeEventHandler OnListChangeEventHandler;
    event CloseEventHandler OnCloseEventHandler;
    event ConnectEventHandler OnConnectEventHandler;

    Task Listen();
    Task TryPairAsync();
    void UpdateChoice(ChoiceUpdate choiceUpdate);
    void CreateState(CreateState createState);
    void RemoveState(RemoveState removeState);
    void UpdateState(StateUpdate stateUpdate);
    }
}