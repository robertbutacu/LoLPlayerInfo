﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.LolAPIServices;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class PlayerInformationService
    {
            /*
            averageCsPerMinute
            gamesFed
            gamesCarried
            gamesGotCarried
            averageDmgToChampions
            averageDmgToTurrets
            averageDmgToChampionsEnemies
            averageDmgToTurretsEnemies
            averageDmgToTurretsTeammates
            averageDmgToChampionsTeammates
            */
        public PlayerInformationDto Get(string summonerName, string region) 
        {
            SummonerDto summoner = new Summoner().GetByName(summonerName, region);
            if (summoner == null)
                return null;

            MatchlistDto matchesHistory = new Matches().Get(summoner.accountId, region);

            if (matchesHistory == null)
                return null;

            var playerSummary =  new PlayerInformationDto();

            playerSummary.name = summoner.name;
            playerSummary.lanesPlayedCount = ComputeLanesPlayedCount(matchesHistory.matches);

            var detailedGames = GetDetailedGames(matchesHistory.matches, region);

            var averageStats = ComputeAverageStats(detailedGames, summoner.accountId, matchesHistory.totalGames);

            var playerScores = GetPlayerScores(averageStats.Item1.Item1, averageStats.Item1.Item2, averageStats.Item1.Item3);

            DamageDealt averageDamageByEnemies   = new DamageDealt();
            DamageDealt averageDamageByTeammates = new DamageDealt();
            DamageDealt averageDamageByPlayer    = new DamageDealt();

            averageDamageByPlayer.averageDmgToChampions = averageStats.Item2;
            averageDamageByPlayer.averageDmgToTurrets   = averageStats.Item3;

            averageDamageByTeammates.averageDmgToChampions = averageStats.Item4;
            averageDamageByTeammates.averageDmgToTurrets   = averageStats.Item5;

            averageDamageByEnemies.averageDmgToChampions = averageStats.Item6;
            averageDamageByEnemies.averageDmgToTurrets   = averageStats.Item7;

            playerSummary.playerScores           = playerScores;
            playerSummary.damageDealtByEnemies   = averageDamageByEnemies;
            playerSummary.damageDealtByTeammates = averageDamageByTeammates;
            playerSummary.damageDealt            = averageDamageByPlayer;

            return playerSummary;
        }
        /*
            Games carried     -> the player's dmg is the highest.
            Games fed         -> (kills + assists/0.70) / deaths < 1; 
            Games got carried -> 
         */

        private PlayerScores GetPlayerScores(double kills, double deaths, double assists)
        {
            var playerScores = new PlayerScores();

            playerScores.averageKills = kills;
            playerScores.averageDeaths = deaths;
            playerScores.averageAssists = assists;

            playerScores.averageKda = Math.Round((playerScores.averageKills + playerScores.averageAssists) / playerScores.averageDeaths, 2);

            return playerScores;
        }

        private List<MatchDto> GetDetailedGames(List<MatchReferenceDto> matchHistory, string region)
        {
            var detailedMatches = new List<MatchDto>();
            matchHistory.ForEach(delegate (MatchReferenceDto match)
            {
                detailedMatches.Add(new Match().GetById(match.gameId, region));
            });

            return detailedMatches;
        }

        private Dictionary<string, int> ComputeLanesPlayedCount(List<MatchReferenceDto> matchHistory)
        {
            var lanesPlayedCount = new Dictionary<string, int>();
            lanesPlayedCount.Add("MID", 0);
            lanesPlayedCount.Add("JUNGLE", 0);
            lanesPlayedCount.Add("TOP", 0);
            lanesPlayedCount.Add("SUPPORT", 0);
            lanesPlayedCount.Add("ADC", 0);

            matchHistory.ForEach(delegate(MatchReferenceDto match)
                {
                if(match.lane == "BOTTOM" && match.role == "DUO_SUPPORT")
                        lanesPlayedCount["SUPPORT"] += 1;
                if(match.lane == "BOTTOM" && match.role != "DUO_SUPPORT")
                        lanesPlayedCount["ADC"] +=1;
                if (match.lane != "BOTTOM")
                        lanesPlayedCount[match.lane] += 1;
                });

            return lanesPlayedCount;
        }

        private Tuple<Tuple<double, double, double>, double, double, double, double, double, double> ComputeAverageStats(List<MatchDto> matchHistory, long accountId, int numberOfGames)
        {
            int kills = 0, deaths = 0, assists = 0;
            long totalDamageToChampions = 0, totalDamageToTurrets = 0;
            long totalDamageDealtToChampionsByEnemies   = 0  , totalDamageDealtToTurretsByEnemies   = 0;
            long totalDamageDealtToChampionsByTeammates = 0, totalDamageDealtToTurretsByTeammates = 0; 
            matchHistory.ForEach(delegate (MatchDto match)
            {
                int participantId = 0;
                int teamId = 0;
                match.participantsIdentities.ForEach(delegate (ParticipantIdentityDto participantIdentityDto)
                {
                    if (participantIdentityDto.player.accountId == accountId)
                        participantId = participantIdentityDto.participantId;
                });
                match.participants.ForEach(delegate (ParticipantDto participant)
                {
                    if (participant.participantId == participantId)
                    {
                        kills   += participant.stats.kills;
                        deaths  += participant.stats.deaths;
                        assists += participant.stats.assists;
                        totalDamageToChampions += participant.stats.totalDamageDealtToChampions;
                        totalDamageToTurrets   += participant.stats.damageDealtToTurrets;
                        teamId = participant.teamId;
                    } 
                });
                var gameStats = ComputeMatchStats(match.participants, participantId, teamId);
                totalDamageDealtToChampionsByTeammates += gameStats.Item1;
                totalDamageDealtToTurretsByTeammates   += gameStats.Item2;
                totalDamageDealtToChampionsByEnemies   += gameStats.Item3;
                totalDamageDealtToTurretsByEnemies     += gameStats.Item4;
            });

            return new Tuple<Tuple<double, double, double>, double, double, double, double, double, double>(
                new Tuple<double, double, double>(Math.Round(System.Convert.ToDouble(kills) / System.Convert.ToDouble(numberOfGames) * 10, 2),
                                                  Math.Round(System.Convert.ToDouble(deaths) / System.Convert.ToDouble(numberOfGames) * 10, 2),
                                                  Math.Round(System.Convert.ToDouble(assists) / System.Convert.ToDouble(numberOfGames) * 10, 2)),
                Math.Round(System.Convert.ToDouble(totalDamageToChampions) / System.Convert.ToDouble(numberOfGames) * 10, 2),
                Math.Round(System.Convert.ToDouble(totalDamageToTurrets)   / System.Convert.ToDouble(numberOfGames) * 10, 2),
                Math.Round(System.Convert.ToDouble(totalDamageDealtToChampionsByTeammates) / System.Convert.ToDouble(numberOfGames) * 10, 2),
                Math.Round(System.Convert.ToDouble(totalDamageDealtToTurretsByTeammates)   / System.Convert.ToDouble(numberOfGames) * 10, 2),
                Math.Round(System.Convert.ToDouble(totalDamageDealtToChampionsByEnemies)   / System.Convert.ToDouble(numberOfGames) * 10, 2),
                Math.Round(System.Convert.ToDouble(totalDamageDealtToTurretsByEnemies)     / System.Convert.ToDouble(numberOfGames) * 10, 2)
                );
        }

        private double ComputeAverageCsPerMin(List<MatchDto> matchHistory, long accountId)
        {
            return 0;
        }

        private int ComputeGamesFed(List<MatchDto> matchHistory, long accountId)
        {
            return 0;
        }

        private int ComputeGamesCarried(List<MatchDto> matchHistory, long accountId)
        {
            return 0;
        }

        private int ComputeGamesGotCarried(List<MatchDto> matchHistory, long accountId)
        {
            return 0;
        }

        private Tuple<long, long, long, long> ComputeMatchStats(List<ParticipantDto> matchStats, int participantId, int teamId)
        {
            long dmgDealtToChampionsByTeammates = 0, dmgDealtToTurretsByTeammates = 0;
            long dmgDealtToChampionsByEnemies   = 0, dmgDealtToTurretsByEnemies  = 0;

            matchStats.ForEach(delegate (ParticipantDto participant)
            {
                if (participant.teamId == teamId && participant.participantId != participantId)
                {
                    dmgDealtToChampionsByTeammates += participant.stats.totalDamageDealtToChampions;
                    dmgDealtToTurretsByTeammates += participant.stats.damageDealtToTurrets;
                }
                else
                {
                    dmgDealtToChampionsByEnemies += participant.stats.totalDamageDealtToChampions;
                    dmgDealtToTurretsByEnemies += participant.stats.damageDealtToTurrets;
                }
            });

            return new Tuple<long, long, long, long>(dmgDealtToChampionsByTeammates, dmgDealtToTurretsByTeammates,
                                                             dmgDealtToChampionsByEnemies, dmgDealtToTurretsByEnemies
                                                            );
        }
    }
}