using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Linq;
using DemoWebApp.Models;
using DemoWebApp.Helpers;

namespace DemoWebApp.Services
{
    public interface IAuthService
    {
        TokenedUser Authenticate(string identityType, string identity, string secret);
    }

    public class AuthService : IAuthService
    {
        private readonly AppSettings settings;
        private readonly ApplicationDbContext context;

        public AuthService(IOptions<AppSettings> settings, ApplicationDbContext context)
        {
            this.settings = settings.Value;
            this.context = context;
        }

        public TokenedUser Authenticate(string identityType, string identity, string secret)
        {
            User user = null;

            if (identityType == "username")
            {
                user = context.Users.FirstOrDefault(u => u.Name == identity && u.Password == CryptoHelper.EncryptPassword(secret));
            }
            else if (identityType == "email")
            {
                user = context.Users.FirstOrDefault(u => u.Email == identity && u.Password == CryptoHelper.EncryptPassword(secret));
            }

            if (user == null) return null;

            var token = AuthHelper.GenerateJwtToken(settings.JwtKey, settings.JwtIssuer, user);

            return new TokenedUser(user, token);
        }
    }
}