using System.Collections.Generic;

namespace TouchPortalApi.Models.TouchPortal.Responses {
  /// <summary>
  /// Class for the TP Response type of broadcast
  /// </summary>
  internal class TPBroadcast : TPShared {
    /// <summary>
    /// Event type
    /// </summary>
    public string Event { get; set; }

    /// <summary>
    /// Name of the page that was switched to
    /// </summary>
    public string PageName { get; set; }
  }
}
