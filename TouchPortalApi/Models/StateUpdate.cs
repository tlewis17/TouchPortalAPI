namespace TouchPortalApi.Models {
  public class StateUpdate {
    public static readonly string Type = "stateUpdate";
    public string Id { get; set; }
    public string Value { get; set; }
  }
}
