using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpiritTime.Core.Entities;

namespace SpiritTime.Backend.Infrastructure.Jwt
{
    public class JwtAuthentication
    {
        private readonly string _securityKey;
        private readonly string _validAudience;
        private readonly string _validIssuer;

        public JwtAuthentication(IConfiguration configuration)
        {
            _securityKey = configuration["Jwt:SecurityKey"] ??
                           throw new InvalidOperationException("Set the 'Jwt:SecurityKey' on appSettings");
            _validAudience = configuration["Jwt:ValidAudience"] ??
                             throw new InvalidOperationException("Set the 'Jwt:ValidAudience' on appSettings");
            _validIssuer = configuration["Jwt:ValidIssuer"] ??
                           throw new InvalidOperationException("Set the 'Jwt:ValidIssuer' on appSettings");
        }

        public string GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //new Claim(ClaimTypes.Role, roles),
                new Claim(ClaimTypes.Name, user.Email)
            };

            return GetJwtToken(claims);
        }



        private string GetJwtToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);

            var tokeOptions = new JwtSecurityToken(
                _validIssuer,
                _validAudience,
                claims,
                expires: DateTime.UtcNow.AddDays(24),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
