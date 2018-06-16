using System.ComponentModel.DataAnnotations;

namespace PlanningPoker2018_backend_2.Models
{
    public class RoomParticipant
    {
        [Key]
        public int id { get; set; }
        public int roomId { get; set; }
        public string mailAddress { get; set; }
        public string userName { get; set; }
    }
}