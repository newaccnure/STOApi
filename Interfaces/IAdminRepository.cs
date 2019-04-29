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
using STOApi.Entities;

namespace STOApi.Interfaces
{
    public interface IAdminRepository
    {
        Sport AddSport(string name);
        EventFormat AddEventFormat(string name);
        bool DeleteSport(int sportId);
        bool DeleteEventFormat(int eventFormatId);
        bool DeleteTournament(int tournamentId);
        List<User> GetUsers();
        List<Tournament> GetTournaments();
        List<Sport> GetSports();
        List<EventFormat> GetEventFormats();


    }
}