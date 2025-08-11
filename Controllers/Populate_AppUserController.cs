using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_AppUserController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public Populate_AppUserController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.AppUsers.Any())
                return BadRequest("Data already seeded.");

            var mobilecodes = db.Countries.ToDictionary(c => c.MobileCode, c => c);
            var nationality = db.Countries.ToDictionary(c => c.Name, c => c);

            List<AppUser> appUserList = new List<AppUser>
            {
                new AppUser
                {
                    Email = "teng@gmail.com",
                    Password = "Teng",
                    Salutation = "Mr",
                    Gender = "M",
                    FirstName = "John",
                    LastName = "Smith",
                    Nationality = nationality["Singapore"],
                    MobileCode = mobilecodes[65],
                    MobileNumber = "91234567",
                    MembershipTier = "Platinum",
                    MembershipNumber = "S123-456-789",
                    PassportNumber = "K1234567A",
                    PassportExpiry = new DateOnly(2030, 12, 20),
                    DateOfBirth = new DateOnly(1994, 08, 10)
                },

                new AppUser
                {
                    Email = "meng@gmail.com",
                    Password = "Meng",
                    Salutation = "Ms",
                    Gender = "M",
                    FirstName = "Mary",
                    LastName = "Poppins",
                    Nationality = nationality["Malaysia"],
                    MobileCode = mobilecodes[60],
                    MobileNumber = "127654321",
                    MembershipTier = "Gold",
                    MembershipNumber = "S234-567-890",
                    PassportNumber = "K2345678B",
                    PassportExpiry = new DateOnly(2031, 12, 20),
                    DateOfBirth = new DateOnly(1985, 03, 20)
                },

                new AppUser
                {
                    Email = "seng@gmail.com",
                    Password = "Seng",
                    Salutation = "Ms",
                    Gender = "M",
                    FirstName = "Linda",
                    LastName = "Too",
                    Nationality = nationality["South Korea"],
                    MobileCode = mobilecodes[82],
                    MobileNumber = "1043215678",
                    MembershipTier = "Silver",
                    MembershipNumber = "S345-678-901",
                    PassportNumber = "K3456789C",
                    PassportExpiry = new DateOnly(2032, 12, 20),
                    DateOfBirth = new DateOnly(1975, 03, 17)
                },

                new AppUser
                {
                    Email = "beng@gmail.com",
                    Password = "Beng",
                    Salutation = "Mdm",
                    Gender = "F",
                    FirstName = "Elsie",
                    LastName = "Bong",
                    Nationality = nationality["Japan"],
                    MobileCode = mobilecodes[81],
                    MobileNumber = "9023416785",
                    MembershipTier = "Normal",
                    MembershipNumber = "S456-789-012",
                    PassportNumber = "K4567890D",
                    PassportExpiry = new DateOnly(2033, 12, 20),
                    DateOfBirth = new DateOnly(2000, 11, 04)
                },

                new AppUser
                {
                    Email = "leng@gmail.com",
                    Password = "Leng",
                    Salutation = "Dr",
                    Gender = "F",
                    FirstName = "Rocky",
                    LastName = "Lim",
                    Nationality = nationality["Australia"],
                    MobileCode = mobilecodes[61],
                    MobileNumber = "412345876",
                    MembershipTier = "Silver",
                    MembershipNumber = "S567-890-123",
                    PassportNumber = "K5678901E",
                    PassportExpiry = new DateOnly(2034, 12, 20),
                    DateOfBirth = new DateOnly(2005, 01, 10)
                },
            };

            db.AppUsers.AddRange(appUserList);
            db.SaveChanges();

            return Ok("App user records created successfully");

        }
    }
}