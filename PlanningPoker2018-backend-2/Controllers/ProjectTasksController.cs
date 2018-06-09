using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;
using SQLitePCL;
using TaskStatus = PlanningPoker2018_backend_2.Entities.TaskStatus;
using SO = System.IO.File;

namespace PlanningPoker2018_backend_2.Controllers
{
    [Produces("application/json")]
    [Route("api/tasks")]
    public class ProjectTasksController : Controller
    {
        private readonly DatabaseContext _context;

        public ProjectTasksController(DatabaseContext context)
        {
            _context = context;
        }

        //GET: api/tasks
        [HttpGet]
        public IEnumerable<ProjectTask> GetProjectTasks()
        {
            return _context.ProjectTask;
        }

        // GET: api/tasks/{roomId}
        [HttpGet("{id}")]
        public ProjectTask GetProjectTask(int id)
        {
            return _context.ProjectTask.First(t => t.id == id);
        }


        // PUT: api/tasks
        [HttpPut]
        public async Task<IActionResult> CreateProjectTask([FromBody] ProjectTask projectTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProjectTask.Add(projectTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectTask", new {id = projectTask.id}, projectTask);
        }

        // Patch: api/tasks/{taskId}
        [HttpPatch("{taskId}")]
        public async Task<IActionResult> PatchProjectTaskEstimate(int taskId, int estimate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectTask = new ProjectTask() {id = taskId, estimate = estimate};
            _context.ProjectTask.Attach(projectTask);
            _context.Entry(projectTask).Property(t => t.estimate).IsModified = true;
            await _context.SaveChangesAsync();

            return AcceptedAtAction("ChangeProjectTaskEstimate", new
            {
                id = projectTask.id,
                estimate = projectTask.estimate
            });
        }

        [HttpPost("{id}/status/{statusName}")]
        public async Task<IActionResult> UpdateTaskStatus([FromRoute] int id, [FromRoute] String statusName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TaskStatus newStatus;
            try
            {
                newStatus = TaskStatus.getByName(statusName);
            }
            catch (StatusNotFoundException ex)
            {
                return BadRequest(new BasicResponse() {message = "Wrong status provided"});
            }

            var task = new ProjectTask() {id = id, status = newStatus.name};
            _context.ProjectTask.Attach(task);
            _context.Entry(task).Property(t => t.status).IsModified = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{roomId}/parse")]
        public async Task<IActionResult> ParseCSVToTasks([FromRoute] int roomId, [FromQuery] string delimeter,
            IFormFile httpFile)
        {
            if (_context.Room.Find(roomId) == null)
            {
                return NotFound(new BasicResponse() {message = "No room with id " + roomId + " found"});
            }

            Stream fileStream = httpFile.OpenReadStream();

            var reader = new StreamReader(fileStream, true);
            var line2 = reader.ReadLine();
            var header = line2.Split(delimeter);

            var summaryIndex = System.Array.IndexOf(header, "Summary");
            var estimateIndex = System.Array.IndexOf(header, "Original Estimate");

            var tasks = new List<ProjectTask>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var lineObjects = line.Split(delimeter);
                var task = new ProjectTask();
                task.title = lineObjects[summaryIndex];
                if (lineObjects[estimateIndex] != "")
                {
                    task.estimate = int.Parse(lineObjects[estimateIndex]);
                }

                task.RoomId = roomId;
                _context.ProjectTask.Add(new ProjectTask());
            }

            return AcceptedAtAction("Parse CSV to tasks");
        }
    }
}