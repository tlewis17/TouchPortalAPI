using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    public class StateRemove
    {
        [JsonProperty]
        public static readonly string Type = "removeState";
        public string Id { get; set; }
    }
}
