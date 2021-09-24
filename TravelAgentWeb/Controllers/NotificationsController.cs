using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;
using TravelAgentWeb.Models;

namespace TravelAgentWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;

        public NotificationsController(TravelAgentDbContext context)
        {
            _context = context;
        }

        [HttpPost("secret")]
        public ActionResult AddSecret(WebhookSecretDto dto)
        {
            _context.SubscriptionSecrets.Add(new WebhookSecret()
            {
                Publisher = dto.Publisher,
                Secret = dto.Secret
            });
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public ActionResult FlightChanged(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine($"Webhook Receieved from: {flightDetailUpdateDto.Publisher}");

            var secretModel = _context.SubscriptionSecrets.FirstOrDefault(s => 
                s.Publisher == flightDetailUpdateDto.Publisher && 
                s.Secret == flightDetailUpdateDto.Secret);

            if (secretModel == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Secret - Ignore Webwook");
                Console.ResetColor();
                return Ok();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid Webhook!");
            Console.WriteLine($"Old Price {flightDetailUpdateDto.OldPrice}, New Price {flightDetailUpdateDto.NewPrice}");
            Console.ResetColor();
            return Ok();
        }
        

    }
}