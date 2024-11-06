using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarketDashboard.Models.AuthModels;
using StockMarketDashboard.Services;
using StockMarketDashboard.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = StockMarketDashboard.Models.AuthModels.LoginRequest;
using RegisterRequest = StockMarketDashboard.Models.AuthModels.RegisterRequest;

namespace StockMarketDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher;

        public AuthController(JwtService jwtService, ApplicationDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
        }

        // login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null ||
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                != PasswordVerificationResult.Success)
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if the username already exists -
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username is already taken.");
            }

            // Create a new ApplicationUser -
            var user = new ApplicationUser
            {
                Username = request.Username,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            // Hash the password -
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Add the user to the database -
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
    }
}
