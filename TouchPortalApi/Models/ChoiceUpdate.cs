using Newtonsoft.Json;
using System.ComponentModel;

namespace TouchPortalApi.Models {
  public class ChoiceUpdate {
    public readonly string Type = "choiceUpdate";
    public string Id { get; set; }
    [DefaultValue("")]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string InstanceId { get; set; }
    public string[] Value { get; set; }
  }
}
