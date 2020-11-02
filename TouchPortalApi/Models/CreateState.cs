using System;
using System.Collections.Generic;
using System.Text;

namespace TouchPortalApi.Models
{
    public class CreateState
    {
        public readonly string Type = "createState";
        public string Id { get; set; }
        public string Desc { get; set; }
        public string DefaultValue { get; set; }
    }
}
