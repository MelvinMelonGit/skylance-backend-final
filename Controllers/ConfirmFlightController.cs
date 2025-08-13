using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Models;
using skylance_backend.Services;
using System;

namespace skylance_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmFlightController : ControllerBase
    {
        private readonly SkylanceDbContext _context;
        private readonly Random _random = new Random();
        private readonly ITripService _tripService;

        public ConfirmFlightController(ITripService tripService, SkylanceDbContext context)
        {
            _tripService = tripService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetAppUserDetail(string id)
        {
            var appUserDetail = await _context.AppUsers
                .Include(f => f.Nationality)
                .Include(f => f.MobileCode)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (appUserDetail == null)
            {
                return NotFound();
            }

            return appUserDetail;
        }

        [HttpPost("CheckinForRebooking")]
        public async Task<IActionResult> ProcessCheckInForRebooking([FromBody] CheckInRequestForRebooking request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var flightDetail = await _context.FlightDetails.FindAsync(request.FlightDetailId);
            if (flightDetail == null)
            {
                return BadRequest("Invalid flight detail ID");
            }

            try
            {
     
                var bookingDetail = new BookingDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    BookingReferenceNumber = Guid.NewGuid().ToString(),
                    AppUser = await _context.AppUsers.FindAsync(request.AppUserId)
                };
                await _context.BookingDetails.AddAsync(bookingDetail);

                double baggageAllowance = Math.Round(15 + _random.NextDouble() * 20, 1);
                Seat seat = new Seat
                {
                    SeatNumber = $"{_random.Next(1, 41)}{(char)('A' + _random.Next(0, 6))}",
                    FlightDetail = flightDetail
                };
                await _context.Seats.AddAsync(seat); 
                bool requireSpecialAssistance = _random.Next(0, 10) < 2; 
                int fareAmount = _random.Next(100, 2001); 
                string gate = _random.Next(1, 51).ToString(); 
                string terminal = _random.Next(1, 4).ToString(); 
                DateTime checkInTime = DateTime.UtcNow;
                DateTime boardingTime = checkInTime.AddMinutes(90); 

                
                var flightBookingDetail = new FlightBookingDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    FlightDetail = await _context.FlightDetails.FindAsync(request.FlightDetailId),
                    BookingDetail = bookingDetail,
                    BaggageAllowance = baggageAllowance,
                    SeatNumber = seat, 
                    RequireSpecialAssistance = requireSpecialAssistance,
                    BookingStatus = BookingStatus.CheckedIn,
                    Fareamount = fareAmount,
                };
                await _context.FlightBookingDetails.AddAsync(flightBookingDetail);

               
                var checkInDetail = new CheckInDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    AppUser = bookingDetail.AppUser,
                    FlightBookingDetail = flightBookingDetail,
                    CheckInTime = checkInTime,
                    BoardingTime = boardingTime,
                    SeatNumber = seat, 
                    Gate = gate,
                    Terminal = terminal
                };
                await _context.CheckInDetails.AddAsync(checkInDetail);

                if (!string.IsNullOrEmpty(request.OverbookingDetailId))
                {
                    var overbookingDetail = await _context.OverbookingDetails
                        .Include(od => od.OldFlightBookingDetail)
                        .FirstOrDefaultAsync(od => od.Id == request.OverbookingDetailId);

                    if (overbookingDetail != null)
                    {
                        if (overbookingDetail.OldFlightBookingDetail != null)
                        {
                            overbookingDetail.OldFlightBookingDetail.BookingStatus = BookingStatus.Rebooked;
                            _context.FlightBookingDetails.Update(overbookingDetail.OldFlightBookingDetail);
                        }
                        overbookingDetail.NewFlightBookingDetail = flightBookingDetail;
                        overbookingDetail.IsRebooking = true;
                        overbookingDetail.FinalCompensationAmount = request.FinalCompensationAmount;
                        _context.OverbookingDetails.Update(overbookingDetail);
                    }
                }
                flightDetail.CheckInCount+=1;
                _context.FlightDetails.Update(flightDetail);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    BookingId = bookingDetail.Id,
                    FlightBookingId = flightBookingDetail.Id,
                    CheckInId = checkInDetail.Id,
                    GeneratedValues = new
                    {
                        BaggageAllowance = baggageAllowance,
                        SeatNumber = seat,
                        RequireSpecialAssistance = requireSpecialAssistance,
                        FareAmount = fareAmount,
                        Gate = gate,
                        Terminal = terminal,
                        CheckInTime = checkInTime,
                        BoardingTime = boardingTime
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("NormalCheckIn")]
        public async Task<IActionResult> NormalCheckIn([FromBody] NormalCheckInRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var flightBookingDetail = await _context.FlightBookingDetails
                    .Include(fbd => fbd.FlightDetail)
                    .Include(fbd => fbd.BookingDetail)
                    .ThenInclude(bd => bd.AppUser)
                    .FirstOrDefaultAsync(fbd => fbd.Id == request.FlightBookingDetailId);

                if (flightBookingDetail == null)
                {
                    return BadRequest("Invalid flight booking detail ID.");
                }

                
                if (flightBookingDetail.BookingDetail.AppUser.Id != request.AppUserId)
                {
                    return BadRequest("The flight booking does not belong to the provided user.");
                }

                var flightDetail = flightBookingDetail.FlightDetail;
                var appUser = flightBookingDetail.BookingDetail.AppUser;

                
                Seat seat = new Seat
                {
                    SeatNumber = $"{_random.Next(1, 41)}{(char)('A' + _random.Next(0, 6))}",
                    FlightDetail = flightDetail
                };
                await _context.Seats.AddAsync(seat);

                
                flightBookingDetail.SeatNumber = seat;
                flightBookingDetail.BookingStatus = BookingStatus.CheckedIn;
                _context.FlightBookingDetails.Update(flightBookingDetail);

                
                DateTime checkInTime = DateTime.UtcNow;
                DateTime boardingTime = checkInTime.AddMinutes(90);
                string gate = _random.Next(1, 51).ToString();
                string terminal = _random.Next(1, 4).ToString();

                
                var checkInDetail = new CheckInDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    AppUser = appUser,
                    FlightBookingDetail = flightBookingDetail,
                    CheckInTime = checkInTime,
                    BoardingTime = boardingTime,
                    SeatNumber = seat,
                    Gate = gate,
                    Terminal = terminal
                };
                await _context.CheckInDetails.AddAsync(checkInDetail);

                
                flightDetail.CheckInCount += 1;
                _context.FlightDetails.Update(flightDetail);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    CheckInId = checkInDetail.Id,
                    SeatNumber = seat.SeatNumber,
                    Gate = gate,
                    Terminal = terminal,
                    CheckInTime = checkInTime,
                    BoardingTime = boardingTime
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





        /*
        
                [HttpGet("{flightBookingId}/boardingPass")]
                public async Task<IActionResult> GetBoardingPass(string checkInId)
                {
                    if (string.IsNullOrEmpty(checkInId))
                    {
                        return new JsonResult(new
                        {
                            status = "Invalid"
                        });
                    }

                    var boardingPass = await _tripService.GetBoardingPass(checkInId);

                    if (boardingPass == null)
                    {
                        return new JsonResult(new
                        {
                            status = "NotFound"
                        }); // Could also redirect to an error page
                    }

                    return new JsonResult(boardingPass);
                }
        */
    }


    public class CheckInRequestForRebooking
    {
        public required string AppUserId { get; set; }
        public required int FlightDetailId { get; set; }

     
        public string? OverbookingDetailId { get; set; }
        public double FinalCompensationAmount { get; set; }
    }

    public class NormalCheckInRequest
    {
        public required string AppUserId { get; set; }
        public required string FlightBookingDetailId { get; set; }
    }
}