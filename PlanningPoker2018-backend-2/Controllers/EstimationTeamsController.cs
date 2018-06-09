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
            return _context.EstimationTeam;
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

            return Ok(estimationTeam);
        }

        // PUT: api/EstimationTeams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstimationTeam([FromRoute] int id, [FromBody] EstimationTeam estimationTeam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != estimationTeam.id)
            {
                return BadRequest();
            }

            _context.Entry(estimationTeam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstimationTeamExists(id))
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

        // POST: api/EstimationTeams
        [HttpPost]
        public async Task<IActionResult> PostEstimationTeam([FromBody] EstimationTeam estimationTeam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.EstimationTeam.Add(estimationTeam);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEstimationTeam", new { id = estimationTeam.id }, estimationTeam);
        }

        // DELETE: api/EstimationTeams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstimationTeam([FromRoute] int id)
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

            _context.EstimationTeam.Remove(estimationTeam);
            await _context.SaveChangesAsync();

            return Ok(estimationTeam);
        }

        private bool EstimationTeamExists(int id)
        {
            return _context.EstimationTeam.Any(e => e.id == id);
        }
    }
}