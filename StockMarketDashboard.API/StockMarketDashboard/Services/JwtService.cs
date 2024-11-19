using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockMarketDashboard.Models.AuthModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockMarketDashboard.Services
{
    public class JwtService
    {
        private readonly SymmetricSecurityKey _securityKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationInMinutes;

        public JwtService(IConfiguration configuration)
        {
            var key = configuration["JwtSettings:Key"];

            // Attempt to decode from Base64, if needed
            try
            {
                _securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));
            }
            catch (FormatException)
            {
                // Use the raw string as key if it's not Base64 encoded
                _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            }

            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _expirationInMinutes = int.Parse(configuration["JwtSettings:ExpirationInMinutes"]);
        }

        public string GenerateToken(string username, string role)
        {
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
