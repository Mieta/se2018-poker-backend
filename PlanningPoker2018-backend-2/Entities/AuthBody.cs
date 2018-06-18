using System.ComponentModel.DataAnnotations;

namespace PlanningPoker2018_backend_2.Entities
{
    public class AuthBody
    {
        [Required]
        public string mailAddress { get; set; }
        [Required]
        public string password { get; set; }
    }
}