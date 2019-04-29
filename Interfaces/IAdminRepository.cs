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
        List<User> GetUsers();
        List<Tournament> GetTournaments();
    }
}