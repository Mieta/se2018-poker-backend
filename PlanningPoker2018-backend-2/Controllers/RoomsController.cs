using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;

namespace PlanningPoker2018_backend_2.Controllers
{
    [Produces("application/json")]
    [Route("api/Rooms")]
    public class RoomsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly List<string> _exportFormats = new List<string> {"csv"};

        public RoomsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Room> GetRooms()
        {
            return _context.Room.ToList();
        }

        [HttpGet("{roomId}")]
        public Room GetRoom(int roomId)
        {
            return _context.Room.First(r => r.id == roomId);
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

            room.link = "https://online-planning-poker.herokuapp.com/room/participant/" + room.id;

            return CreatedAtAction("GetRoom", new {room.id}, room);
        }

        // GET: api/rooms/{roomId}/summary
        [HttpGet("{roomId}/summary")]
        public async Task<GameSummary> GetGameSummary(int roomId)
        {
            var room = _context.Room.First(r => r.id == roomId);
            var roomName = room.name;
            var roomTasks = _context.ProjectTask.Where(task => task.RoomId == roomId).ToArray();
            var currentDate = DateTime.Now;
            var roomHost = room.hostMailAddress ?? room.hostUsername;
            room.roomDate = currentDate.ToString(new CultureInfo("pl-PL"));
            _context.Entry(room).Property(t => t.roomDate).IsModified = true;
            await _context.SaveChangesAsync();
            var roomParticipants = _context.RoomParticipant.Where(rp => rp.roomId == roomId).ToArray();
            return new GameSummary()
            {
                host = roomHost,
                date = currentDate.ToString(new CultureInfo("pl-PL")),
                participants = roomParticipants,
                roomName = roomName,
                tasks = roomTasks
            };
        }

        [HttpPost("{roomId}/po")]
        public async Task<IActionResult> AssignPoToRoom([FromRoute] int roomId, [FromBody] RoomAssignmentBody body)
        {
            if (!_context.Room.Any(r => r.id == roomId))
            {
                return NotFound(new BasicResponse {message = "Nie znaleziono pokoju o podanym id"});
            }

            var fetchedRoom = _context.Room.First(r => r.id == roomId);
            if (fetchedRoom.hostMailAddress != null)
            {
                return BadRequest(new BasicResponse {message = "Host already assigned"});
            }

            if (body.mailAddress != null)
            {
                if (_context.User.Any(u => u.mailAddress.Equals(body.mailAddress)))
                {
                    fetchedRoom.hostMailAddress = body.mailAddress;
                    _context.Entry(fetchedRoom).Property(t => t.hostMailAddress).IsModified = true;
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                else
                {
                    return NotFound(new BasicResponse
                    {
                        message = "Nie znaleziono użytkownika o podanym adresie e-mail"
                    });
                }
            }
            else if (body.username != null)
            {
                var roomToUpdate = new Room() {id = roomId, hostUsername = body.username};
                _context.Room.Attach(roomToUpdate);
                _context.Entry(roomToUpdate).Property(t => t.hostUsername).IsModified = true;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest(new BasicResponse() {message = "Missing parameters"});
            }
        }

        [HttpPost("{roomId}/participant")]
        public async Task<IActionResult> AssignParticipantToRoom([FromRoute] int roomId,
            [FromBody] RoomAssignmentBody body)
        {
            if (!_context.Room.Any(r => r.id == roomId))
            {
                return NotFound(new BasicResponse {message = "Nie znaleziono pokoju o podanym id"});
            }

            if (body.mailAddress != null)
            {
                if (!_context.User.Any(u => u.mailAddress.Equals(body.mailAddress)))
                    return NotFound(new BasicResponse
                    {
                        message = "Nie znaleziono użytkownika o podanym adresie e-mail"
                    });
                var fetchedRoom = _context.Room.First(r => r.id == roomId);
                if (fetchedRoom.hostMailAddress != null)
                {
                    return BadRequest(new BasicResponse {message = "Host already assigned"});
                }

                var roomParticipant = new RoomParticipant() {mailAddress = body.mailAddress, roomId = roomId};
                _context.RoomParticipant.Add(roomParticipant);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            if (body.username != null)
            {
                var roomParticipant = new RoomParticipant() {userName = body.username, roomId = roomId};
                _context.RoomParticipant.Add(roomParticipant);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest(new BasicResponse() {message = "Missing parameters"});
        }

        [HttpGet("{roomId}/summary/export/{format}")]
        public IActionResult GetExportedSummaryReport([FromRoute] int roomId, [FromRoute] string format = "csv")
        {
            if (!_context.Room.Any(r => r.id.Equals(roomId)))
            {
                return NotFound(new BasicResponse {message = "Room not found"});
            }

            if (!_exportFormats.Contains(format))
            {
                return BadRequest(new BasicResponse {message = "Unsupported format"});
            }

            var room = _context.Room.First(r => r.id == roomId);
            var tasks = _context.ProjectTask.Where(t => t.RoomId.Equals(roomId)).ToList();
            if (room.roomDate == null)
            {
                return BadRequest(new BasicResponse {message = "Estimation have not finished yet"});
            }

            var csvRows = new List<CsvRow>();
            tasks.ForEach(t =>
            {
                csvRows.Add(new CsvRow()
                {
                    StoryPoints = t.estimate.ToString(),
                    Summary = t.title
                });
            });
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var csv = new CsvWriter(streamWriter);
            csv.Configuration.RegisterClassMap<CsvRowMap>();
            csv.WriteRecords(csvRows);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            var sb = new StringBuilder();
            sb.Append(room.id)
                .Append("-")
                .Append(room.name)
                .Append("-export.csv");
            var response = File(stream, "text/csv", sb.ToString());
            return response;
        }
    }
}