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

namespace STOApi.Interfaces
{
    public interface IAuthRepository
    {
        TokenResponse Login(string username, string password);
        TokenResponse AddUser(string username, string password, string repeatPassword, string role, string aboutMe);

    }
}