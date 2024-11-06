using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockMarketDashboard.Data;
using StockMarketDashboard.Models;
using StockMarketDashboard.Models.AuthModels;
using StockMarketDashboard.Services;
using System.Text;

namespace StockMarketDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load secrets first
            builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

            // Configure JWT settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<JwtService>();

            // Add JWT authentication
            var jwtKey = builder.Configuration["JwtSettings:Key"];
            SymmetricSecurityKey securityKey;
            try
            {
                securityKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey));
            }
            catch (FormatException)
            {
                securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = securityKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Define CORS policies
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://stockmarketdashboard.azurewebsites.net")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Configure Stock API and database connection
            builder.Services.Configure<StockApiConfig>(builder.Configuration.GetSection("StockAPI"));
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Redis Cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "StockMarketDashboard_";
            });

            // Add services to the container
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<StockService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Apply CORS policy
            app.UseCors("AllowAngular");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
