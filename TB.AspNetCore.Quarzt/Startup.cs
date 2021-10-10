using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jetsun.AspNetCore.QuartzJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using TB.AspNetCore.Data.Entity;
using TB.AspNetCore.Quarzt.CompileProfile;

namespace TB.AspNetCore.Quarzt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static string _testDbConnectionString = "Data Source=GUOSHAOYUE-5040;Initial Catalog=Test;Persist Security Info=True;User ID=sa;Password=jetsun";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "scheduler.db");
            services.AddDbContext<SchedulerDbContext>(options => options.UseSqlite($"Data Source={path}"));
            //services.AddDbContext<SchedulerDbContext>(options => options.UseMySql(@"server=localhost;database=test_db;uid=root;pwd=jetsun;"));
            
            services.AddDbContext<TestDBContext>(options =>
            {
                options
                    .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                    .UseSqlServer(_testDbConnectionString)
                    .AddInterceptors(new QueryWithNoLockDbCommandInterceptor());
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.Configure<MvcOptions>(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Added - uses IOptions<T> for your settings.
            services.AddOptions();

            // Added - Confirms that we have a home for our DemoSettings
            services.Configure<BASE_PATHS>(Configuration.GetSection("BASE_PATHS"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
