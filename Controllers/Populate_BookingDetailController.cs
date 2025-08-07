using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_BookingDetailController : ControllerBase
    {
        private readonly SkylanceDbContext db;

        public Populate_BookingDetailController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.BookingDetails.Any())
                return BadRequest("Data already seeded.");

            var appUsers = db.AppUsers.ToDictionary(a => a.Email, a => a);

            List<BookingDetail> bookingDetailList = new List<BookingDetail>
            {                
                new BookingDetail {
                    BookingReferenceNumber = "G66666",
                    AppUser = appUsers["teng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "K78906",
                    AppUser = appUsers["meng@gmail.com"]
                },
                
                new BookingDetail {
                    BookingReferenceNumber = "G66688",
                    AppUser = appUsers["seng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "J01927",
                    AppUser = appUsers["beng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "L76543",
                    AppUser = appUsers["leng@gmail.com"]
                },
                
                new BookingDetail {
                    BookingReferenceNumber = "H54321",
                    AppUser = appUsers["teng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "A00835",
                    AppUser = appUsers["seng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "U24899",
                    AppUser = appUsers["seng@gmail.com"]
                },

                new BookingDetail {
                    BookingReferenceNumber = "H67556",
                    AppUser = appUsers["leng@gmail.com"]
                },                

                new BookingDetail {
                    BookingReferenceNumber = "H37766",
                    AppUser = appUsers["teng@gmail.com"]
                },
            };

                db.AddRange(bookingDetailList);
                db.SaveChanges();

                return Ok("Booking detail records created successfully");
        }
    }
}

