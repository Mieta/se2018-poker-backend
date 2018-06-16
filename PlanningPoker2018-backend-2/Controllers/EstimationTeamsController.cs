using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;

namespace PlanningPoker2018_backend_2.Controllers
{
    [Produces("application/json")]
    [Route("api/teams")]
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
            estimationTeams.ForEach(t =>
            {
                t.members = _context.TeamMember.Where(m => m.teamId == t.id)
                    .Select(s => s.mailAddress)
                    .ToList();
            });
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

            estimationTeam.members = _context.TeamMember.Where(tm => tm.teamId == estimationTeam.id)
                .Select(s => s.mailAddress)
                .ToList();
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

        [HttpPost("{teamId}")]
        public IActionResult AddMemberToTeam([FromRoute] int teamId, [FromBody] AddTeamMemberBody body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.EstimationTeam.Any(t => t.id == teamId))
            {
                return NotFound(new BasicResponse() {message = "Estimation team not found"});
            }

            if (!_context.User.Any(u => u.mailAddress == body.mailAddress))
            {
                return NotFound(new BasicResponse() {message = "User to invite not found"});
            }

            if (_context.TeamMember.Any(m => m.teamId == teamId && m.mailAddress == body.mailAddress))
            {
                return BadRequest(new BasicResponse() {message = "User is already a member of a team"});
            }

            var member = new TeamMember() {teamId = teamId, mailAddress = body.mailAddress};
            _context.TeamMember.Add(member);
            _context.SaveChanges();
            return NoContent();
        }
    }
}