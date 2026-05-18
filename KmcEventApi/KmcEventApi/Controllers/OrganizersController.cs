using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KmcEventApi.Data;
using KmcEventApi.Models;

[Route("api/[controller]")]
[ApiController]
public class OrganizersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrganizersController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/Organizers/Register
    [HttpPost("Register")]
    public async Task<IActionResult> Register(Organizer organizer)
    {
        if (organizer == null) return BadRequest();

        _context.Organizers.Add(organizer);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration Successful!" });
    }

    // POST: api/Organizers/Login
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] Organizer loginDto)
    {
        var user = await _context.Organizers
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

        if (user == null) return Unauthorized(new { message = "Invalid Email or Password" });

        return Ok(new { message = "Login Successful", userName = user.Name });
    }
}