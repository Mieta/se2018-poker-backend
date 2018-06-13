using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;
using Xbehave;
using Xunit;

namespace PlanningPoker2018_backend.Tests
{
    public class ManageTasksFeature
    {
        [Scenario]
        public void SelectingTaskToEstimate(ProjectTask task)
        {
            "Given a new task"
                .x(() => task = new ProjectTask()
                {
                    id = 1,
                    status = TaskStatus.UNESTIMATED.name,
                    title = "New task in BDD",
                    RoomId = 999,
                    estimate = 0
                });

            "When I select task"
                .x(() => task.status = TaskStatus.VOTING.name);

            "Then task status is 'Voting'"
                .x(() => Assert.Equal(task.status, TaskStatus.VOTING.name));
        }
    }
}