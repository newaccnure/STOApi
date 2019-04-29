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

namespace STOApi.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private STOContext context;

        public OrganizerRepository(STOContext context)
        {
            this.context = context;
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

    }
}