using Microsoft.AspNetCore.Mvc;
using KmcEventApi.Data;
using KmcEventApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KmcEventApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParticipantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register(Participant participant)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.Id == participant.EventId);
            if (!eventExists) return BadRequest("Invalid Event ID.");

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Successfully registered!", participant });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Participant>>> GetAllParticipants()
        {
            return await _context.Participants.ToListAsync();
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants(int eventId)
        {
            return await _context.Participants.Where(p => p.EventId == eventId).ToListAsync();
        }

        // 4. DELETE: api/participants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null) return NotFound();

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}