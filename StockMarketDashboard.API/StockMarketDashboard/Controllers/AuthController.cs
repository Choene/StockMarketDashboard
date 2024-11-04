using Microsoft.AspNetCore.Mvc;
using StockMarketDashboard.Models.AuthModels;
using StockMarketDashboard.Services;

namespace StockMarketDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        // In a real application, would need to inject a user service/repository
        private readonly Dictionary<string, string> _users = new()
        {
            // Demo purposes only - will need proper password hashing in production
            { "admin", "admin123" }, 
            { "user", "user123" }
        };

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Demo authentication - will replace with actual user authentication logic
            if (!_users.TryGetValue(request.Username, out var password) ||
                password != request.Password)
            {
                return Unauthorized("Invalid username or password");
            }

            var role = request.Username == "admin" ? "Admin" : "User";
            var token = _jwtService.GenerateToken(request.Username, role);

            return Ok(new UserDto
            {
                Username = request.Username,
                Role = role,
                Token = token
            });
        }
    }
}
