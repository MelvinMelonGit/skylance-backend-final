using Microsoft.AspNetCore.Mvc;

namespace skylance_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestingController : Controller
{
    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello from your API! Deployed!" });
    }
}