using Lanina.Public.Web.Api.Models;
using Lanina.Public.Web.Api.ThirdPartyClients.Google;
using Lanina.Public.Web.Api.ThirdPartyClients.Koplanet;
using Lanina.Public.Web.Api.ThirdPartyClients.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Lanina.Public.Web.Api
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var koplanetSettings = new KoplanetSettings();
            Configuration.Bind("KoplanetSettings", koplanetSettings);
            services.AddSingleton(koplanetSettings);
            services.AddSingleton<KoplanetClient>();

            var googleCalendarSettings = new GoogleCalendarSettings();
            Configuration.Bind("GoogleCalendarSettings", googleCalendarSettings);
            services.AddSingleton(googleCalendarSettings);
            services.AddSingleton<GoogleCalendarClient>();

            var googleReCaptchaSettings = new GoogleReCaptchaSettings();
            Configuration.Bind("GoogleReCaptchaSettings", googleReCaptchaSettings);
            services.AddSingleton(googleReCaptchaSettings);
            services.AddSingleton<GoogleReCaptchaClient>();

            var mailSettings = new MailSettings();
            Configuration.Bind("MailSettings", mailSettings);
            services.AddSingleton(mailSettings);
            services.AddSingleton<MailClient>();

            services.AddSingleton<LaNinaInterviewManager>();

            services.Configure<AdminSettings>(Configuration.GetSection("AdminSettings"));            

            services.AddEntityFrameworkSqlite()
            .AddDbContext<LaNinaApplicantDbContext>(
                options => options.UseSqlite($"Data Source={_hostingEnvironment.ContentRootPath}/Databases/applicants.db"));

            services.AddEntityFrameworkSqlite()
            .AddDbContext<LaNinaFlagDbContext>(
                options => options.UseSqlite($"Data Source={_hostingEnvironment.ContentRootPath}/Databases/flags.db"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        // Use exceptionHandlerPathFeature to process the exception (for example, 
                        // logging), but do NOT expose sensitive error information directly to 
                        // the client.

                        await context.Response.WriteAsync($"{exceptionHandlerPathFeature?.Error.ToString()}<br><br>\r\n");



                        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                    });
                });
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
