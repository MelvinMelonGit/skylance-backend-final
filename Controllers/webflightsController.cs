using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Models;
using System;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/webflights")]
    public class Web_FlightsController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        private readonly Random _random = new Random();
        public Web_FlightsController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpGet("allflights")]
        public IActionResult GetAllFlights(int page = 1, int pageSize = 4)
        {
            int totalFlights = db.FlightDetails
                //.Where(f => f.DepartureTime > DateTime.Now)   
                .Count();

            var flights = db.FlightDetails
                .Include(f => f.Aircraft)
                .Include(f => f.OriginAirport)
                    .ThenInclude(a => a.City)
                .Include(f => f.DestinationAirport)
                    .ThenInclude(a => a.City)                
                .OrderBy(f => f.DepartureTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new Web_FlightsDTO
                {
                    AirlineName = f.Aircraft.Airline,
                    FlightName = f.Aircraft.FlightNumber,
                    DepartureDate = f.DepartureTime.ToString("s"),
                    ArrivalTime = f.ArrivalTime.ToString("s"),
                    DepartureCity = f.OriginAirport.City.Name,
                    ArrivalCity = f.DestinationAirport.City.Name,
                    DepartureCityCode = f.OriginAirport.IataCode,
                    ArrivalCityCode = f.DestinationAirport.IataCode,
                    TotalNoOfPassengers = f.Aircraft.SeatCapacity,
                    TotalNoOfCrew = f.NumberOfCrew,
                    FlightStatus = f.FlightStatus
                })
                .ToList();

            var result = new
            {
                status = "success",
                page,
                pageSize,
                totalFlights,
                flights
            };

            return Ok(result);
        }

    
        [HttpGet("open-for-checkin")]
        public async Task<ActionResult<object>> GetFlightDetails(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 3)
        {
            var query = db.FlightDetails
                .Include(f => f.Aircraft)
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Where(f => f.CheckInCount < f.Aircraft.SeatCapacity);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            var flights = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            string[] gateLetters = new[] { "A", "B", "C", "D" };

            return new
            {
                Status = "success",
                Message = "Flights open for check-in retrieved successfully.",
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalItems = totalItems
                },
                Data = flights.Select(f => new
                {
                    flightid = f.Aircraft.FlightNumber,
                    route = $"{f.OriginAirport.IataCode} → {f.DestinationAirport.IataCode}",
                    departure = f.DepartureTime.ToString("HH:mm"),
                    capacity = f.Aircraft.SeatCapacity,
                    booked = f.SeatsSold,
                    checkedIn = f.CheckInCount,
                    status = f.FlightStatus,
                    gate = $"{gateLetters[_random.Next(0, 4)]}{_random.Next(1, 51)}",
                    aircraft = f.Aircraft.AircraftModel,
                    date = f.DepartureTime.ToString("yyyy-MM-dd")
                })
            };
        }

    }



}
