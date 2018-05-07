using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Controllers;
using PlanningPoker2018_backend_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PlanningPoker2018_backend.Tests
{
    public class RoomsControllerUnitTests
    {
        public RoomsControllerUnitTests()
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
        public void Get_ShouldReturnTwoRooms()
        {
            ClearRoomsFromDatabase();
            var controller = new RoomsController(context);
            context.Room.Add(new Room {id = 1, name = "Pokój 1", link = "https://localhost:4200/room/1" });
            context.Room.Add(new Room { id = 2, name = "Pokój 2", link = "https://localhost:4200/room/2" });
            context.SaveChanges();
            IEnumerable<Room> result = controller.GetRooms();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async void Put_ShouldAddNewRoom()
        {
            ClearRoomsFromDatabase();
            var controller = new RoomsController(context);
            var room = new Room { id = 1, name = "Pokój œwie¿o dodany" };
            var result = await controller.PutRoom(room);
            var typeResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(typeResult.StatusCode);
            Assert.Equal(201, typeResult.StatusCode.Value);
            Assert.Contains(context.Room, p => p.name.Equals("Pokój œwie¿o dodany") && p.id.Equals(1));
        }

        private void ClearRoomsFromDatabase()
        {
            foreach (var room in context.Room)
                context.Room.Remove(room);
            context.SaveChanges();
        }
    }
}
