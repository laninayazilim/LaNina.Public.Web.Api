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

                if (!dbContext.InterviewDates.Any())
                {
                    var date = new DateTime(2019, 9, 2, 10, 0, 0);
                    
                    while (date.Month == 9)
                    {
                        if (date.Hour == 13 
                            || date.Hour > 20 
                            || date.Hour < 10
                            || date.DayOfWeek == DayOfWeek.Saturday
                            || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            date = date.AddHours(1);
                            continue;
                        }

                        dbContext.InterviewDates.Add(new InterviewDate
                        {
                            Date = date,
                            Key = Guid.NewGuid().ToString()
                        });

                        date = date.AddHours(1);
                    }

                    dbContext.SaveChanges();
                }
            }

            return webHost;
        }
    }
}
