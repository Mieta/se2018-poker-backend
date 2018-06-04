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
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Users/{userId}
        [HttpGet("userId")]
        public User GetUser(int userId)
        {
            return _context.User.First(u => u.id == userId);
        }


        //POST: api/Users/join
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            user.roleId = 2;
            if (ModelState.IsValid)
            {
                _context.User.Add(user);
                return CreatedAtAction("AddUser", user);
            }
            return BadRequest();
        }
        
        //POST: api/Users/po/join
        [HttpPost]
        public async Task<IActionResult> AddProductOwnerUser([FromBody] User user)
        {
            user.roleId = 1;
            if (ModelState.IsValid)
            {
                _context.User.Add(user);
                return CreatedAtAction("AddUser", user);
            }
            return BadRequest();
        }


        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.id == id);
        }
    }
}