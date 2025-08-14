using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Attributes;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class dashboardController : ControllerBase
    {
        private readonly SkylanceDbContext db;

        public dashboardController(SkylanceDbContext db)
        {
            this.db = db;
        }

        //Helper method
        private string? GetLoggedInEmployeeId()
        {
            var empSession = HttpContext.Items["EmployeeSession"] as EmployeeSession;
            if (empSession == null)
                return null;

            return empSession.Employee.Id;
        }

        [ProtectedRoute]
        [HttpGet("summary")]
        public IActionResult Summary()
        {
            // assign the logged-in EmployeeId with the helper method (EmployeeSession from AuthMiddleware)
            var loggedInEmployeeId = GetLoggedInEmployeeId();
            if (loggedInEmployeeId == null)
                return Unauthorized();

            try
            {
                // active flights now
                //var startTime = new DateTime(2025, 8, 11, 14, 0, 0);
                //var endTime = new DateTime(2025, 8, 11, 15, 0, 0);
                var activeFlightsNow = db.FlightDetails
                    .Count(f => f.DepartureTime <= DateTime.Now && f.ArrivalTime >= DateTime.Now);

                // active flights yesterday as of now
                var activeFlightsYesterdayNow = db.FlightDetails
                    .Count(f => f.DepartureTime <= DateTime.Now.AddDays(-1) && f.ArrivalTime >= DateTime.Now.AddDays(-1));

                // active flights - percentage change 
                double trendActiveFlights;
                if (activeFlightsYesterdayNow == 0)
                {
                    trendActiveFlights = activeFlightsNow > 0 ? 100 : 0;
                }
                else
                {
                    trendActiveFlights = ((double)(activeFlightsNow - activeFlightsYesterdayNow) / activeFlightsYesterdayNow) * 100;
                }

                // overbooked flights today
                var overbookedFlightsToday = db.FlightDetails
                    .Include(f => f.Aircraft)
                    .Where(f => f.DepartureTime.Date == DateTime.Today &&
                    f.OverbookingCount > 0)
                    .Count();

                // overbooked flights yesterday
                var overbookedFlightsYesterday = db.FlightDetails
                    .Include(f => f.Aircraft)
                    .Where(f => f.DepartureTime.Date == DateTime.Today.AddDays(-1) &&
                    f.OverbookingCount > 0)
                    .Count();

                // overbooked flights - percentage change
                double trendOverbookedFlights;
                if (overbookedFlightsYesterday == 0)
                {
                    trendOverbookedFlights = overbookedFlightsToday > 0 ? 100 : 0;
                }
                else
                {
                    trendOverbookedFlights = ((double)(overbookedFlightsToday - overbookedFlightsYesterday) / overbookedFlightsYesterday) * 100;
                }

                // total passengers today
                var totalPassengersToday = db.FlightDetails
                    .Where(f => f.DepartureTime.Date == DateTime.Today)
                    .Sum(f => f.CheckInCount);

                // total passengers yesterday
                var totalPassengersYesterday = db.FlightDetails
                    .Where(f => f.DepartureTime.Date == DateTime.Today.AddDays(-1))
                    .Sum(f => f.CheckInCount);

                // total passengers - percentage change
                double trendTotalPassengers;
                if (totalPassengersYesterday == 0)
                {
                    trendTotalPassengers = totalPassengersToday > 0 ? 100 : 0;
                }
                else
                {
                    trendTotalPassengers = ((double)(totalPassengersToday - totalPassengersYesterday) / totalPassengersYesterday) * 100;
                }

                // revenue today
                var revenueToday = db.FlightBookingDetails
                    .Include(fbd => fbd.FlightDetail)
                    .Where(fbd => fbd.FlightDetail.DepartureTime.Date == DateTime.Today)
                    .Sum(fbd => fbd.Fareamount);

                // revenue yesterday
                var revenueYesterday = db.FlightBookingDetails
                    .Include(fbd => fbd.FlightDetail)
                    .Where(fbd => fbd.FlightDetail.DepartureTime.Date == DateTime.Today.AddDays(-1))
                    .Sum(fbd => fbd.Fareamount);

                // revenue - percentage change
                double trendRevenue;
                if (revenueYesterday == 0)
                {
                    trendRevenue = revenueToday > 0 ? 100 : 0;
                }
                else
                {
                    trendRevenue = ((double)(revenueToday - revenueYesterday) / revenueYesterday) * 100;
                }

                var result = new
                {
                    status = "success",
                    data = new
                    {
                        activeFlights = new { valueToday = activeFlightsNow, percentChange = Math.Round(trendActiveFlights,1).ToString("F1") + "%" },
                        overbookedFlights = new { valueToday = overbookedFlightsToday, percentChange = Math.Round(trendOverbookedFlights,1).ToString("F1") + "%"},                        
                        totalPassengers = new { valueToday = totalPassengersToday, percentChange = Math.Round(trendTotalPassengers,1).ToString("F1") + "%"},
                        revenueToday = new { valueToday = revenueToday, percentChange = Math.Round(trendRevenue,1).ToString("F1") + "%"}
                    }
                };

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new
                {
                    status = "error",
                    errorCode = 500,
                    message = "Internal server error"
                });
            }
        }
    }
}