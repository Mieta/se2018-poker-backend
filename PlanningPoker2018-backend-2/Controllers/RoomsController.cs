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
    [Route("api/Rooms")]
    public class RoomsController : Controller
    {
        private readonly DatabaseContext _context;

        public RoomsController(DatabaseContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IEnumerable<Room> GetRooms()
        {
            return _context.Room.ToList();
        }

        // PUT: api/rooms
        [HttpPut]
        public async Task<IActionResult> PutRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Room.Add(room);
            await _context.SaveChangesAsync();

            room.link = "http://localhost:4200/room/" + room.id.ToString();

            return CreatedAtAction("GetRoom", new { id = room.id }, room);
        }

    

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.id == id);
        }
    }
}