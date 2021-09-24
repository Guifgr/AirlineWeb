using System.ComponentModel.DataAnnotations;

namespace TravelAgentWeb.Dtos
{
    public class WebhookSecretDto
    {
        [Required]
        public string Secret { get; set; }  

        [Required]
        public string Publisher { get; set; }
    }
}