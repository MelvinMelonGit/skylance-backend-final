using Microsoft.AspNetCore.Mvc;
using skylance_backend.Services;
using System;
using System.Threading.Tasks;

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


        /// Triggers the Python service to predict for all un‐predicted bookings.
        /// Python handles feature extraction, inference, and write‐back.

        /// <returns>{ "updated": int }</returns>
        [HttpPost("all")]
        public async Task<IActionResult> PredictAll()
        {
            var result = await _mlService.CallBulkAsync();
            return Ok(new { updated = result.updated });
        }

        /// Triggers the Python service to predict for a single booking by ID.

        /// <param name="id">Booking ID</param>
        /// <returns>{ "bookingId": string, "prediction": int }</returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> PredictSingle(string id)
        {
            var result = await _mlService.CallSingleAsync(id);
            return Ok(new
            {
                bookingId = id,
                prediction = result.prediction
            });
        }
    }
}