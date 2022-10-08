using System;
using System.Collections.Generic;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder appBuilder)
        {
            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();

            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(serviceScope.ServiceProvider.GetRequiredService<ICommandsService>(), platforms);
        }

        private static void SeedData(ICommandsService repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms ...");

            foreach(var plat in platforms)
            {
                if (!repo.ExternalPlatformExist(plat.ExternalID)) repo.CreatePlatform(plat);

                repo.SaveChanges();
            }
        }
    }

 
}
