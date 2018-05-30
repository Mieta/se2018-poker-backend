using System;
using Microsoft.CodeAnalysis;

namespace PlanningPoker2018_backend_2.Models
{
    public class GameSummary
    {
        public String date { get; set; }
        public String roomName { get; set; }
        public User[] participants { get; set; }
        public ProjectTask[] tasks { get; set; }
    }
}
