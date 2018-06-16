using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlanningPoker2018_backend_2.Models
{
    public class User
    {
        [Key] public int id { get; set; }
        [Required]
        public string mailAddress { get; set; }
        [Required] public string username { get; set; }
        [Required]
        public string password { get; set; }
        public string team { get; set; }
    }
}