using System.ComponentModel.DataAnnotations;

namespace PlanningPoker2018_backend_2.Entities
{
    public class AddTeamMemberBody
    {
        [Required]
        public string mailAddress { get; set; }
    }
}