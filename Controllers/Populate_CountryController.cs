using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_CountryController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public Populate_CountryController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {

            if (db.Countries.Any())
                return BadRequest("Data already seeded.");

            List<Country> countryList = new List<Country>
            { 
                new Country { Name = "Singapore", CountryCode = "SG", MobileCode = 65 },
                new Country { Name = "Japan", CountryCode = "JP", MobileCode = 81 },
                new Country { Name = "Malaysia", CountryCode = "MY", MobileCode = 60 },
                new Country { Name = "South Korea", CountryCode = "KR", MobileCode = 82 },
                new Country { Name = "Australia", CountryCode = "AU", MobileCode = 61 },           
                new Country { Name = "United Arab Emirates", CountryCode = "AE", MobileCode = 971 },
                new Country { Name = "Switzerland", CountryCode = "CH", MobileCode = 41 },
                new Country { Name = "Vietnam", CountryCode = "VN", MobileCode = 84 },
                new Country { Name = "United Kingdom", CountryCode = "GB", MobileCode = 44 },
                new Country { Name = "France", CountryCode = "FR", MobileCode = 33 },
                new Country { Name = "Germany", CountryCode = "DE", MobileCode = 49 },
                new Country { Name = "Spain", CountryCode = "ES", MobileCode = 34 },
                new Country { Name = "Netherlands", CountryCode = "NL", MobileCode = 31 },
                new Country { Name = "Brazil", CountryCode = "BR", MobileCode = 55 },
                new Country { Name = "Argentina", CountryCode = "AR", MobileCode = 54 }
            };

            db.Countries.AddRange(countryList);

            db.SaveChanges();

            return Ok("Seeded Country records.");
        }

    }
}