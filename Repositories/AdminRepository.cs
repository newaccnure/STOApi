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

        public List<Tournament> GetTournaments()
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            return context.Users.ToList();
        }

    }
}