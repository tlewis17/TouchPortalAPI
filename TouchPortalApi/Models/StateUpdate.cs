namespace TouchPortalApi.Models {
  public class StateUpdate {
    public const string Type = "stateUpdate";
    public string Id { get; set; }
    public string Value { get; set; }
  }
}
