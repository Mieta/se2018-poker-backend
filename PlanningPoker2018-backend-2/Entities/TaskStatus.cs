namespace PlanningPoker2018_backend_2.Entities
{
    public sealed class TaskStatus
    {
        private readonly string name;
        private readonly int value;
        
        public static readonly TaskStatus UNESTIMATED = new TaskStatus(1, "Unestimated");
        public static readonly TaskStatus VOTING = new TaskStatus(2, "Voting");

        public TaskStatus(int value, string name)
        {
            this.name = name;
            this.value = value;
        }
    }
}