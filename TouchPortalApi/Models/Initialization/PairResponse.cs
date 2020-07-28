using TouchPortalApi.Models.TouchPortal.Responses;

namespace TouchPortalApi.Models.Initialization {
  internal class PairResponse : TPResponseBase {
    public string SDKVersion { get; set; }
    public string TPVersionString { get; set; }
    public string TPVersionCode { get; set; }
    public string PluginVersion { get; set; }
  }
}
