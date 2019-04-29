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

namespace STOApi.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private STOContext context;

        public TournamentRepository(STOContext context)
        {
            this.context = context;
        }

        public int AddTournament(string name, int sportId, int eventFormatId)
        {
            
            return 0;
        }

    }
}