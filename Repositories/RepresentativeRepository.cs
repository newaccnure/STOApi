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
    public class RepresentativeRepository : IRepresentativeRepository
    {
        private STOContext context;

        public RepresentativeRepository(STOContext context)
        {
            this.context = context;
        }

        public bool AddParticipantsToTournament(int tournamentId, List<string> participantsEmails)
        {
            if (!context.Tournaments.Where(t => t.Id == tournamentId).Any())
            {
                return false;
            }

            foreach (var email in participantsEmails)
            {
                if (context.Users.Where(u => u.Email == email && u.Role == "participant").Any())
                {
                    User participant = context
                                            .Users
                                            .Where(u => u.Email == email
                                                        && u.Role == "participant")
                                            .First();
                    var tournament = context.Tournaments.Where(t => t.Id == tournamentId).First();
                    tournament.UserTournaments.Add(new UserTournament()
                    {
                        User = participant,
                        Tournament = tournament,
                        Joined = false,
                        
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
    }
}