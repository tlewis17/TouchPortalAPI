using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StateCreate
    {
        [JsonProperty]
        public static readonly string Type = "createState";
        [JsonProperty]
        public string Id { get; set; }
        [JsonProperty]
        public string Desc { get; set; }
        [JsonProperty]
        public string DefaultValue { get; set; }
    }
}
