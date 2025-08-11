using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Enum;
using Microsoft.AspNetCore.Http.HttpResults;

namespace skylance_backend.Services
{
    public class RevenueService
    {
        private readonly SkylanceDbContext _db;

        public RevenueService(SkylanceDbContext db)
        {
            _db = db;
        }

        public async Task CalculateAndStoreRevenue()
        {

            _db.AirlineRevenue.RemoveRange(_db.AirlineRevenue);

            var flightbookings = await _db.FlightBookingDetails
           .Include(b => b.FlightDetail)
               .ThenInclude(f => f.Aircraft)
           .Where(b => b.BookingStatus == BookingStatus.Confirmed)
           .Select(b => new {
               b.Id,
               b.Fareamount,
               FlightDetail = new
               {
                   b.FlightDetail.DepartureTime,
                   Aircraft = new
                   {
                       b.FlightDetail.Aircraft.Airline
                   }
               }
           })
           .AsNoTracking()
           .ToListAsync();


            var compensations = await _db.OverbookingDetails
            .Select(o => new { o.OldFlightBookingDetailId, o.FinalCompensationAmount })
            .AsNoTracking()
            .ToListAsync();

            var monthlyData = flightbookings
    .GroupBy(b => new
    {
        AirlineCode = b.FlightDetail.Aircraft.Airline[..2],
        AirlineName = b.FlightDetail.Aircraft.Airline,
        Year = b.FlightDetail.DepartureTime.Year,
        Month = b.FlightDetail.DepartureTime.Month
    })
    .Select(g => {
        var totalCompensation = compensations
            .Where(c => g.Select(b => b.Id).Contains(c.OldFlightBookingDetailId))
            .Sum(c => c.FinalCompensationAmount);

            return new AirlineRevenue
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:00}",
                        PeriodType = "month",
                        AirlineCode = g.Key.AirlineCode,
                        AirlineName = g.Key.AirlineName,
                        TicketsSold = g.Count(),
                        Revenue = (decimal)(g.Sum(b => b.Fareamount) - totalCompensation)
                    };
                })
                .ToList();

            var yearlyData = flightbookings
            .GroupBy(b => new
            {
                AirlineCode = b.FlightDetail.Aircraft.Airline[..2],
                AirlineName = b.FlightDetail.Aircraft.Airline,
                Year = b.FlightDetail.DepartureTime.Year
            })
            .Select(g => {
                var totalCompensation = compensations
                    .Where(c => g.Select(b => b.Id).Contains(c.OldFlightBookingDetailId))
                    .Sum(c => c.FinalCompensationAmount);

                return new AirlineRevenue
                {
                    Period = g.Key.Year.ToString(),
                    PeriodType = "year",
                    AirlineCode = g.Key.AirlineCode,
                    AirlineName = g.Key.AirlineName,
                    TicketsSold = g.Count(),
                    Revenue = (decimal)(g.Sum(b => b.Fareamount) - totalCompensation)
                };
            })
            .ToList();

            if (monthlyData.Count == 0 && yearlyData.Count == 0)
            {
                Console.WriteLine("NOTFOUND") ; // No data to store
            }
            await _db.AirlineRevenue.AddRangeAsync(monthlyData);
            await _db.AirlineRevenue.AddRangeAsync(yearlyData);
            await _db.SaveChangesAsync();
        }
    }
}