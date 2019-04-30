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
        Tournament AddTournament(string name, int numberOfParticipants, int sportId, int eventFormatId);
        bool AutoGenerateTournamentSchedule(int tournamentId, int gameTime, int gameDayStart, int gameDayEnd,
            int breakTime, DateTime startDate, DateTime endDate);
        bool AddRepresentativesToTournament(int tournamentId, 
            List<string> representativesEmails);
        Schedule GetTournamentSchedule(int tournamentId);
        List<User> GetTournamentRepresentatives(int tournamentId);
        List<User> GetTournamentParticipants(int tournamentId);
    }
}