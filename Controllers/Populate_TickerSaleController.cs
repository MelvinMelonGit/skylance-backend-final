using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_TicketSaleController : Controller
    {
        private readonly SkylanceDbContext db;
        public Populate_TicketSaleController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.TicketSales.Any())
                return BadRequest("Data already seeded.");

            var aircraft = db.Aircraft.ToDictionary(ac => ac.FlightNumber, ac => ac);

            List<TicketSale> ticketSaleList = new List<TicketSale>
            {
                new TicketSale {
                    Id = 1,
                    Aircraft = aircraft["JL1"],
                    SaleDate = new DateTime(2025, 8, 20, 8, 0, 0),
                    Sales = 2000
                },

                new TicketSale {
                    Id = 2,
                    Aircraft = aircraft["JL1"],
                    SaleDate = new DateTime(2025, 9, 20, 8, 0, 0),
                    Sales = 4000
                },

                new TicketSale {
                    Id = 3,
                    Aircraft = aircraft["JL1"],
                    SaleDate = new DateTime(2025, 10, 20, 8, 0, 0),
                    Sales = 1000
                },

                new TicketSale {
                    Id = 4,
                    Aircraft = aircraft["KE85"],
                    SaleDate = new DateTime(2025, 8, 20, 8, 0, 0),
                    Sales = 3000
                },

                new TicketSale {
                    Id = 5,
                    Aircraft = aircraft["KE85"],
                    SaleDate = new DateTime(2025, 9, 20, 8, 0, 0),
                    Sales = 1000
                },

                new TicketSale {
                    Id = 6,
                    Aircraft = aircraft["KE85"],
                    SaleDate = new DateTime(2025, 10, 20, 8, 0, 0),
                    Sales = 2000
                },

                new TicketSale {
                    Id = 7,
                    Aircraft = aircraft["SQ322"],
                    SaleDate = new DateTime(2025, 8, 20, 8, 0, 0),
                    Sales = 4000
                },

                new TicketSale {
                    Id = 8,
                    Aircraft = aircraft["SQ322"],
                    SaleDate = new DateTime(2025, 9, 20, 8, 0, 0),
                    Sales = 3500
                },

                new TicketSale {
                    Id = 9,
                    Aircraft = aircraft["SQ322"],
                    SaleDate = new DateTime(2025, 10, 20, 8, 0, 0),
                    Sales = 2500
                },
            };

            db.TicketSales.AddRange(ticketSaleList);
            db.SaveChanges();

            return Ok("Ticket Sales seeded successfully.");
        }
    }
}