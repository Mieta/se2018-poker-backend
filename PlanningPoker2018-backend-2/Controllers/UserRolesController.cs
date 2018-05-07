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
    [Route("api/UserRoles")]
    public class UserRolesController : Controller
    {
        private readonly DatabaseContext _context;

        public UserRolesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/UserRoles
        [HttpGet]
        public IEnumerable<UserRole> GetUserRole()
        {
            return _context.UserRole;
        }
    }
}