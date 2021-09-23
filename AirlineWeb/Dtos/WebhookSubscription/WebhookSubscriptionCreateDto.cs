using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos.WebhookSubscription
{
    public class WebhookSubscriptionCreateDto
    {
        [Required] public string WebhookUri { get; set; }
        [Required] public string WebhookType { get; set; }
    }
}