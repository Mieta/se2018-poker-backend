﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningPoker2018_backend_2.Controllers;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Models;
using Xunit;

namespace PlanningPoker2018_backend.Tests
{
    public class UserControllerUnitTests
    {

        public UserControllerUnitTests()
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
        public async void Put_ShouldAddNewUser()
        {
            ClearUsersFromDatabase();
            var controller = new UsersController(context);
            var user = new User { id = 1, username = "John Doe", mailAddress = "john.doe@gmail.com", password = "password 123"};
            var result = await controller.PutNewUser(user);
            var typeResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(typeResult.StatusCode);
            Assert.Equal(201, typeResult.StatusCode.Value);
            Assert.Contains(context.User, p => p.username.Equals("John Doe") && p.id.Equals(1) && p.mailAddress.Equals("john.doe@gmail.com"));
        }

        [Fact]
        public async void Post_ShouldAuthenticateUser()
        {
            ClearUsersFromDatabase();
            var controller = new UsersController(context);
            var user = new User { id = 1, username = "John Doe", mailAddress = "john.doe@gmail.com", password = "password123"};
            context.User.Add(user);
            context.SaveChanges();

            var result =
                controller.AuthenticateUser(new AuthBody()
                {
                    mailAddress = "john.doe@gmail.com",
                    password = "password123"
                });
            
            var typeResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(typeResult.StatusCode);
            Assert.Equal(200, typeResult.StatusCode.Value);
        }
        
        private void ClearUsersFromDatabase()
        {
            foreach (var user in context.User)
                context.User.Remove(user);
            context.SaveChanges();
        }
    }
}