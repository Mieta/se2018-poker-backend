namespace PlanningPoker2018_backend.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PlanningPokerRemoteContext : DbContext
    {
        public PlanningPokerRemoteContext()
            : base("name=PlanningPokerRemoteContext")
        {
        }

        public virtual DbSet<Models.User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
