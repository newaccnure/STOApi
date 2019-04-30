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
using STOApi.Interfaces;
using STOApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace STOApi.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private STOContext context;

        public OrganizerRepository(STOContext context)
        {
            this.context = context;
        }

        public bool AddRepresentativesToTournament(int tournamentId, List<string> representativesEmails)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any())
            {
                return false;
            }

            foreach (var email in representativesEmails)
            {
                if (context.Users.Where(u => u.Email == email && u.Role == "representative").Any())
                {
                    User representative = context
                                            .Users
                                            .Where(u => u.Email == email
                                                        && u.Role == "representative")
                                            .First();
                    var tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();
                    tournament.UserTournaments.Add(new UserTournament()
                    {
                        User = representative,
                        Tournament = tournament,
                        Joined = false
                    });
                }
                else
                {
                    return false;
                }
            }
            context.SaveChanges();
            return true;
        }

        public Tournament AddTournament(string name, int sportId, int eventFormatId)
        {
            if (!context.Sports.Where(s => s.Id == sportId).Any()
                || !context.EventFormats.Where(ef => ef.Id == eventFormatId).Any()
                || name.Equals(String.Empty))
                return new Tournament();
            if (context
                    .Tournaments
                    .Where(t => t.Name == name
                                && t.SportId == sportId
                                && t.EventFormatId == eventFormatId).Any())
                return new Tournament();

            context.Tournaments.Add(new Tournament()
            {
                Name = name,
                SportId = sportId,
                EventFormatId = eventFormatId
            });
            context.SaveChanges();

            Tournament tournament = context
                                    .Tournaments
                                    .Where(t => t.Name == name
                                                && t.SportId == sportId
                                                && t.EventFormatId == eventFormatId)
                                    .FirstOrDefault();
            return tournament;
        }

        public bool AutoGenerateTournamentSchedule(int tournamentId, int gameTime, int breakTime, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<User> GetTournamentRepresentatives(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any()) return new List<User>();
            return context
                    .UserTournament
                    .Include(ut => ut.User)
                    .Where(ut => ut.TournamentId == tournamentId 
                                    && ut.User.Role=="representative")
                    .Select(ut => ut.User)
                    .ToList();
        }

        public List<User> GetTournamentParticipants(int tournamentId)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any()) return new List<User>();
            return context
                    .UserTournament
                    .Include(ut => ut.User)
                    .Where(ut => ut.TournamentId == tournamentId 
                                    && ut.User.Role=="participant")
                    .Select(ut => ut.User)
                    .ToList();
        }
    }
}