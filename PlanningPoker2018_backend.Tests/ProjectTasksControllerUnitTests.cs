using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Controllers;
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
            var controller = new RoomsController(context);
            var room = new Room { id = 1, name = "Pokój świeżo dodany" };
            var result = await controller.PutRoom(room);
            var typeResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(typeResult.StatusCode);
            Assert.Equal(201, typeResult.StatusCode.Value);
            Assert.Contains(context.Room, p => p.name.Equals("Pokój świeżo dodany") && p.id.Equals(1));
        }

        private void ClearTasksFromDatabase()
        {
            foreach (var task in context.ProjectTask)
                context.ProjectTask.Remove(task);
            context.SaveChanges();
        }
    }
}