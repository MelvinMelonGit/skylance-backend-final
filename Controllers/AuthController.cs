using Microsoft.AspNetCore.Mvc;
using skylance_backend.Attributes;
using skylance_backend.Data;
using skylance_backend.Models;

namespace skylance_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SkylanceDbContext _db;

    public AuthController(SkylanceDbContext db)
    {
        _db = db;
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO request)
    {
        // 1. Validate user
        var user = _db.AppUsers
             .FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

         if (user == null)
             return Unauthorized("Invalid credentials");

         // 2. Generate token
         var token = Guid.NewGuid().ToString();

         // 3. Save session in database
         var session = new AppUserSession
         {
             Id = token,
             AppUser = user,
             SessionExpiry = DateTime.UtcNow.AddHours(1)
         };

         _db.AppUserSessions.Add(session);
         _db.SaveChanges();

        return Ok(new
        {
            token,
            user,
            expires = DateTime.UtcNow.AddHours(1)
        });
    }

    [HttpPost("login/employee")]
    public IActionResult EmployeeLogin([FromBody] EmployeeLoginRequestDTO request)
    {
        var employee = _db.Employees
            .FirstOrDefault(e => e.EmployeeNumber == request.EmployeeNumber && e.Password == request.Password);

        if (employee == null)
            return Unauthorized("Invalid credentials");

        var token = Guid.NewGuid().ToString();

        var empSession = new EmployeeSession
        {
            Id = token,
            Employee = employee,
            SessionExpiry = DateTime.UtcNow.AddHours(1)
        };

        _db.EmployeeSessions.Add(empSession);
        _db.SaveChanges();

        return Ok(new
        {
            token,
            expires = empSession.SessionExpiry
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check for duplicate email synchronously
        if (_db.AppUsers.Any(u => u.Email == dto.Email))
            return Conflict("Email already registered.");

        // Lookup related Country entities synchronously
        var nationality = _db.Countries.Find(dto.NationalityId);
        var mobileCode = _db.Countries.Find(dto.MobileCodeId);

        if (nationality == null || mobileCode == null)
            return BadRequest("Invalid nationality or mobile code.");

        var newUser = new AppUser
        {
            Email = dto.Email,
            Password = dto.Password,  // Remember to hash passwords in real apps
            Salutation = dto.Salutation,
            Gender = dto.Gender,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Nationality = nationality,
            MobileCode = mobileCode,
            MobileNumber = dto.MobileNumber,
            MembershipTier = dto.MembershipTier,
            MembershipNumber = dto.MembershipNumber,
            PassportNumber = dto.PassportNumber,
            PassportExpiry = dto.PassportExpiry,
            DateOfBirth = dto.DateOfBirth
        };

        _db.AppUsers.Add(newUser);
        _db.SaveChanges();  // Synchronous save

        return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = Request.Headers["Session-Token"].ToString();
        var session = _db.AppUserSessions.FirstOrDefault(s => s.Id == token);
    
        if (session != null)
        {
            _db.AppUserSessions.Remove(session);
            _db.SaveChanges();
        }

        return Ok();
    }

}