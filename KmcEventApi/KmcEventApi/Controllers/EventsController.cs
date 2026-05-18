using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KmcEventApi.Data;
using KmcEventApi.Models;

namespace KmcEventApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> SearchEvents(string? type, DateTime? date)
        {
            // අලුත් වෙනස: "Public" ඒවා සහ කලින් හදපු පරණ (null/හිස්) ඒවා ඔක්කොම ගන්නවා
            var query = _context.Events
                .Where(e => e.Visibility == "Public" || string.IsNullOrEmpty(e.Visibility))
                .AsQueryable();

            if (!string.IsNullOrEmpty(type)) query = query.Where(e => e.Type == type);
            if (date.HasValue) query = query.Where(e => e.EventDate.Date == date.Value.Date);

            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(SearchEvents), new { id = newEvent.Id }, newEvent);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("ByOrganizer/{email}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByOrganizer(string email)
        {
            return await _context.Events
                .Where(e => e.OrganizerEmail == email)
                .ToListAsync();
        }
        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            return ev;
        }
        // PUT: api/Events/5
[HttpPut("{id}")]
public async Task<IActionResult> PutEvent(int id, Event updatedEvent)
{
    // Frontend එකෙන් එවන ID එකයි, Data වල තියෙන ID එකයි සමානද බලනවා
    if (id != updatedEvent.Id)
    {
        return BadRequest(new { message = "ID mismatch" });
    }

    _context.Entry(updatedEvent).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Events.Any(e => e.Id == id))
        {
            return NotFound(new { message = "Event not found" });
        }
        else
        {
            throw;
        }
    }

    return NoContent(); // සාර්ථකව Update වුණාම යවන හිස් පිළිතුර
}
    }
}