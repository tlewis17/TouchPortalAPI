using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    public class StateUpdate
    {
        [JsonProperty]
        public static readonly string Type = "stateUpdate";
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
