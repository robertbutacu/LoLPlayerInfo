﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class DamageService
    {
        public DamageDealt ComputeDamageDealtByTeam(List<MatchDto> matches, long accountId, Boolean isPlayerTeam)
        {
            DamageDealt dmgDealtByTeam = new DamageDealt();

            matches.ForEach(delegate (MatchDto match)
            {
                int playerParticipantId = Retrieve.ParticipantIdForCurrentMatch(match.participantsIdentities, accountId);

                int playerTeamId = Retrieve.PlayerTeamId(match.participants, playerParticipantId);

                dmgDealtByTeam.Add(ComputeDamageDealtByTeam(match.participants, playerTeamId, playerParticipantId));

            });

            dmgDealtByTeam.Normalize(matches.Count * (isPlayerTeam ? 4 : 5));

            return dmgDealtByTeam;
        }


        public DamageDealt ComputeAverageDamageDealtByPlayer(List<MatchDto> matches, long accountId)
        {
            DamageDealt dmgDealtByPlayer = new DamageDealt();
            int totalGames = matches.Count;

            matches.ForEach(delegate (MatchDto match)
            {
                int playerParticipantId = Retrieve.ParticipantIdForCurrentMatch(match.participantsIdentities, accountId);

                dmgDealtByPlayer.Add(ComputeDamageDealtByPlayer(match.participants, playerParticipantId));
            });

            dmgDealtByPlayer.Normalize(matches.Count);

            return dmgDealtByPlayer;
        }


        public DamageDealt ComputeDamageDealtByPlayer(List<ParticipantDto> participants, int participantId)
        {
            DamageDealt dmg = new DamageDealt();

            participants.ForEach(delegate (ParticipantDto participant)
            {
                if (participant.participantId == participantId)
                    dmg.Add(participant.stats.totalDamageDealtToChampions, participant.stats.damageDealtToTurrets);
            });

            return dmg;
        }

        public DamageDealt GetHighestDamageDealerInTeam(List<ParticipantDto> participants, int teamId, int participantId)
        {
            DamageDealt highestInTeam = new DamageDealt();
            participants.ForEach(delegate (ParticipantDto participant)
            {
                if (participant.stats.totalDamageDealtToChampions > highestInTeam.averageDmgToChampions &&
                    participant.participantId != participantId &&
                    participant.teamId == teamId)
                    highestInTeam.ReplaceDamage(participant.stats.totalDamageDealtToChampions,
                                                participant.stats.damageDealtToTurrets);
            });

            return highestInTeam;
        }


        private DamageDealt ComputeDamageDealtByTeam(List<ParticipantDto> participants, int teamId, int playerParticipantId)
        {
            DamageDealt dmgDealt = new DamageDealt();
            participants.ForEach(delegate (ParticipantDto participant)
            {
                if (participant.teamId == teamId && participant.participantId != playerParticipantId)
                    dmgDealt.Add(participant.stats.totalDamageDealtToChampions, participant.stats.damageDealtToTurrets);
            });

            return dmgDealt;
        }
    }
}