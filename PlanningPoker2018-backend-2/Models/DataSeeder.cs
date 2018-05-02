using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.Models
{
    public class DataSeeder
    {
        public static void SeedUserRoles(DatabaseContext context)
        {
            if(!context.UserRole.Any())
            {
                var roles = new List<UserRole>
                {
                    new UserRole {name = "Product Owner"},
                    new UserRole {name = "User"}
                };
                context.AddRange(roles);
                context.SaveChanges();
            }
        }
    }
}
