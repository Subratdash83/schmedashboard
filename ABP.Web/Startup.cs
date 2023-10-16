using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ABP.Domain.Login.OTP;
using ABP.Infrastructure;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.SMSTemplate;
using ABP.Web.DIContainer;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage.SQLite;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ABP.Web
{
    [Obsolete]
    public class Startup
    {
        private static IDistrictRepository _DistrictRepository;

        private static IBlockRepository _blockRepository;
        private static IDistrictDataRepository _districtDataRepository;
        private static IIndicatorRepository _indicatorRepository;
        private static ISendSMSRepository _sms;
        private static ISMSTemplateRepository _SMSTemplateRepository;
        private static IBlockDataRepository _blockDataRepository;
        private static Job jobscheduler = new Job(_DistrictRepository, _blockRepository, _districtDataRepository, _indicatorRepository, _sms, _SMSTemplateRepository, _blockDataRepository);
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            string hangfireConnectionString = Configuration.GetConnectionString("ABPConnectionString");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddCustomContainer(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Adding Session middleware
            GlobalConfiguration.Configuration.UseMemoryStorage();
            services.AddHangfire(configuration => { configuration.UseMemoryStorage(); });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.Name = ".ABP.Session";
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddMvc();
            //    services.AddRazorPages()
            //.AddRazorRuntimeCompilation();
            //    //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //    services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            //services.Configure<SmsSetting>(Configuration.GetSection("TwilioSetting"));
            //services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager , IServiceProvider serviceProvider)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            
            app.UseHttpsRedirection();

            //***Hangfire Job fire ****//
            var options = new SQLiteStorageOptions();
            GlobalConfiguration.Configuration.UseSQLiteStorage("SqlconnectionString", options);
            //        GlobalConfiguration.Configuration.UseStorage(
            //new OracleStorage(connectionString));
            var option = new BackgroundJobServerOptions
            {
                WorkerCount = Environment.ProcessorCount * 4
            };

            app.UseHangfireServer(option);
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            backgroundJobClient.Enqueue(() => Console.WriteLine("Hello HangFire Job!"));
            //recurringJobManager.AddOrUpdate("Insert in Analytic Table Monthly", () => jobscheduler.JobAsync(), Cron.Weekly(DayOfWeek.Thursday, 17, 10), TimeZoneInfo.Local);
            //recurringJobManager.AddOrUpdate("Insert in Analytic Table Yearly", () => jobscheduler.JobAsyncYearly(), Cron.Weekly(DayOfWeek.Tuesday, 12, 24), TimeZoneInfo.Local);
            recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Monthly", () => jobscheduler.JobIndScoreCalculation(), Cron.Daily(11,50), TimeZoneInfo.Local);
            recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => jobscheduler.JobIndScoreCalculationYearly(), Cron.Daily(21, 30), TimeZoneInfo.Local);
            //recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => jobscheduler.JobAsyncsms(), Cron.Daily(16, 10), TimeZoneInfo.Local);
            //recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => jobscheduler.JobAsyncsms(), Cron.Monthly(22, 16,47), TimeZoneInfo.Local);
            //recurringJobManager.AddOrUpdate("Insert in Indicator Score Table Yearly", () => jobscheduler.JobAsyncsms(), Cron.Hourly, TimeZoneInfo.Local);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
          
            //app.UseRouting();
          
          //  RecurringJob.AddOrUpdate(19872.ToString(), () => jobscheduler.JobAsync(), Cron.Daily(09, 39));
         
           // backgroundJobClient.Schedule(() => jobscheduler.JobAsync(), TimeSpan.FromDays(5));
            //****Hangfire Job Fire****//
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSession();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });
            //app.UseRouting();

            //app.UseAuthorization();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Login}/{id?}");
            });
        }
    }
}
