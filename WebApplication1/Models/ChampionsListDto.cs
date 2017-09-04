﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApplication1.Models
{
    [DataContract]
    public class ChampionsListDto
    {
        [JsonProperty("data")]
        public Dictionary<String, ChampionDto> data { get; set; }

        [JsonProperty("type")]
        public String type { get; set; }

        [JsonProperty("version")]
        public String version { get; set; }
    }
}