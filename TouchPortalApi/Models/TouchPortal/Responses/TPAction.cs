using System.Collections.Generic;

namespace TouchPortalApi.Models.TouchPortal.Responses {
  /// <summary>
  /// Class for the TP Response type of action
  /// </summary>
  internal class TPAction : TPShared {
    /// <summary>
    /// Id of the Action
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// Data of the action
    /// </summary>
    public List<ActionData> Data { get; set; }
  }
}
