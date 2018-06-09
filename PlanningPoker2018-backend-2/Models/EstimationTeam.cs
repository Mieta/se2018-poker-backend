using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanningPoker2018_backend_2.Models
{
    public class EstimationTeam
    {
        [Key] public int id { get; set; }
        [Required] public string name { get; set; }
        public string creator { get; set; }
        [NotMapped] public List<TeamMember> members { get; set; }
    }
}