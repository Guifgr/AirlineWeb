using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos.FlightDetails
{
    public class FlightDetailReadDto
    {
        [Required] public int Id { get; set; }
        
        [Required] public string FlightCode { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}