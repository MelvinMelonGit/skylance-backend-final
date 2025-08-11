using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketSalesController : ControllerBase
    {
        private readonly SkylanceDbContext _context;

        public TicketSalesController(SkylanceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketSales([FromQuery] string periodType)
        {
            if (string.IsNullOrWhiteSpace(periodType))
                return BadRequest(new { status = "error", message = "Missing periodType" });

            var salesQuery = _context.TicketSales
                .Include(ts => ts.Aircraft)
                .AsQueryable();

            var grouped = new List<Dictionary<string, object>>();

            if (periodType.ToLower() == "month")
            {
                var rawData = await salesQuery
                    .ToListAsync();

                var groupedRaw = rawData
                    .GroupBy(ts => ts.SaleDate.ToString("MMM"))
                    .Select(g => new
                    {
                        Period = g.Key,
                        AirlineSales = g
                            .GroupBy(ts => ts.Aircraft.Airline)
                            .ToDictionary(a => a.Key, a => a.Sum(x => x.Sales))
                    })
                    .ToList();

                grouped = groupedRaw.Select(item =>
                {
                    var dict = new Dictionary<string, object>
                    { 
                        ["period"] = item.Period 
                    };
                    foreach (var kv in item.AirlineSales)
                        dict[kv.Key] = kv.Value;

                    return dict;
                }).ToList();
            }
            else if (periodType.ToLower() == "year")
            {
                var rawData = await salesQuery
                    .ToListAsync();

                var groupedRaw = rawData
                    .GroupBy(ts => ts.SaleDate.Year.ToString())
                    .Select(g => new
                    {
                        Period = g.Key,
                        AirlineSales = g
                            .GroupBy(ts => ts.Aircraft.Airline)
                            .ToDictionary(a => a.Key, a => a.Sum(x => x.Sales))
                    })
                    .ToList();

                grouped = groupedRaw.Select(item =>
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["period"] = item.Period
                    };
                    foreach (var kv in item.AirlineSales)
                        dict[kv.Key] = kv.Value;

                    return dict;
                }).ToList();
            }
            else
            {
                return BadRequest(new { status = "error", message = "Invalid periodType. Use 'month' or 'year'." });
            }

            return Ok(new
            {
                status = "success",
                periodType = periodType.ToLower(),
                data = grouped
            });
        }
    }

}