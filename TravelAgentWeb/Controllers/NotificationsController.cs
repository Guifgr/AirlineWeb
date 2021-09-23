using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers
{
    public class NotificationsController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;

        public NotificationsController(TravelAgentDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> FlightChanges(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine($"Webhook Received from: {flightDetailUpdateDto.Publisher}");

            var secretModel =
                await _context.WebhookSecrets.FirstOrDefaultAsync(s => 
                    s.Publisher == flightDetailUpdateDto.Publisher &&
                    s.Secret == flightDetailUpdateDto.Secret);
            if (secretModel == default)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid Secret {flightDetailUpdateDto.Secret} - ignore Webhook");
                Console.ResetColor();
                return BadRequest();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Valid Secret {flightDetailUpdateDto.Secret}");
            Console.WriteLine($"Old price {flightDetailUpdateDto.OldPrice}, New Price {flightDetailUpdateDto.NewPrice}");
            Console.ResetColor();
            return Ok();
        }
    }
}