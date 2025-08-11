using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Models;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace skylance_backend.Services
{
    public class TripService : ITripService
    {
        private readonly SkylanceDbContext _context;

        public TripService(SkylanceDbContext context)
        {
            _context = context;
        }

        public async Task<TripDetailDTO> GetTripDetailsAsync(string flightBookingId)
        {
            var flightBooking = await _context.FlightBookingDetails
                .Include(fb => fb.FlightDetail)
                .FirstOrDefaultAsync(fb => fb.Id == flightBookingId);

            if (flightBooking == null) return null;

            var flight = flightBooking.FlightDetail;

            return new TripDetailDTO
            {
                BookingReferenceNumber = flightBooking.BookingDetail.BookingReferenceNumber,
                Airline = flight.Aircraft.Airline,
                AircraftModel = flight.Aircraft.AircraftModel,
                FlightNumber = flight.Aircraft.FlightNumber,
                OriginAirportCode = flight.OriginAirport.IataCode,
                OriginAirportName = flight.OriginAirport.Name,
                DestinationAirportCode = flight.DestinationAirport.IataCode,
                DestinationAirportName = flight.DestinationAirport.Name,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                FlightDuration = flight.ArrivalTime - flight.DepartureTime,
                HasCheckedIn = flightBooking.BookingStatus == BookingStatus.CheckedIn
            };
        }
        // Check if seat has been pre-selected
        // If not, then assign seat
        public async Task<CheckInValidationResult> ValidateCheckInAsync(string flightBookingId)
        {
            var flightBooking = await _context.FlightBookingDetails
                .Include(fb => fb.FlightDetail)
                .FirstOrDefaultAsync(fb => fb.Id == flightBookingId);

            if (flightBooking == null)
                throw new Exception("Booking not found");

            if (flightBooking.BookingStatus == BookingStatus.CheckedIn)
                return CheckInValidationResult.AlreadyCheckedIn;

            var flight = flightBooking.FlightDetail;

            if (flight.DepartureTime <= DateTime.UtcNow)
                return CheckInValidationResult.FlightDeparted;

            // var checkedInCount = await _context.FlightBookingDetails
            //    .Where(fb => fb.FlightDetail.Id == flight.Id && fb.BookingStatus == BookingStatus.CheckedIn)
            //    .CountAsync();

            var checkedInCount = flight.CheckInCount;
            var seatCapacity = flight.Aircraft.SeatCapacity;
            var overbookingCount = flight.OverbookingCount;

            if (checkedInCount >= seatCapacity)
            {
                flight.OverbookingCount++;
                await _context.SaveChangesAsync();

                return CheckInValidationResult.FlightFullyCheckedIn;
            }

            return CheckInValidationResult.Allowed;
        }
/*
        public async Task<bool> ConfirmCheckInAsync(string flightBookingId)
        {
            var flightBooking = await _context.FlightBookingDetails
                .Include(fb => fb.FlightDetail)
                .FirstOrDefaultAsync(fb => fb.Id == flightBookingId);

            if (flightBooking == null)
                return false;

            if (flightBooking.FlightDetail.DepartureTime <= DateTime.UtcNow)
                return false;

            // Check seat availability
            if (flightBooking.SeatNumber == null)
            {
                // Get all unassigned seats for this flight
                var availableSeats = await _context.Seats
                    .Where(s => s.FlightDetail.Id == flightBooking.FlightDetail.Id && !s.IsAssigned)
                    .ToListAsync();

                if (!availableSeats.Any())
                    return false; // no seat available, cannot check in

                // Pick one at random
                var random = new Random();
                var selectedSeat = availableSeats[random.Next(availableSeats.Count)];

                // Assign to passenger
                flightBooking.SeatNumber = selectedSeat;
                selectedSeat.IsAssigned = true; // mark seat as taken
            }

      
            var checkedInCount = flightBooking.FlightDetail.CheckInCount;

            flightBooking.BookingStatus = BookingStatus.CheckedIn;
            checkedInCount++;
            await _context.SaveChangesAsync();

            return true;
        }
*/
        public async Task<BoardingPassDTO> GetBoardingPass(string checkInId)
        {
            var boardingPass = await _context.CheckInDetails
                .Include(bp => bp.FlightBookingDetail)
                .FirstOrDefaultAsync(bp => bp.Id == checkInId);

            if (boardingPass == null) return null;

            var flight = boardingPass.FlightBookingDetail.FlightDetail;

            return new BoardingPassDTO
            {
                Airline = flight.Aircraft.Airline,
                FlightNumber = flight.Aircraft.FlightNumber,
                OriginAirportCode = flight.OriginAirport.IataCode,
                OriginAirportName = flight.OriginAirport.Name,
                DestinationAirportCode = flight.DestinationAirport.IataCode,
                DestinationAirportName = flight.DestinationAirport.Name,
                BoardingTime = boardingPass.CheckInTime,
                Seat = boardingPass.SeatNumber,
                Gate = boardingPass.Gate,
                Terminal = boardingPass.Terminal
            };
        }
    }
}