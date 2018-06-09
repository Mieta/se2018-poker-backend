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
    [Route("api/EstimationTeams")]
    public class EstimationTeamsController : Controller
    {
        private readonly DatabaseContext _context;

        public EstimationTeamsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/EstimationTeams
        [HttpGet]
        public IEnumerable<EstimationTeam> GetEstimationTeam()
        {
            var estimationTeams = _context.EstimationTeam.ToList();
            estimationTeams.ForEach(t => { t.members = _context.TeamMember.Where(m => m.teamId == t.id).ToList(); });
            return estimationTeams;
        }

        // GET: api/EstimationTeams/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstimationTeam([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var estimationTeam = await _context.EstimationTeam.SingleOrDefaultAsync(m => m.id == id);

            if (estimationTeam == null)
            {
                return NotFound();
            }

            estimationTeam.members = _context.TeamMember.Where(tm => tm.teamId == estimationTeam.id).ToList();
            return Ok(estimationTeam);
        }

        // PUT: api/EstimationTeams
        [HttpPut]
        public async Task<IActionResult> PutNewEstimationTeam([FromBody] EstimationTeam estimationTeam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.EstimationTeam.Add(estimationTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEstimationTeam", new {id = estimationTeam.id}, estimationTeam);
        }
    }
}