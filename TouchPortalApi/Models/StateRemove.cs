using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StateRemove
    {
        [JsonProperty]
        public static readonly string Type = "removeState";
        [JsonProperty]
        public string Id { get; set; }
    }
}
