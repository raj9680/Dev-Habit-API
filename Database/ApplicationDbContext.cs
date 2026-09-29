using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Database
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schemas.Application);
            base.OnModelCreating(modelBuilder);
        }
    }

    public static class Schemas
    {
        public const string Application = "dev_habit";
    }
}
