using System.ComponentModel.DataAnnotations;

namespace StockMarketDashboard.Models.AuthModels
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        [Required]
        public string ?Username { get; set; }
        [Required]
        public string ?PasswordHash { get; set; }
        public string ?Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
