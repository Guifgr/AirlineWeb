using System;
using System.Threading.Tasks;
using AirlineWeb.Data;
using AirlineWeb.Dtos.FlightDetails;
using AirlineWeb.Dtos.Notification;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;

namespace AirlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _context = context;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet("flightCode", Name = "GetFlightDetailByCode")]
        public async Task<ActionResult<FlightDetailReadDto>> GetFlightDetailByCode(string flightCode)
        {
            var flightDetail = await _context.FlightDetails.FirstOrDefaultAsync(f => f.FlightCode == flightCode);
            
            if (flightCode == default) return NotFound();

            return Ok(_mapper.Map<FlightDetailReadDto>(flightDetail));
        }
        
        [HttpPost]
        public async Task<ActionResult<FlightDetailReadDto>> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
        {
            var flightDetail = await _context.FlightDetails.FirstOrDefaultAsync(f => f.FlightCode == flightDetailCreateDto.FlightCode);
            if (flightDetail != default) return NoContent();
            var flightDetailModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);
            try
            {
                await _context.FlightDetails.AddAsync(flightDetailModel);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); 
            }
            var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);
            return CreatedAtRoute(nameof(GetFlightDetailByCode), new{flightCode = flightDetailReadDto.FlightCode }, flightDetailReadDto);
        }
        
        [HttpPut("{flightId:int}")]
        public async Task<ActionResult<FlightDetailReadDto>> CreateFlight(int flightId, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flightDetail = await _context.FlightDetails.FirstOrDefaultAsync(f => f.Id == flightId);
            if (flightDetail == default) return NoContent();
            try
            {
                if (flightDetail.Price != flightDetailUpdateDto.Price)
                {
                    var message = new NotificationMessageDto()
                    {
                        WebhookType = "pricechange",
                        OldPrice = flightDetail.Price,
                        NewPrice = flightDetailUpdateDto.Price,
                        FlightCode = flightDetail.FlightCode
                    };
                    
                    flightDetail.FlightCode = flightDetailUpdateDto.FlightCode;
                    flightDetail.Price = flightDetailUpdateDto.Price;
                    await _context.SaveChangesAsync();
                    
                    _messageBusClient.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No price change");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); 
            }
            var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetail);
            return Ok(flightDetailReadDto);
        }
    }
}