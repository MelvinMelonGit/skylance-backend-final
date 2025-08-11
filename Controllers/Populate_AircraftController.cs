using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_AircraftController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public Populate_AircraftController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {

            if (db.Aircraft.Any())            
                return BadRequest("Data already seeded.");

            List<Aircraft> aircraftList = new List<Aircraft>
            {     
                new Aircraft { Airline = "Singapore Airlines", FlightNumber = "SQ322", AircraftBrand = "Boeing", AircraftModel = "737 Max", SeatCapacity = 180 },
                new Aircraft { Airline = "Japan Airlines", FlightNumber = "JL1", AircraftBrand = "Airbus", AircraftModel = "A320 Neo", SeatCapacity = 150 },
                new Aircraft { Airline = "Malaysia Airlines", FlightNumber = "MH1", AircraftBrand = "Boeing", AircraftModel = "787 Dreamliner", SeatCapacity = 242 },
                new Aircraft { Airline = "Korean Air", FlightNumber = "KE85", AircraftBrand = "Airbus", AircraftModel = "A350", SeatCapacity = 325 },
                new Aircraft { Airline = "Qantas", FlightNumber = "QF1", AircraftBrand = "Embraer", AircraftModel = "E195", SeatCapacity = 120 },            
                new Aircraft { Airline = "Singapore Airlines", FlightNumber = "SQ12", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 303 },
                new Aircraft { Airline = "Qantas", FlightNumber = "QF93", AircraftBrand = "Boeing", AircraftModel = "787-9 Dreamliner", SeatCapacity = 236 },
                new Aircraft { Airline = "Etihad Airways", FlightNumber = "EY101", AircraftBrand = "Airbus", AircraftModel = "A380", SeatCapacity = 496 },
                new Aircraft { Airline = "Swiss International Air Lines", FlightNumber = "LX38", AircraftBrand = "Boeing", AircraftModel = "777-300ER", SeatCapacity = 340 },
                new Aircraft { Airline = "Vietnam Airlines", FlightNumber = "VN50", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 305 },
                new Aircraft { Airline = "Korean Air", FlightNumber = "KE621", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 300 },
                new Aircraft { Airline = "Singapore Airlines", FlightNumber = "SQ26", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 320 },
                new Aircraft { Airline = "KLM Royal Dutch Airlines", FlightNumber = "KL605", AircraftBrand = "Boeing", AircraftModel = "777-300ER", SeatCapacity = 408 },
                new Aircraft { Airline = "Japan Airlines", FlightNumber = "JL37", AircraftBrand = "Boeing", AircraftModel = "787-9 Dreamliner", SeatCapacity = 300 },
                new Aircraft { Airline = "Aerolineas Argentinas", FlightNumber = "AR1301", AircraftBrand = "Boeing", AircraftModel = "737-800", SeatCapacity = 162 },
                new Aircraft { Airline = "Iberia", FlightNumber = "IB6401", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 348 },
                new Aircraft { Airline = "Air France", FlightNumber = "AF83", AircraftBrand = "Boeing", AircraftModel = "777-300ER", SeatCapacity = 296 },
                new Aircraft { Airline = "British Airways", FlightNumber = "BA283", AircraftBrand = "Airbus", AircraftModel = "A380-800", SeatCapacity = 469 },
                new Aircraft { Airline = "Lufthansa", FlightNumber = "LH492", AircraftBrand = "Boeing", AircraftModel = "747-8", SeatCapacity = 364 },
                new Aircraft { Airline = "LATAM Airlines Brasil", FlightNumber = "LA8070", AircraftBrand = "Airbus", AircraftModel = "A350-900", SeatCapacity = 339 }
            };

            db.Aircraft.AddRange(aircraftList);

            db.SaveChanges();

            return Ok("Seeded Aircraft records.");
        }
    }
}