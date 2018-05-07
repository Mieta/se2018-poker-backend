using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlanningPoker2018_backend_2.Models;

namespace PlanningPoker2018_backend_2.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PlanningPoker2018_backend_2.Models.Room> Room { get; set; }

        public DbSet<PlanningPoker2018_backend_2.Models.User> User { get; set; }

        public DbSet<PlanningPoker2018_backend_2.Models.UserRole> UserRole { get; set; }
    }
}
