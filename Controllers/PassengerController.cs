using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Services;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class PassengerController : ControllerBase
    {
        private readonly SkylanceDbContext _context;
        private readonly MLService _mlService;

        public PassengerController(SkylanceDbContext context)
        {
            _context = context;
        }

        [HttpGet("allpassengers")]
        public async Task<IActionResult> GetAllPassengers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = await _context.FlightBookingDetails.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = await _context.FlightBookingDetails
                .OrderBy(f => f.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new PassengerDetailsDTO
                {
                    PassengerId = f.BookingDetail.AppUser.Id,
                    FlightNumber = f.FlightDetail.Aircraft.FlightNumber,
                    Airline = f.FlightDetail.Aircraft.Airline,
                    PassengerName = (f.BookingDetail.AppUser.FirstName) + f.BookingDetail.AppUser.LastName,
                    Class = f.Class.ToString(),
                    MembershipTier = f.BookingDetail.AppUser.MembershipTier,
                    DateOfTravel = f.FlightDetail.DepartureTime,
                    BookingStatus = f.BookingStatus.ToString(),
                    BaggageAllowance = f.BaggageAllowance.ToString(),
                    BaggageChecked = f.BaggageChecked.ToString(),
                    SeatNumber = f.SeatNumber != null ? f.SeatNumber.SeatNumber : null,
                    CheckinStatus = f.BookingStatus == BookingStatus.CheckedIn
                    ? "Checked-In"
                    : f.BookingStatus == BookingStatus.Confirmed
                    ? "Not Checked-In"
                    : "Not Applicable",
                    BookingReferenceNumber = f.BookingDetail.BookingReferenceNumber,
                    Email = f.BookingDetail.AppUser.Email,
                    PhoneNumber = f.BookingDetail.AppUser.MobileNumber,
                    PassportNumber = f.BookingDetail.AppUser.PassportNumber,
                    DepartureCity = f.FlightDetail.OriginAirport.City.Name,
                    ArrivalCity = f.FlightDetail.DestinationAirport.City.Name,
                    SpecialRequests = f.SpecialRequest.ToString()
                })
                .ToListAsync();

            var result = new
            {
                success = true,
                message = "Passenger details fetched successfully.",
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalRecords = totalRecords,
                    totalPages = totalPages
                },
                data = data
            };

            return Ok(result);
        }

        [HttpGet("{flightNumber}/passengers")]
        public async Task<IActionResult> GetPassengersByFlightId(string flightNumber, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;

            var flight = _context.FlightBookingDetails
            .Include(f => f.BookingDetail)
                .ThenInclude(b => b.AppUser)
            .Include(f => f.FlightDetail)
                .ThenInclude(fd => fd.Aircraft)
            .Where(f => f.FlightDetail.Aircraft.FlightNumber == flightNumber && f.BookingDetail != null);

            var totalPassengers = await flight.CountAsync();

            var pagedPassengers = await flight
                .OrderBy(f => f.SeatNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var passengers = new List<PassengerDetailsDTO>();
            foreach (var f in pagedPassengers)
            {
                var checkin = await _context.CheckInDetails
                            .FirstOrDefaultAsync(c => c.FlightBookingDetail.Id == f.Id);

                passengers.Add(new PassengerDetailsDTO
                {
                    PassengerName = f.BookingDetail.AppUser.FirstName + " " + f.BookingDetail.AppUser.LastName,
                    SeatNumber = f.SeatNumber != null ? f.SeatNumber.SeatNumber : null,
                    CheckinStatus = f.BookingStatus == BookingStatus.CheckedIn
                                ? "Checked-in"
                                : f.BookingStatus == BookingStatus.Confirmed
                                    ? "Not Checked-in"
                                    : "No Show",
                    MembershipTier = f.BookingDetail?.AppUser?.MembershipTier ?? "",
                    BookingDate = f.BookingDate,
                    CheckinTime = checkin?.CheckInTime,
                    Prediction = f.Prediction?.ToString() ?? "No Prediction",
                    SpecialRequests = f.SpecialRequest?.ToString() ?? ""
                });
            }

            var response = new
            {
                status = "success",
                flightId = flightNumber,
                page = page,
                pageSize = pageSize,
                totalPassengers = totalPassengers,
                passengers = passengers
            };

            return Ok(response);
        }
    }
}