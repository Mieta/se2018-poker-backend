using System;
using System.Collections.Generic;
using System.Globalization;
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

        [HttpGet]
        public Room getRoom(int id)
        {
            return _context.Room.First(r => r.id == id);
        }

        [HttpGet("{roomId}/tasks")]
        public IEnumerable<ProjectTask> GetProjectTaskForRoom([FromRoute] int roomId)
        {
            return _context.ProjectTask.Where(t => t.RoomId.Equals(roomId));
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

            room.link = "https://online-planning-poker.herokuapp.com/room/participant/" + room.id.ToString();

            return CreatedAtAction("GetRoom", new {id = room.id}, room);
        }

        // GET: api/rooms/{roomId}/summary
        [HttpGet("{roomId}/summary")]
        public GameSummary GetGameSummary(int roomId)
        {
            var roomName = _context.Room.First(r => r.id == roomId).name;
            var users = _context.User.Where(u => u.roomId == roomId).ToArray();
            var roomTasks = _context.ProjectTask.Where(task => task.RoomId == roomId).ToArray();
            var currentDate = DateTime.Now;

            return new GameSummary()
            {
                date = currentDate.ToString(new CultureInfo("pl-PL")),
                participants = users,
                roomName = roomName,
                tasks = roomTasks
            };
        }
        
    }
}