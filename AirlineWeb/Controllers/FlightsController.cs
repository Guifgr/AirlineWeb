using System;
using System.Threading.Tasks;
using AirlineWeb.Data;
using AirlineWeb.Dtos.FlightDetails;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineWeb.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;

        public FlightsController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        
        [HttpPut("{flightId}")]
        public async Task<ActionResult<FlightDetailReadDto>> CreateFlight(int flightId, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flightDetail = await _context.FlightDetails.FirstOrDefaultAsync(f => f.Id == flightId);
            if (flightDetail == default) return NoContent();
            try
            {
                flightDetail.FlightCode = flightDetailUpdateDto.FlightCode;
                flightDetail.Price = flightDetailUpdateDto.Price;
                await _context.SaveChangesAsync();
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