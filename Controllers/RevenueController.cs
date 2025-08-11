using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;
using skylance_backend.Services;
using System.Globalization;
namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevenueController : Controller
    {
        private readonly SkylanceDbContext _db;
        private readonly RevenueService _revenueService;

        public RevenueController(SkylanceDbContext db, RevenueService revenueService)
        {
            _db = db;
            _revenueService = revenueService;
        }

        [HttpGet]
        [RequestTimeout(30)]
        public async Task<IActionResult> GetRevenue(string periodType)
        {

            await _revenueService.CalculateAndStoreRevenue();

            if (periodType != "month" && periodType != "year")
            {
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Invalid periodType. Allowed values: 'month' or 'year'"
                });
            }

            var revenueData = periodType == "month"? await GetMonthlyRevenueData(): await GetYearlyRevenueData();

            return Ok(new
            {
                success = true,
                periodType = periodType,
                data = revenueData
            });

            

        }

        private async Task<object> GetMonthlyRevenueData()
        {
            DateTime today = DateTime.Today; 
            DateTime startDate = today.AddMonths(-5);
            string startPeriod = startDate.ToString("yyyy-MM");
            var monthlyData = await _db.AirlineRevenue
                .Where(r => r.PeriodType == "month" &&r.Period.CompareTo(startPeriod) >= 0)
                .Select(r => new {
                    r.Period,
                    r.Revenue
                })
                .AsNoTracking() 
                .ToListAsync(); 

            var result = monthlyData
                .GroupBy(r => new {
                    Year = r.Period.Substring(0, 4),   
                    Month = int.Parse(r.Period.Substring(5, 2)) 
                })
                .Select(g => new {
                    YearMonth = g.Key, 
                    Period = $"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)}",
                    Revenue = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.YearMonth.Year)    
                .ThenByDescending(x => x.YearMonth.Month)   
                .Take(6)
                .Select(x => new {
                    year =x.YearMonth.Year,
                    period = x.Period,
                    Revenue = x.Revenue
                })
                .ToList();

            return result;
        }

        private async Task<object> GetYearlyRevenueData()
        {
            return await _db.AirlineRevenue
                 .Where(r => r.PeriodType == "year") 
                 .GroupBy(r => r.Period)             
                 .Select(g => new
                 {
                     period = g.Key,
                     revenue = g.Sum(x => x.Revenue) 
                 })
                 .OrderByDescending(x => x.period) 
                 .ToListAsync();
        }
    }
}