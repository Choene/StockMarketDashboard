using Microsoft.EntityFrameworkCore;
using StockMarketDashboard.Models.AuthModels;

namespace StockMarketDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}
