using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformService.Data
{
    public static class PrepDb
    {
       
        public static void PrePopulation(IApplicationBuilder app, bool isProd)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            AppDbContext context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            SeedData(context, isProd);
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            // if (isProd)
            // {
            //  Console.WriteLine("Migration");
            // try
            // {
            // context.Database.Migrate();
            // }
            //   catch (Exception ex)
            // {
            //Console.WriteLine($"Could not run migration {ex.Message}");
            // }

            // context.Database.Migrate();
            // }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seed data.");

                context.Platforms.AddRange(
                    new Models.Platform()
                    {
                        Name = ".Net",
                        Publisher = "Microsoft",
                        Cost = "free"
                    },
                     new Models.Platform()
                     {
                         Name = "SQL Server",
                         Publisher = "Microsoft",
                         Cost = "free"
                     },
                      new Models.Platform()
                      {
                          Name = "Docker",
                          Publisher = "Docker CloudNative Computing",
                          Cost = "free"
                      });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Already have data.");
            }
        }
    }
}
