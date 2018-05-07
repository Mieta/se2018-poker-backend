using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.Models
{
    public class UserRole
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
    }
}
