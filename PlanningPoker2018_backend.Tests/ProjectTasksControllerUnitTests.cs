using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Controllers;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;
using Xunit;

namespace PlanningPoker2018_backend.Tests
{
    public class ProjectTasksControllerUnitTests
    {
        public ProjectTasksControllerUnitTests()
        {
            InitializeContext();
        }

        private DatabaseContext context;

        private void InitializeContext()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("Planning-Poker-DB");
            builder.EnableSensitiveDataLogging();
            context = new DatabaseContext(builder.Options);
        }

        [Fact]
        public void Get_ShouldReturnTwoTasks()
        {
            ClearTasksFromDatabase();
            var controller = new ProjectTasksController(context);
            context.ProjectTask.Add(new ProjectTask
            {
                id = 1,
                title = "Zadanie 1",
                author = new User(),
                RoomId = 1,
                estimate = 5
            });
            context.ProjectTask.Add(new ProjectTask
            {
                id = 2,
                title = "Zadanie 2",
                author = new User(),
                RoomId = 1,
                estimate = 5
            });
            context.SaveChanges();
            var projectTasks = controller.GetProjectTasks();
            Assert.Equal(2, projectTasks.Count());
        }

        [Fact]
        public async void Put_ShouldAddNewTask()
        {
            ClearTasksFromDatabase();
            var controller = new ProjectTasksController(context);
            var task = new ProjectTask { title = "Zadanie testowe", RoomId = 1, status = TaskStatus.UNESTIMATED.name };
            var result = await controller.CreateProjectTask(task);
            var typeResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(typeResult.StatusCode);
            Assert.Equal(201, typeResult.StatusCode.Value);
            Assert.Contains(context.ProjectTask, p => p.title.Equals("Zadanie testowe") && p.id.Equals(task.id));
        }

        [Fact]
        public async void Post_ShouldChangeTaskStatusToVoting()
        {
            ClearTasksFromDatabase();
            var task = new ProjectTask() { title = "Zadanie testowe", RoomId = 1, status = TaskStatus.UNESTIMATED.name };
            context.ProjectTask.Add(task);
            context.SaveChanges();
            context.Entry(task).State = EntityState.Detached;

            var controller = new ProjectTasksController(context);
            var result = await controller.UpdateTaskStatus(task.id, TaskStatus.VOTING.name);
            var updatedTask = context.ProjectTask.First(t => t.id == task.id);
            var typeResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, typeResult.StatusCode);
            Assert.Equal(TaskStatus.VOTING.name, updatedTask.status);
        }


        [Fact]
        public async void Patch_ShouldChangeTaskEstimateTo_5()
        {
            ClearTasksFromDatabase();
            var taskId = 1;
            var expectedEstimate = 5;
            var projectTask = new ProjectTask()
            {
                id = taskId,
                title = "test task 1",
                author = new User(),
                RoomId = 1,
                estimate = 1
            };
            context.ProjectTask.Add(projectTask);
            context.SaveChanges();
            context.Entry(projectTask).State = EntityState.Detached;
            
            var controller = new ProjectTasksController(context);
            var result = await controller.PatchProjectTaskEstimate(taskId, expectedEstimate);
            var projectTaskEstimate = context.ProjectTask.Find(taskId).estimate;
            Assert.Equal(expectedEstimate, projectTaskEstimate);
        }

        private void ClearTasksFromDatabase()
        {
            foreach (var task in context.ProjectTask)
                context.ProjectTask.Remove(task);
            context.SaveChanges();
        }
    }
}