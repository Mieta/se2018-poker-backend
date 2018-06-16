using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Models;

namespace PlanningPoker2018_backend_2.Controllers
{
    [Produces("application/json")]
    [Route("api/RoomParticipants")]
    public class RoomParticipantsController : Controller
    {
        private readonly DatabaseContext _context;

        public RoomParticipantsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/RoomParticipants
        [HttpGet]
        public IEnumerable<RoomParticipant> GetRoomParticipant()
        {
            return _context.RoomParticipant;
        }

        // GET: api/RoomParticipants/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomParticipant([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomParticipant = await _context.RoomParticipant.SingleOrDefaultAsync(m => m.id == id);

            if (roomParticipant == null)
            {
                return NotFound();
            }

            return Ok(roomParticipant);
        }

        // PUT: api/RoomParticipants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomParticipant([FromRoute] int id, [FromBody] RoomParticipant roomParticipant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roomParticipant.id)
            {
                return BadRequest();
            }

            _context.Entry(roomParticipant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomParticipantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RoomParticipants
        [HttpPost]
        public async Task<IActionResult> PostRoomParticipant([FromBody] RoomParticipant roomParticipant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RoomParticipant.Add(roomParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomParticipant", new { id = roomParticipant.id }, roomParticipant);
        }

        // DELETE: api/RoomParticipants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomParticipant([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomParticipant = await _context.RoomParticipant.SingleOrDefaultAsync(m => m.id == id);
            if (roomParticipant == null)
            {
                return NotFound();
            }

            _context.RoomParticipant.Remove(roomParticipant);
            await _context.SaveChangesAsync();

            return Ok(roomParticipant);
        }

        private bool RoomParticipantExists(int id)
        {
            return _context.RoomParticipant.Any(e => e.id == id);
        }
    }
}