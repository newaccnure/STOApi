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
    public class AuthRepository : IAuthRepository
    {
        private STOContext context;

        public AuthRepository(STOContext context)
        {
            this.context = context;
        }

        public TokenResponse AddUser(string username, string password, string repeatPassword, string role)
        {
            context.Users.Add(new User(){
                Email = username,
                Password = password,
                Role = role
            });
            context.SaveChanges();
            
            return Login(username, password);
        }

        public TokenResponse Login(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
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

        public List<User> GetUsers()
        {
            return context.Users.ToList();
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User person = context.Users.FirstOrDefault(x => x.Email == username && x.Password == password);
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

            // если пользователя не найдено
            return null;
        }
    }
}