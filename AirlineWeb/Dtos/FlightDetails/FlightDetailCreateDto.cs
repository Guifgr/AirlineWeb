using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos.FlightDetails
{
    public class FlightDetailCreateDto
    {
        [Required] public string FlightCode { get; set; }
        
        [Required]
        public decimal Price { get; set; }
    }
}