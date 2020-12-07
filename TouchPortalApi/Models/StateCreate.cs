namespace TouchPortalApi.Models {
  public class StateCreate
    {
        public const string Type = "createState";
        public string Id { get; set; }
        public string Desc { get; set; }
        public string DefaultValue { get; set; }
    }
}
