using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;

namespace PlanningPoker2018_backend_2.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Users/{userId}
        [HttpGet("{userId}")]
        public User GetUser(int userId)
        {
            return _context.User.First(u => u.id == userId);
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult AuthenticateUser([FromBody] AuthBody authBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_context.User.Any(u => authBody.mailAddress.Equals(u.mailAddress)))
            {
                return NotFound();
            }

            var user = _context.User.First(u => u.mailAddress.Equals(authBody.mailAddress));
            if (BCrypt.Net.BCrypt.Verify(authBody.password, user.password))
            {
                var response = new AuthResponseBody()
                {
                    id = user.id,
                    mailAddress = user.mailAddress,
                    team = user.team,
                    username = user.username
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(new BasicResponse {message = "Wrong password"});
            }
        }


        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> PutNewUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(user.password, 11);
            if (isUserExists(user))
            {
                return BadRequest(new BasicResponse {message = "User already exists"});
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new {id = user.id},
                new BasicResponse {message = "User created successfully"});
        }

        private bool isUserExists(User user)
        {
            return _context.User.Any(u => u.mailAddress == user.mailAddress);
        }
    }
}