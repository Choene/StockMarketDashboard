using System.ComponentModel.DataAnnotations;

namespace StockMarketDashboard.Models.AuthModels
{
    public class LoginRequest
    {
        [Required]
        public string ?Username { get; set; }
        [Required]
        public string ?Password { get; set; }
    }
}
