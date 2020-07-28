namespace TouchPortalApi.Models.TouchPortal.Responses {
  /// <summary>
  /// Class for the TP Response type of List Change
  /// </summary>
  /// 
  internal class TPListChange : TPShared {
    /// <summary>
    /// Id of the Action
    /// </summary>
    public string ActionId { get; set; }

    /// <summary>
    /// Id of the list being used in the inline action
    /// </summary>
    public string ListId { get; set; }

    /// <summary>
    /// Id of the instance
    /// </summary>
    public string InstanceId { get; set; }

    /// <summary>
    /// New value
    /// </summary>
    public string Value { get; set; }
  }
}
