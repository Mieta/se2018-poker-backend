using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.Models
{
    public class ProjectTask
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        public User author { get; set; }
        [Required]
        public int RoomId { get; set; }
        public int estimate { get; set; }
        
        public String status { get; set; }
    }
}
