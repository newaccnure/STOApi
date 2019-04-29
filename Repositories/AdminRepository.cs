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
using STOApi.Interfaces;
using STOApi.Models;

namespace STOApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private STOContext context;

        public AdminRepository(STOContext context)
        {
            this.context = context;
        }

        public EventFormat AddEventFormat(string name)
        {
            if (name.Equals(String.Empty)) return new EventFormat();
            if (context.EventFormats.Where(ef => ef.Name.Equals(name)).Any()) return new EventFormat();
            context.EventFormats.Add(new EventFormat()
            {
                Name = name
            });
            context.SaveChanges();
            return context.EventFormats.Where(ef => ef.Name.Equals(name)).FirstOrDefault();
        }

        public Sport AddSport(string name)
        {
            if (name.Equals(String.Empty)) return new Sport();
            if (context.Sports.Where(ef => ef.Name.Equals(name)).Any()) return new Sport();
            context.Sports.Add(new Sport()
            {
                Name = name
            });
            context.SaveChanges();
            return context.Sports.Where(ef => ef.Name.Equals(name)).FirstOrDefault();
        }

        public bool DeleteEventFormat(int eventFormatId)
        {
            if (context.EventFormats.Where(ef => ef.Id == eventFormatId).Any()) return false;
            EventFormat eventFormat = context.EventFormats.Where(ef => ef.Id == eventFormatId).First();
            context.Remove(eventFormat);
            context.SaveChanges();
            return true;
        }

        public bool DeleteSport(int sportId)
        {
            if (context.Sports.Where(s => s.Id == sportId).Any()) return false;
            Sport sport = context.Sports.Where(s => s.Id == sportId).First();
            context.Remove(sport);
            context.SaveChanges();
            return true;
        }

        public bool DeleteTournament(int tournamentId)
        {
            if (context.Tournaments.Where(t => t.Id == tournamentId).Any()) return false;
            Tournament tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();
            context.Remove(tournament);
            context.SaveChanges();
            return true;
        }

        public List<EventFormat> GetEventFormats()
        {
            return context.EventFormats.ToList();
        }

        public List<Sport> GetSports()
        {
            return context.Sports.ToList();
        }

        public List<Tournament> GetTournaments()
        {
            return context.Tournaments.ToList();
        }

        public List<User> GetUsers()
        {
            return context.Users.ToList();
        }

    }
}