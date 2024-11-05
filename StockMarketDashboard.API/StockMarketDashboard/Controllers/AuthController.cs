using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarketDashboard.Models.AuthModels;
using StockMarketDashboard.Services;
using StockMarketDashboard.Data;

namespace StockMarketDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthController(JwtService jwtService, ApplicationDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            // Add proper hashing in production
            if (user == null || user.PasswordHash != request.Password) 
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return Ok(new UserDto
            {
                Username = user.Username,
                Role = user.Role,
                Token = token
            });
        }
    }
}
