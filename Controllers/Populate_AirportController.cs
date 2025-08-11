using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_AirportController : Controller
    {
        private readonly SkylanceDbContext db;
        public Populate_AirportController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.Airports.Any())
                return BadRequest("Data already seeded.");

            var cities = db.Cities.ToDictionary(c => c.Name, c => c);

            List<Airport> airportList = new List<Airport>
            {
            
            new Airport { IataCode = "SIN", Name = "Singapore Changi Airport", City = cities["Singapore"], TimeZone = "Asia/Singapore"},
            new Airport { IataCode = "NRT", Name = "Narita International Airport", City = cities["Tokyo"], TimeZone = "Asia/Tokyo"},
            new Airport { IataCode = "KUL", Name = "Kuala Lumpur International Airport", City = cities["Kuala Lumpur"], TimeZone = "Asia/Kuala_Lumpur"},
            new Airport { IataCode = "ICN", Name = "Incheon International Airport", City = cities["Seoul"], TimeZone = "Asia/Seoul"},
            new Airport { IataCode = "CBR", Name = "Canberra International Airport", City = cities["Canberra"], TimeZone = "Australia/Sydney"},           
            new Airport { IataCode = "AUH", Name = "Abu Dhabi International Airport", City = cities["Abu Dhabi"], TimeZone = "Asia/Dubai" },
            new Airport { IataCode = "ZRH", Name = "Zurich Airport", City = cities["Zurich"], TimeZone = "Europe/Zurich" },
            new Airport { IataCode = "HAN", Name = "Noi Bai International Airport", City = cities["Hanoi"], TimeZone = "Asia/Bangkok" }
        };

            db.Airports.AddRange(airportList);
            db.SaveChanges();

            return Ok("Airports seeded successfully.");
        }

    }
}