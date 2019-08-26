using Lanina.Public.Web.Api.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Lanina.Public.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .SeedData()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public static class IWebHostExtensions
    {
        public static IWebHost SeedData(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<LaNinaApplicantDbContext>();
                if (!dbContext.Flags.Any())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        dbContext.Flags.Add(new Flag
                        {
                            Value = Guid.NewGuid().ToString()
                        });
                    }

                    dbContext.SaveChanges();
                }
            }

            return webHost;
        }
    }
}
