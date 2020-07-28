using System;
using TouchPortalApi.Models;

namespace TouchPortalApi.Interfaces {
  public interface IChoiceService {
    void RegisterChoiceEvent(string id, Action<string> eventCallback);
    void UpdateChoice(ChoiceUpdate choiceUpdate);
  }
}