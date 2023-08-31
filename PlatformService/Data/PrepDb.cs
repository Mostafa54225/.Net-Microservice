using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool IsProduction)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool IsProduction)
        {
            if(IsProduction)
            {
                try
                {
                    Console.WriteLine("--> Attempting to apply migration");
                    context.Database.Migrate();
                } catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            if(!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data...");
                context.Platforms.AddRange(
                    new Platform() { Name = "Dotnet", Publisher="Microsoft", Cost="Free" },
                    new Platform() { Name = "Sql Server Express", Publisher="Microsoft", Cost="Free" },
                    new Platform() { Name = "Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free" }
                );
                context.SaveChanges();
            } 
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}