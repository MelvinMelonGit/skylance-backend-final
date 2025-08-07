using Microsoft.AspNetCore.Mvc;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Populate_EmployeeController : ControllerBase
    {
        private readonly SkylanceDbContext db;
        public Populate_EmployeeController(SkylanceDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult Populate()
        {
            if (db.Employees.Any())
                return BadRequest("Data already seeded.");

            List<Employee> employeeList = new List<Employee>
            {
                new Employee
                {
                EmployeeNumber = "51234",
                Username = "timmy",
                Email = "tim@skylance.com",
                Password = "timtim",
                FirstName = "Timmy",
                LastName = "Tim",
                Rank = "M1",
                Position = "Baggage Handler"
                },

                new Employee
                {
                EmployeeNumber = "64435",
                Username = "hammy",
                Email = "ham@skylance.com",
                Password = "hamham",
                FirstName = "Hammy",
                LastName = "Tan",
                Rank = "M2",
                Position = "Gate Agent"
                },

                new Employee
                {
                EmployeeNumber = "76789",
                Username = "john",
                Email = "john@skylance.com",
                Password = "johjoh",
                FirstName = "John",
                LastName = "Oh",
                Rank = "M2",
                Position = "Reservation Staff"
                },

                new Employee
                {
                EmployeeNumber = "66432",
                Username = "heidi",
                Email = "heidi@skylance.com",
                Password = "heihei",
                FirstName = "Heidi",
                LastName = "Hee",
                Rank = "M5",
                Position = "Account Executive"
                },

                new Employee
                {
                EmployeeNumber = "24657",
                Username = "bill",
                Email = "bill@skylance.com",
                Password = "bilbil",
                FirstName = "Bill",
                LastName = "Bun",
                Rank = "M3",
                Position = "Customer Care"
                },
            
            };

                db.Employees.AddRange(employeeList);
                db.SaveChanges();

                return Ok("Employee records created successfully");

        }
    }
}
