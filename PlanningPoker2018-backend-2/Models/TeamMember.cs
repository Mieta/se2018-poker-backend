using System.ComponentModel.DataAnnotations;

namespace PlanningPoker2018_backend_2.Models
{
    public class TeamMember
    {
        [Key] public int id { get; set; }
        [Required] public int teamId { get; set; }
        [Required] public string mailAddress { get; set; }
    }
}