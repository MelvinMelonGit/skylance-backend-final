using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public LoginController (SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetCountries()
        {
            var countries = db.Countries
                .Select(c => new
                {
                    Id = c.Id,
                    Nationality = c.Name,
                    MobileCode = c.MobileCode
                }).ToList();

            return Ok(countries);
        }
    }
}