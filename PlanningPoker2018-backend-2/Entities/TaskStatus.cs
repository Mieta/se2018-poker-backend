using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.Entities
{
    public sealed class TaskStatus
    {
        public readonly string name;
        private readonly int value;
        
        public static readonly TaskStatus UNESTIMATED = new TaskStatus(1, "Unestimated");
        public static readonly TaskStatus VOTING = new TaskStatus(2, "Voting");
        public static readonly TaskStatus ESTIMATED = new TaskStatus(3, "Estimated");

        private static readonly Dictionary<string, TaskStatus> statuses = new Dictionary<string, TaskStatus>()
        {
            {UNESTIMATED.name, UNESTIMATED},
            {VOTING.name, VOTING },
            {ESTIMATED.name, ESTIMATED }
        };

        public TaskStatus(int value, string name)
        {
            this.name = name;
            this.value = value;
        }

        public static TaskStatus getByName(string name)
        {
            string formattedName = name.First().ToString().ToUpper() + name.Substring(1);
            if (statuses.ContainsKey(formattedName))
            {
                return statuses[formattedName];
            }

            throw new StatusNotFoundException(name);
        }
    }

    class StatusNotFoundException : Exception
    {
        private String statusName;

        public StatusNotFoundException(string statusName)
        {
            this.statusName = statusName;
        }
    }
}