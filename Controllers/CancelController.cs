using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancelFlightController : Controller
    {
        private readonly SkylanceDbContext _db;

        public CancelFlightController(SkylanceDbContext db)
        {
            _db = db;
        }
        [HttpGet("cancelconfirmation")]
        public IActionResult GetCancelFlight(string flightBookingDetailId)
        {
            var flightBookingDetail = _db.FlightBookingDetails
                .Include(b => b.FlightDetail)
                    .ThenInclude(f => f.Aircraft)
                .Include(b => b.FlightDetail)
                    .ThenInclude(f => f.OriginAirport)
                          .ThenInclude(o => o.City)
                              .ThenInclude(c => c.Country)
                .Include(b => b.FlightDetail)
                    .ThenInclude(f => f.DestinationAirport)
                          .ThenInclude(o => o.City)
                              .ThenInclude(c => c.Country)
                .Include(b => b.BookingDetail)
                    .ThenInclude(x => x.AppUser)
                .FirstOrDefault(b => b.Id == flightBookingDetailId);

            if (flightBookingDetail == null)
            {
                return NotFound();
            }
            var overbooking = _db.OverbookingDetails
                .FirstOrDefault(o => o.OldFlightBookingDetailId == flightBookingDetail.Id);
            var notification = _db.Notifications
                .FirstOrDefault(n => n.OverbookingDetail == overbooking);
            var flightDuringTime = flightBookingDetail.FlightDetail.ArrivalTime - flightBookingDetail.FlightDetail.DepartureTime;

            return Ok(new
            {
                flightBookingDetailId = flightBookingDetail.FlightDetail.Id,
                OriginAirport = flightBookingDetail.FlightDetail.OriginAirport.IataCode,
                DestinationAirport = flightBookingDetail.FlightDetail.DestinationAirport.IataCode,
                FlightDate = flightBookingDetail.FlightDetail.DepartureTime.ToString("yyyy-MM-dd"),
                FlightDuringTime = flightDuringTime,
                Aircraft = flightBookingDetail.FlightDetail.Aircraft.Airline,
                Compensation = overbooking?.FinalCompensationAmount
            });
        }
        [HttpPost("excutecancel")]
        public IActionResult ExecuteFlightCancellation(string flightBookingDetailId)
        {
            var flightBookingDetail = _db.FlightBookingDetails
            .Include(f => f.BookingDetail)
                .ThenInclude(b => b.AppUser)
            .Include(f => f.FlightDetail)
            .FirstOrDefault(f => f.Id == flightBookingDetailId);
            var overbooking = _db.OverbookingDetails
               .FirstOrDefault(o => o.OldFlightBookingDetailId == flightBookingDetailId);
            var notification = _db.Notifications
               .FirstOrDefault(n => n.OverbookingDetail == overbooking);
            if (flightBookingDetail == null)
            {
                return NotFound();
            }
            flightBookingDetail.BookingStatus = BookingStatus.Cancelled;
            _db.SaveChanges();

            return Ok(new
            {
                Status = "Success",
                Message = "You have successfully cancelled this flight",

            });
        }
    }
}