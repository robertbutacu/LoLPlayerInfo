﻿using System.Runtime.Serialization;

namespace WebApplication1.Models
{
    [DataContract]
    public class GamesSummary
    {
        [DataMember]
        public int gamesFed;

        [DataMember]
        public int gamesCarried;

        [DataMember]
        public int gamesGotCarried;

        public GamesSummary() { }

        public void Add(GamesSummary gamesSummary)
        {
            this.gamesFed        += gamesSummary.gamesFed;
            this.gamesCarried    += gamesSummary.gamesCarried;
            this.gamesGotCarried += gamesSummary.gamesGotCarried;
        }
    }
}