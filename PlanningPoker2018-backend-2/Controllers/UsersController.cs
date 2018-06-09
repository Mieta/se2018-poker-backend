using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> PutNewUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password, 15);
            if (isUserExists(user))
            {
                return BadRequest(new BasicResponse {message = "User already exists"});
            }
            
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new {id = user.id}, new BasicResponse {message = "User created successfully"} );
        }

        private bool isUserExists(User user)
        {
            return _context.User.Any(u => u.mailAddress == user.mailAddress);
        }
    }
}