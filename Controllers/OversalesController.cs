using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Services;
using System;
using System.Threading.Tasks;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OversalesController : ControllerBase
    {
        private readonly MLService _mlService;
        private readonly SkylanceDbContext _context;

        public OversalesController(MLService mlService, SkylanceDbContext context)
        {
            _mlService = mlService;
            _context = context;
        }


        /// <summary>
        /// Returns a compact list of flights for the dropdown.
        /// No date filter — suitable for newly created flights too.
        /// </summary>
        /// GET /api/oversales/available-flights
        // GET: /api/Oversales/available-flights
        [HttpGet("available-flights")]
        public async Task<IActionResult> AvailableFlights()
        {
            var list = await _context.FlightDetails
                .Where(f => f.Aircraft != null
                            && f.Aircraft.SeatCapacity > 0
                            && f.Aircraft.FlightNumber != null
                            && f.Aircraft.FlightNumber != "")
                .OrderByDescending(f => f.Id)
                .Take(50)
                .Select(f => new AvailableFlightDto(
                    f.Id,
                    f.Aircraft.FlightNumber,
                    f.Aircraft.SeatCapacity))
                .ToListAsync();

            return Ok(new { success = true, data = list });
        }

        // POST: /api/Oversales/calculate
        [HttpPost("calculate")]
        public async Task<ActionResult<OversalesResponse>> Calculate([FromBody] OversalesRequest req)
        {
            var flight = await _context.FlightDetails
                .Where(f => f.Id == req.FlightId)
                .Select(f => new
                {
                    f.Id,
                    Prob = (double?)f.Probability,            // may be null
                    Code = f.Aircraft.FlightNumber,
                    Capacity = f.Aircraft.SeatCapacity
                })
                .SingleOrDefaultAsync();

            if (flight == null)
                return NotFound($"Flight {req.FlightId} not found.");

            if (string.IsNullOrWhiteSpace(flight.Code) || flight.Capacity <= 0)
                return BadRequest("Flight has no valid aircraft/code or seat capacity.");

            if (flight.Prob is null)
                return Conflict($"Flight {flight.Code} has no stored probability. Please run prediction first.");

            // default safety factor is 80% if 0 or out of range
            var safety = (req.SafetyFactor is null || req.SafetyFactor <= 0 || req.SafetyFactor > 1)
                ? 0.8
                : req.SafetyFactor.Value;
            // DB may store 0..1 or 0..100; normalize to percent
            var showPct = flight.Prob <= 1.0 ? flight.Prob.Value * 100.0 : flight.Prob.Value;

            // Recommend extra tickets = capacity × (1 − show%) × safety
            var recommend = Math.Max(0, (int)Math.Round(
                flight.Capacity * (1.0 - (showPct / 100.0)) * safety));

            var rationale =
                $"Used stored show probability {showPct:F2}% for {flight.Code} " +
                $"(capacity {flight.Capacity}), safety factor {safety:0.##}. " +
                $"Recommend oversale {recommend} tickets. ";
                

            return Ok(new OversalesResponse(
                flight.Id,
                showPct,
                recommend,
                rationale));
        }
    }
}