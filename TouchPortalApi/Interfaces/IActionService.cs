using System;
using System.Collections.Generic;
using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public interface IActionService {
    void RegisterActionEvent(string id, Action<List<ActionData>> eventCallback);
  }
}