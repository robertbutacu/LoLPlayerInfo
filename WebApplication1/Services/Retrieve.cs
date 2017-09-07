﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public static class Retrieve
    {
        public static int ParticipantIdForCurrentMatch(List<ParticipantIdentityDto> participantIdentities, long accountId)
        {
            int participantId = 0;
            participantIdentities.ForEach(delegate (ParticipantIdentityDto participant)
            {
                if (participant.player.accountId == accountId)
                    participantId = participant.participantId;
            });

            return participantId;
        }

        public static int PlayerTeamId(List<ParticipantDto> participants, int participantId)
        {
            int teamId = 0;
            participants.ForEach(delegate (ParticipantDto participant)
            {
                if (participant.participantId == participantId)
                    teamId = participant.teamId;
            });

            return teamId;
        }
    }
}