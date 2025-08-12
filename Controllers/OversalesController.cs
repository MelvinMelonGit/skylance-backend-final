using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
// using skylance_backend.Services; if need ML prediction in future
using System;
using System.Linq;
using System.Threading.Tasks;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OversalesController : ControllerBase
    {
        private readonly SkylanceDbContext _context;

        public OversalesController(SkylanceDbContext context)
        {
            _context = context;
        }

        /// For dropdowns
        /// GET: /api/oversales/available-flights

        [HttpGet("available-flights")]
        public async Task<IActionResult> AvailableFlights()
        {
            var list = await _context.FlightDetails
                .AsNoTracking()
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

        /// Calculate oversales for a (possibly new) flight using the historical average
        /// show probability of the given FlightNumber.

        /// POST: /api/oversales/calculate
        [HttpPost("calculate")]
        public async Task<ActionResult<OversalesResponse>> Calculate([FromBody] OversalesRequest req)
        {
            if (req is null) return BadRequest("Request body is required.");
            if (string.IsNullOrWhiteSpace(req.FlightNumber))
                return BadRequest("FlightNumber is required.");

            var code = req.FlightNumber.Trim();

            // 1. Find historical flights with same code that already have a probability
            var hist = _context.FlightDetails
                .AsNoTracking()
                .Where(f => f.Aircraft != null
                            && f.Aircraft.FlightNumber == code
                            && f.Probability != null);

            var count = await hist.CountAsync();
            if (count == 0)
                return Conflict($"No historical probabilities found for flight number {code}. Run predictions first.");

            // 2. Average show prob (normalize each row from 0..1 or 0..100 to 0..1)
            var avgShow01 = await hist
                .Select(f => f.Probability!.Value <= 1.0
                    ? f.Probability!.Value
                    : f.Probability!.Value / 100.0)
                .AverageAsync();

            // 3. Resolve capacity: prefer request value, else use most recent seat capacity for this code
            int? capacity = req.Capacity;
            if (capacity is null || capacity <= 0)
            {
                capacity = await _context.FlightDetails
                    .AsNoTracking()
                    .Where(f => f.Aircraft != null
                                && f.Aircraft.FlightNumber == code
                                && f.Aircraft.SeatCapacity > 0)
                    .OrderByDescending(f => f.Id)
                    .Select(f => (int?)f.Aircraft.SeatCapacity)
                    .FirstOrDefaultAsync();
            }

            if (capacity is null || capacity <= 0)
                return BadRequest($"No valid seat capacity found/provided for {code}.");

            // 4. Safety factor (default 0.8 if null/out of range)
            var safety = (req.SafetyFactor is null || req.SafetyFactor <= 0 || req.SafetyFactor > 1)
                ? 0.8
                : req.SafetyFactor.Value;

            // 5. Compute recommendation
            var showPct = Math.Clamp(avgShow01 * 100.0, 0, 100);
            var recommend = Math.Max(0, (int)Math.Round(capacity.Value * (1.0 - avgShow01) * safety));

            var rationale =
                $"Used historical average show probability {showPct:F2}% for {code} " +
                $"from {count} past flight(s), capacity {capacity}, safety factor {safety:0.##}. " +
                $"Recommend oversale {recommend} tickets.";

            var resp = new OversalesResponse(
                FlightNumber: code,
                ShowPercentage: showPct,
                RecommendOversale: recommend,
                Rationale: rationale
            );

            return Ok(resp);
        }
    }
}