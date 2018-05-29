﻿using System;
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

            return CreatedAtAction("GetProjectTask", new { id = projectTask.id }, projectTask);
        }

        [HttpPost("{id}/status/{statusName}")]
        public async Task<IActionResult> UpdateTaskStatus([FromRoute] int id, [FromRoute] String statusName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}