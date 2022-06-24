using Newtonsoft.Json;

namespace TouchPortalApi.Models
{
    public class StateCreate
    {
        [JsonProperty]
        public static readonly string Type = "createState";
        public string Id { get; set; }
        public string Desc { get; set; }
        public string DefaultValue { get; set; }
        public string ParentGroup { get; set; }
    }
}
