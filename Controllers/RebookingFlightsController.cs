using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RebookingFlightsController : ControllerBase
    {
        private readonly SkylanceDbContext _context;

        public RebookingFlightsController(SkylanceDbContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDetail>>> GetFlightDetails()
        {
            return await _context.FlightDetails
                .Include(f => f.Aircraft)
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .ToListAsync();
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDetail>> GetFlightDetail(int id)
        {
            var flightDetail = await _context.FlightDetails
                .Include(f => f.Aircraft)
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flightDetail == null)
            {
                return NotFound();
            }

            return flightDetail;
        }

       


    }

}


