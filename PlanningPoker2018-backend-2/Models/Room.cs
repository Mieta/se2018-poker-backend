using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.Models
{
    public class Room
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [NotMapped]
        public string link { get; set; }
    }
}
