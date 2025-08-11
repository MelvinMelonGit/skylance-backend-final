using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Models;

namespace skylance_backend.Services
{
    public interface ITripService
    {
        Task<TripDetailDTO> GetTripDetailsAsync(string flightBookingId);
        Task<CheckInValidationResult> ValidateCheckInAsync(string flightBookingId);
        // Task<bool> ConfirmCheckInAsync(string flightBookingId);
        Task<BoardingPassDTO> GetBoardingPass(string checkInId);
    }
}