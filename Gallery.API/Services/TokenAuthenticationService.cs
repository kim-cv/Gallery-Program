using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenData _tokenManagement;

         public TokenAuthenticationService(IOptions<TokenData> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }

        public string GenerateTokenForUser(Guid userUid)
        {
            Claim[] claim = new[]
            {
                new Claim(ClaimTypes.Name, userUid.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _tokenManagement.Issuer,
                _tokenManagement.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
                signingCredentials: credentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }
    }
}
