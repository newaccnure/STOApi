using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using STOApi.Models;
using STOApi.Entities;

namespace STOApi.Interfaces
{
    public interface IOrganizerRepository
    {
        Tournament AddTournament(string email, string name, int numberOfParticipants, 
            int sportId, int eventFormatId, int gameTime, int breakTime,
            TimeSpan gameDayStart, TimeSpan gameDayEnd, DateTime startDate, DateTime endDate);
        bool AutoGenerateTournamentSchedule(int tournamentId);
        bool AddRepresentativesToTournament(int tournamentId, 
            List<string> representativesEmails);
        Schedule GetTournamentSchedule(int tournamentId);
        List<User> GetTournamentRepresentatives(int tournamentId);
        List<User> GetTournamentParticipants(int tournamentId);
        List<Sport> GetSports();
        List<EventFormat> GetEventFormats();
        List<Tournament> GetOngoingTournaments(string email);
        List<Tournament> GetFinishedTournaments(string email);
        List<Tournament> GetIncomingTournaments(string email);
        List<Game> GetIncomingGames(string email);
        List<Game> GetFinishedGamesWithNoScore(string email);
        bool AddScore(int tournamentId, int gameId, int winnerId, int firstParticipantScore, int secondParticipantScore);
        bool DeleteTournament(int tournamentId);
    }
}