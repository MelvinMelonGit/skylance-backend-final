using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Web_FlightsController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public Web_FlightsController(SkylanceDbContext db)
        {
            this.db = db;
        }
        
        [HttpGet("AllFlights")]
        public IActionResult GetAllFlights (int page = 1, int pageSize = 4)
        {            
            int totalFlights = db.FlightDetails
                .Where(f => f.DepartureTime > DateTime.Now)
                .Count();
                        
            var flights = db.FlightDetails
                .Include(f => f.Aircraft)
                .Include(f => f.OriginAirport)
                    .ThenInclude(a => a.City)
                .Include(f => f.DestinationAirport)
                    .ThenInclude(a => a.City)
                .Where(f => f.DepartureTime > DateTime.Now)
                .OrderBy(f => f.DepartureTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new Web_FlightsDTO
                {
                    AirlineName = f.Aircraft.Airline,
                    FlightName = f.Aircraft.FlightNumber,
                    DepartureDate = f.DepartureTime.ToString("yyyy-MM-dd"),
                    ArrivalTime = f.ArrivalTime.ToString("s"),
                    DepartureCity = f.OriginAirport.City.Name,
                    ArrivalCity = f.DestinationAirport.City.Name,
                    DepartureCityCode = f.OriginAirport.IataCode,
                    ArrivalCityCode = f.DestinationAirport.IataCode,
                    TotalNoOfPassengers = f.Aircraft.SeatCapacity,
                    TotalNoOfCrew = f.NumberOfCrew,
                    FlightStatus = f.FlightStatus
                })
                .ToList();

            var result = new
            {
                status = "success",
                page,
                pageSize,
                totalFlights,
                flights
            };

            return Ok(result);
        }
    }

}
