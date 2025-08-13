using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Services;
using skylance_backend.Attributes;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly MLService _mlService;

        public PredictionController(MLService mlService)
        {
            _mlService = mlService;
        }

        //Helper method
        private string? GetLoggedInEmployeeId()
        {
            var empSession = HttpContext.Items["EmployeeSession"] as EmployeeSession;
            if (empSession == null)
                return null;

            return empSession.Employee.Id;
        }


        /// Triggers the Python service to predict for all un‐predicted bookings.
        /// Python handles feature extraction, inference, and write‐back.

        /// <returns>{ "updated": int }</returns>
        [ProtectedRoute]
        [HttpPost("all")]
        public async Task<IActionResult> PredictAll()
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            var loggedInEmployeeId = GetLoggedInEmployeeId();
            if (loggedInEmployeeId == null)
                return Unauthorized();

            var result = await _mlService.CallBulkAsync();
            return Ok(new { updated = result.updated });
        }

        /// Triggers the Python service to predict for a single booking by ID.

        /// <param name="id">Booking ID</param>
        /// <returns>{ "bookingId": string, "prediction": int }</returns>
        [ProtectedRoute]
        [HttpPost("{id}")]
        public async Task<IActionResult> PredictSingle(string id)
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            var loggedInEmployeeId = GetLoggedInEmployeeId();
            if (loggedInEmployeeId == null)
                return Unauthorized();

            var result = await _mlService.CallSingleAsync(id);
            return Ok(new
            {
                bookingId = id,
                prediction = result.prediction
            });
        }
    }
}