using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotPlanning.DataModels
{
    public class Plan
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string date { get; set; }

        [JsonProperty(PropertyName = "time9")]
        public string time9 { get; set; }

        [JsonProperty(PropertyName = "time10")]
        public string time10 { get; set; }

        [JsonProperty(PropertyName = "time11")]
        public string time11 { get; set; }

        [JsonProperty(PropertyName = "time12")]
        public string time12 { get; set; }

        [JsonProperty(PropertyName = "time13")]
        public string time13 { get; set; }

        [JsonProperty(PropertyName = "time14")]
        public string time14 { get; set; }

        [JsonProperty(PropertyName = "time15")]
        public string time15 { get; set; }

        [JsonProperty(PropertyName = "time16")]
        public string time16 { get; set; }


    }
}