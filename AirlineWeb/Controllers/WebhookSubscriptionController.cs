using System;
using System.Threading.Tasks;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineWeb.Controllers
{
    [Route( "api/[controller]")]
    [ApiController]
    public class WebhookSubscriptionController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }
        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        public async Task<ActionResult<WebhookSubscriptionReadDto>> GetSubscriptionBySecret(string secret)
        {
            var subscription = await _context.WebhookSubscriptions.FirstOrDefaultAsync(s => s.Secret == secret);

            if (subscription == default)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WebhookSubscriptionReadDto>(subscription));
        }
        
        [HttpPost]
        public async Task<ActionResult<WebhookSubscriptionReadDto>> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionDto)
        {
            var subscription = await _context.WebhookSubscriptions.FirstOrDefaultAsync(s => s.WebhookUri == webhookSubscriptionDto.WebhookUri);
            if (subscription == default)
            {
                subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionDto);
                subscription.Secret = Guid.NewGuid().ToString();
                subscription.WebhookPublisher = "PanAus";
                try
                {
                    await _context.WebhookSubscriptions.AddAsync(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message); 
                }

                var webhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);
                return CreatedAtRoute(nameof(GetSubscriptionBySecret), new {secret = subscription.Secret}, webhookSubscriptionReadDto);
            }
            else
            {
                return NoContent();   
            }
        }
    }
}