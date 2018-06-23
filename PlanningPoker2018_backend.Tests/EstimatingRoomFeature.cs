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
        public void GettingSummaryOfEstimatingRoom(Room room, List<ProjectTask> tasks, List<RoomParticipant> participants, List<User> users, ProjectTasksController projectTasksController, GameSummary summary, RoomsController roomsController)
        {
            "Given a list of users"
                .x(() =>
                {
                    users = new List<User>()
                    {
                        new User(){ id = 1, username = "john.doe", mailAddress = "john.doe@company.com" },
                        new User(){ id = 2, username = "foo.bar", mailAddress = "foo.bar@company.com" },
                        new User(){ id = 3, username = "lorem.ipsum", mailAddress = "lorem.ipsum@company.com" },
                        new User(){ id = 4, username = "almighty.PO", mailAddress = "almighty.PO@company.com" },
                    };
                    _context.User.AddRange(users);
                    _context.SaveChanges();
                });
            "And a room"
                .x(() =>
                {
                    room = new Room() { hostMailAddress = users[3].mailAddress, id = 2999, name = "BDD Testing Room" };
                    _context.Room.Add(room);
                    _context.SaveChanges();
                });
            "And list of tasks"
                .x(() =>
                {
                    tasks = new List<ProjectTask>()
                    {
                        new ProjectTask() {id = 1, title = "Add a new task", RoomId = 2999, estimate = 0, status = TaskStatus.UNESTIMATED.name, author = users[3]},
                        new ProjectTask() {id = 2, title = "Assign estimates", RoomId = 2999, estimate = 0, status = TaskStatus.UNESTIMATED.name, author = users[3]},
                        new ProjectTask() {id = 3, title = "Sign in to an application", RoomId = 2999, estimate = 0, status = TaskStatus.UNESTIMATED.name, author = users[3]},
                        new ProjectTask() {id = 4, title = "Sign up to an application", RoomId = 2999, estimate = 0, status = TaskStatus.UNESTIMATED.name, author = users[3]}
                    };
                    _context.ProjectTask.AddRange(tasks);
                    _context.SaveChanges();
                    tasks.ForEach(t => _context.Entry(t).State = EntityState.Detached);
                });
            "And having users assigned to the room"
                .x(() =>
                {
                    participants = new List<RoomParticipant>();
                    users.ForEach(u =>
                    {
                        if (u.id != 4)
                        {
                            participants.Add(new RoomParticipant() { mailAddress = u.mailAddress, roomId = 2999 });
                        }
                    });
                    _context.RoomParticipant.AddRange(participants);
                    _context.SaveChanges();
                });
            "When I estimate all tasks"
                .x(async () =>
                {
                    projectTasksController = new ProjectTasksController(_context);
                    await projectTasksController.PatchProjectTaskEstimate(1, 1);
                    await projectTasksController.PatchProjectTaskEstimate(2, 10);
                    await projectTasksController.PatchProjectTaskEstimate(3, 5);
                    await projectTasksController.PatchProjectTaskEstimate(4, 20);
                });
            "And request room summary"
                .x(async () => 
                {
                    roomsController = new RoomsController(_context);
                    summary = await roomsController.GetGameSummary(2999);
                });
            "Then all tasks will be present"
                .x(() =>
                {
                    tasks = _context.ProjectTask.Where(pt => pt.RoomId == 2999).ToList();
                    tasks.ForEach(t =>
                    {
                        Assert.Contains(summary.tasks, (pt) => t.id == pt.id);
                    });
                });
            "And all tasks will be having correct estimates"
                .x(() =>
                {
                    tasks.ForEach(t =>
                    {
                        Assert.Contains(summary.tasks, (pt) => t.id == pt.id && t.estimate == pt.estimate);
                    });
                });
            "And all participants will be present"
                .x(() => 
                {
                    users.ForEach(u =>
                    {
                        if(u.mailAddress != summary.host)
                        {
                            Assert.Contains(summary.participants, rp => rp.id == u.id);
                        }
                    });
                });

        }

        private void SetUpTest()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("Planning-Poker-DB");
            _context = new DatabaseContext(builder.Options);
            foreach (var task in _context.ProjectTask)
                _context.ProjectTask.Remove(task);
            foreach (var user in _context.User)
                _context.User.Remove(user);
            foreach (var room in _context.Room)
                _context.Room.Remove(room);
            _context.SaveChanges();
        }
    }
}