using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Services;

namespace skylance_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TripController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly SkylanceDbContext _context;

    public TripController(SkylanceDbContext context, ITripService tripService)
    {
        _tripService = tripService;
        _context = context;
    }

    // GET: api/trips/{flightDetailsId}
    // to display the flight detail page
    //[ProtectedRoute]
    [HttpGet("{flightBookingId}")]
    public async Task<IActionResult> TripDetail(string flightBookingId)
    {
        if (string.IsNullOrEmpty(flightBookingId))
        {
            return BadRequest("Invalid booking ID.");
        }

        var tripDetails = await _tripService.GetTripDetailsAsync(flightBookingId);

        if (tripDetails == null)
        {
            return NotFound(); // Could also redirect to an error page
        }

        return Ok(tripDetails); // This will pass the DTO to the Razor view
    }


    // GET: api/trips/{flightDetailsId}/checkin/validate
    // to verify if flight is overbooked or not
    // if it is not, then display confirm check in page
    // if it is, then redirect
    //[ProtectedRoute]
    [HttpGet("{flightBookingId}/checkin/validate")]
    public async Task<IActionResult> ValidateCheckIn(string flightBookingId)
    {
        var result = await _tripService.ValidateCheckInAsync(flightBookingId);

        return result switch
        {
            CheckInValidationResult.Allowed => new JsonResult(new
            {
                status = "Allowed"
            }),
            CheckInValidationResult.AlreadyCheckedIn => new JsonResult(new
            {
                status = "AlreadyCheckedIn"
            }),
            CheckInValidationResult.FlightDeparted => new JsonResult(new
            {
                status = "FlightDeparted"
            }),
            CheckInValidationResult.FlightFullyCheckedIn => Redirect($"/api/Overbooking/overbooking?flightBookingDetailId={flightBookingId}"),
            _ => new JsonResult(new
            {
                status = "Error",
                message = "An error has occured."
            }),
        };
    }
  /*  
    //[ProtectedRoute]
    [HttpPost("{flightBookingId}/checkin/confirm")]
    public async Task<IActionResult> ConfirmCheckIn(string flightBookingId)
    {
        var result = await _tripService.ConfirmCheckInAsync(flightBookingId);
        return result ? Ok("Checked in successfully.") : BadRequest("Check-in failed.");
    }
  */
}