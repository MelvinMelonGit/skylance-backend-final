using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_CityController : Controller
    {
        private readonly SkylanceDbContext db;
        public Populate_CityController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.Cities.Any())
            return BadRequest("Data already seeded.");
           
            var countries = db.Countries.ToDictionary(c => c.Name, c => c);

            List<City> cityList = new List<City>
            {
                new City { Name = "Singapore", Country = countries["Singapore"] },
                new City { Name = "Tokyo", Country = countries["Japan"] },
                new City { Name = "Seoul", Country = countries["South Korea"] },
                new City { Name = "Canberra", Country = countries["Australia"] },
                new City { Name = "Kuala Lumpur", Country = countries["Malaysia"]},           
                new City { Name = "Abu Dhabi", Country = countries["United Arab Emirates"] },
                new City { Name = "Zurich", Country = countries["Switzerland"] },
                new City { Name = "Hanoi", Country = countries["Vietnam"] }
            };

            db.Cities.AddRange(cityList);
            db.SaveChanges();

            return Ok("Cities seeded successfully.");
        }

    }
}

