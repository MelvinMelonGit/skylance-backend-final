using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Attributes;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerShowController : ControllerBase
    {
        private readonly SkylanceDbContext _db;
        public PassengerShowController(SkylanceDbContext db) => _db = db;

        //Helper method
        private string? GetLoggedInEmployeeId()
        {
            var empSession = HttpContext.Items["EmployeeSession"] as EmployeeSession;
            if (empSession == null)
                return null;

            return empSession.Employee.Id;
        }

        // GET api/PassengerShow?range=month&year=2025&month=8
        // GET api/PassengerShow?range=year&year=2025
        [ProtectedRoute]
        [HttpGet]
        public async Task<IActionResult> GetPassengerShow(
            [FromQuery] string range = "month",
            [FromQuery] int? year = null,
            [FromQuery] int? month = null,
            // weighted = SUM(CheckedInCount) / SUM(SeatsSold)
            // unweighted = individual (CheckedInCount) / (SeatsSold)
            [FromQuery] string method = "weighted"  // "weighted" | "unweighted"
        )
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            var loggedInEmployeeId = GetLoggedInEmployeeId();
            if (loggedInEmployeeId == null)
                return Unauthorized();

            // 1. show the selected year and month, if not
            // get the current datetime to decide which year or month to show
            var now = DateTime.UtcNow;
            var y = year ?? now.Year;
            var m = month ?? now.Month;

            DateTime start, end;
            if (range.Equals("year", StringComparison.OrdinalIgnoreCase))
            {
                start = new DateTime(y, 1, 1);
                end = start.AddYears(1);
            }
            else
            {
                start = new DateTime(y, m, 1);
                end = start.AddMonths(1);
            }

            var query = _db.FlightDetails
                .Where(fd => fd.DepartureTime >= start && fd.DepartureTime < end);

            double showPct;
            int flights;

            if (method.Equals("unweighted", StringComparison.OrdinalIgnoreCase))
            {
                var rows = await query
                    .Select(fd => new
                    {
                        Seats = (int?)fd.SeatsSold ?? 0,
                        Checkins = (int?)fd.CheckInCount ?? 0
                    })
                    .ToListAsync();

                flights = rows.Count;
                var ratios = rows.Where(r => r.Seats > 0)
                                 .Select(r => (double)r.Checkins / r.Seats);
                var avg = ratios.Any() ? ratios.Average() : 0.0;
                // avg * 100 convert pct into int to floating error
                showPct = Math.Round(avg * 100.0, 2);
            }
            else
            {
                var agg = await query
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        SumSeats = g.Sum(x => (int?)x.SeatsSold) ?? 0,
                        SumCheckins = g.Sum(x => (int?)x.CheckInCount) ?? 0,
                        Flights = g.Count()
                    })
                    .FirstOrDefaultAsync();

                flights = agg?.Flights ?? 0;
                showPct = (agg != null && agg.SumSeats > 0)
                    // * 100 convert pct into int to floating error
                    ? Math.Round(100.0 * (double)agg.SumCheckins / agg.SumSeats, 2)
                    : 0.0;
            }

            // return actual showPercentage and noShowPercentage to gauge show
            var result = new
            {
                // check calculation whether is for 'month' or 'year'
                range = range.ToLower(),
                year = y,
                month = range.Equals("month", StringComparison.OrdinalIgnoreCase) ? m : (int?)null,
                // 'weighted' or 'unweighted' method
                method = method.ToLower(),
                // total filght numbers
                flights,
                // actual show and no show percentage
                showPercentage = showPct,
                noShowPercentage = Math.Round(100.0 - showPct, 2)
            };


            return Ok(new
            {
                success = true,
                data = new[] { result }
            });
        }
    }
}