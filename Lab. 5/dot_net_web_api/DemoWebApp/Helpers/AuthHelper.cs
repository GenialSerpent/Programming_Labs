using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DemoWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace DemoWebApp.Helpers
{
    public static class AuthHelper
    {
        public static AuthenticationTicket ValidateJwtToken(string jwtKey, string jwtIssuer, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = jwtIssuer
                }, out var validatedToken);

                if (validatedToken == null)
                {
                    return null;
                }

                var identity = new ClaimsIdentity(((JwtSecurityToken)validatedToken).Claims, "JwtBearerAuthentication");

                return new AuthenticationTicket(new ClaimsPrincipal(identity), "JwtBearerAuthentication");
            }
            catch
            {
                return null;
            }
        }

        public static string GenerateJwtToken(string jwtKey, string jwtIssuer, User user)
        {
            var key = Encoding.ASCII.GetBytes(jwtKey);

            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, user.isAdmin ? "admin" : "user"),
                    new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(new User()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        DisplayName = user.DisplayName,
                        isAdmin = user.isAdmin
                    }))
                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtIssuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static User GetUser(ClaimsPrincipal user)
        {
            return JsonConvert.DeserializeObject<User>(user?.Identities.FirstOrDefault()?.Claims.FirstOrDefault(t => t.Type == ClaimTypes.UserData)?.Value);
        }
    }
}