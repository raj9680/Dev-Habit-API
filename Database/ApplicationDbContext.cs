using DevHabit.API.Database.Configurations;
using DevHabit.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Database
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
    {
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<HabitTag> HabitTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schemas.Application);

            // conf wil apply
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }

    public static class Schemas
    {
        public const string Application = "dev_habit";
    }
}
