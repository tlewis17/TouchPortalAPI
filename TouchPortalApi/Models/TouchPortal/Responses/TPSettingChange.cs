using System.Collections.Generic;

namespace TouchPortalApi.Models.TouchPortal.Responses {
  /// <summary>
  /// Class for the TP Response type of Setting Change
  /// </summary>
  /// 
  internal class TPSettingChange : TPShared {
    /// <summary>
    /// New values
    /// </summary>
    public List<Dictionary<string, dynamic>> Values { get; set; }
  }
}
