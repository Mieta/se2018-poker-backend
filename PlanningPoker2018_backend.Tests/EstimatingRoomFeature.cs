using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Controllers;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;
using System.Collections.Generic;
using System.Linq;
using Xbehave;
using Xunit;

namespace PlanningPoker2018_backend.Tests
{
    public class EstimatingRoomFeature
    {
        private DatabaseContext _context;

        public EstimatingRoomFeature()
        {
            SetUpTest();
        }

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
            "And having the task in database"
                .x(() =>
                {
                    _context.ProjectTask.Add(task);
                    _context.SaveChanges();
                    _context.Entry(task).State = EntityState.Detached;
                });

            "When I select task"
                .x(async () =>
                {
                    await new ProjectTasksController(_context).UpdateTaskStatus(task.id, TaskStatus.VOTING.name);
                });

            "Then task status is 'Voting'"
                .x(() => {
                    task = _context.ProjectTask.First(pt => pt.id == task.id);
                    Assert.Equal(TaskStatus.VOTING.name, task.status);
                });
        }

        [Scenario]
        public void EstimatingATask(ProjectTask task)
        {
            var controller = new ProjectTasksController(_context);
            "Given a new task"
                .x(() => task = new ProjectTask()
                {
                    id = 2,
                    status = TaskStatus.UNESTIMATED.name,
                    title = "New task in BDD",
                    RoomId = 1999,
                    estimate = 0
                });
            "And adding the task in database"
                .x(async () =>
                {
                    await controller.CreateProjectTask(task);
                    _context.Entry(task).State = EntityState.Detached;
                });
            "When I select task to estimate"
                .x(async () =>
                {
                    await controller.UpdateTaskStatus(task.id, TaskStatus.VOTING.name);
                });
            "And assign estimate"
                .x(async () =>
                {
                    await controller.PatchProjectTaskEstimate(task.id, 5);
                });
            "Then task status is 'Estimated'"
                .x(() =>
                {
                    task = _context.ProjectTask.First(pt => pt.id == task.id);
                    Assert.Equal(5, task.estimate);
                });
            "And esitmate is equal to assigned one"
                .x(() => 
                {
                    Assert.Equal(TaskStatus.ESTIMATED.name, task.status);
                });
        }

        [Scenario]
        public void GettingSummaryOfEstimatingRoom(List<ProjectTask> tasks, List<RoomParticipant> participants, List<User> users)
        {
            "Given a list of users"
                .x(() =>
                {

                });
            "And list of tasks"
                .x(() =>
                {
                });
            "And having users assigned to the room"
                .x(() =>
                {

                });
        }

        private void SetUpTest()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("Planning-Poker-DB");
            _context = new DatabaseContext(builder.Options);
        }
    }
}