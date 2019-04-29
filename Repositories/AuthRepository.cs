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
    public class AuthRepository : IAuthRepository
    {

        private List<string> roles = new List<string>(){
            "admin",
            "participant",
            "organizer",
            "representative"
        };
        private STOContext context;

        public AuthRepository(STOContext context)
        {
            this.context = context;
        }

        public TokenResponse AddUser(string email, string password, string repeatPassword, string role, string aboutMe)
        {
            if (context.Users.Where(x => x.Email == email && x.Role == role).Any())
            {
                return new TokenResponse();
            }

            if (!password.Equals(repeatPassword))
            {
                return new TokenResponse();
            }

            if (!roles.Where(r => r.Equals(role)).Any())
            {
                return new TokenResponse();
            }

            context.Users.Add(new User()
            {
                Email = email,
                Password = password,
                Role = role,
                AboutMeInfo = aboutMe
            });
            context.SaveChanges();

            return Login(email, password);
        }

        public TokenResponse Login(string email, string password)
        {
            var identity = GetIdentity(email, password);
            if (identity == null)
            {
                return new TokenResponse();
            }

            var now = DateTime.UtcNow;
            
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                notBefore: now,
                audience: AuthOptions.AUDIENCE,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new TokenResponse()
            {
                Token = encodedJwt,
                Email = identity.Name
            };

            return response;
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            User person = context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}