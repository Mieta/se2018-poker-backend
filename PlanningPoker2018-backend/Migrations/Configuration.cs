namespace PlanningPoker2018_backend.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.PlanningPokerRemoteContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.PlanningPokerRemoteContext context)
        {
            context.Users.AddOrUpdate(x => x.Id,
                new Models.User() { Id = 1, Name = "Test user"}
                );
        }
    }
}
