using Microsoft.EntityFrameworkCore;

namespace DevHabit.API.Database.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            try
            {
                await dbContext.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied");
            }
            catch (Exception ex)
            {
                app.Logger.LogError("An error occured while applying migrations");
                throw;
            }
        }
    }
}
