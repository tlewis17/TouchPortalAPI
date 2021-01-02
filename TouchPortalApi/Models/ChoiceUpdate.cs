using Newtonsoft.Json;
using System.ComponentModel;

namespace TouchPortalApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ChoiceUpdate
    {
        [JsonProperty]
        public static readonly string Type = "choiceUpdate";
        [JsonProperty]
        public string Id { get; set; }
        [DefaultValue("")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string InstanceId { get; set; }
        [JsonProperty]
        public string[] Value { get; set; }
    }
}
