using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StateUpdate
    {
        [JsonProperty]
        public static readonly string Type = "stateUpdate";
        [JsonProperty]
        public string Id { get; set; }
        [JsonProperty]
        public string Value { get; set; }
    }
}
