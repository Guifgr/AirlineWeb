using System.ComponentModel.DataAnnotations;

namespace TravelAgentWeb.Model
{
    public class WebhookSecret
    {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Secret { get; set; }
        [Required] public string Publisher { get; set; }
    }
}