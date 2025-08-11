using Microsoft.AspNetCore.Mvc;

namespace skylance_backend.Controllers;

[ApiController]
[Route("/")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello world, deployment success!" });
    }
}