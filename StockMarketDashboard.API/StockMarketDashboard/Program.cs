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

            // Add JWT settings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<JwtService>();

            // Add JWT authentication
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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Define CORS policies
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });

                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://stockmarketdashboard.azurewebsites.net")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configuration settings
            builder.Services.Configure<StockApiConfig>(builder.Configuration.GetSection("StockAPI"));
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<StockService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Load secrets in development
            builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                // Apply CORS policy for Angular frontend in development
                app.UseCors("AllowAngular");
            }
            else
            {
                // Apply CORS policy for specific origin in production
                app.UseCors("AllowSpecificOrigin");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
