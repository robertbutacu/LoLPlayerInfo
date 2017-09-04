﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApplication1.Models
{
    [DataContract]
    public class MatchParticipantFrameDto
    {
        [JsonProperty("totalGold")]
        public int totalGold { get; set; }

        [JsonProperty("teamScore")]
        public int teamScore { get; set; }

        [JsonProperty("participantId")]
        public int participantId { get; set; }

        [JsonProperty("level")]
        public int level { get; set; }

        [JsonProperty("currentGold")]
        public int currentGold { get; set; }

        [JsonProperty("minionsKilled")]
        public int minionsKilled { get; set; }

        [JsonProperty]

        [JsonProperty]

        [JsonProperty]

        [JsonProperty]

    }
}