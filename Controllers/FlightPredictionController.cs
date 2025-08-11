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
    public class FlightPredictionController : ControllerBase
    {
        private readonly MLService _mlService;
        private readonly SkylanceDbContext _context;

        public FlightPredictionController(MLService mlService, SkylanceDbContext context)
        {
            _mlService = mlService;
            _context = context;
        }


        /// Triggers the Python service to predict for all un‐predicted flights.
        /// Python handles feature extraction, inference, and write‐back.

        /// <returns>{ "updated": int }</returns>
        [HttpPost("all")]
        public async Task<IActionResult> PredictFlights()
        {
            var result = await _mlService.CallBulkFlightAsync();
            return Ok(new { updated = result.updated });
        }

        /// Triggers the Python service to predict for a single flight by ID.

        /// <param name="id">Flight ID</param>
        /// <returns>{ "flightId": int, "probability": float }</returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> PredictFlight(int id)
        {
            var result = await _mlService.CallSingleFlightAsync(id);
            return Ok(new
            {
                flightId = id,
                probability = result.probability
            });
        }

        //Helper method
        //private string? GetLoggedInUserId()
        //{
        //    var empSession = HttpContext.Items["EmployeeSession"] as EmployeeSession;
        //    if (empSession == null)
        //        return null;

        //    return empSession.Employee.Id;
        //}

        /// Triggers the Skylance MySQL DB to get probability for a single flight by ID.

        /// <param name="id">Flight ID</param>
        /// <returns>{ "flightDetailId": int, "FlightNumber": str, "probability": float }</returns>

        //[ProtectedRoute]
        [HttpGet("show-percentage")]
        public async Task<IActionResult> GetAllShowPercentages()
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            //var loggedInUserId = GetLoggedInUserId();
            //if (loggedInUserId == null)
            //    return Unauthorized();

            var list = await _context.FlightDetails
                // Projection shortcut: project (.Select(...)) to a non-entity type,
                // EF Core automatically translates any navigation‐property access (e.g. fd.Aircraft.FlightNumber)
                // into a SQL join under the covers.
                // project only the two fields that need:
                .Select(fd => new
                {
                    flight = fd.Aircraft.FlightNumber,
                    predictedShowPercentage = fd.Probability ?? 0f
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = list
            });
        }

        //[ProtectedRoute]
        /// GET api/FlightPrediction/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlightDetail(int id)
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            //var loggedInUserId = GetLoggedInUserId();
            //if (loggedInUserId == null)
            //    return Unauthorized();

            var fd = await _context.FlightDetails
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fd == null) return NotFound();

            return Ok(new
            {
                flightDetailId = fd.Id,
                FlightNumber = fd.Aircraft.FlightNumber,
                probability = fd.Probability
            });
        }
    }
}