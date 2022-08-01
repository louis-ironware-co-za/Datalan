using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.Services;
using Newtonsoft.Json;
using System.Globalization;

namespace IdentityManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.MachineName == "LJACOBS")
            services.AddDbContext<ApplicationDbContext>(options =>
                          options.UseSqlServer(Configuration.GetConnectionString("DevLJ")));
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                         options.UseSqlServer(Configuration.GetConnectionString("Dev")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.Use(async (context, next) =>
            //{
            //    var culture = CultureInfo.CurrentCulture.Clone() as CultureInfo;// Set user culture here

            //    NumberFormatInfo nfi = new NumberFormatInfo();
            //    nfi.NumberDecimalSeparator = ".";
            //    nfi.CurrencyGroupSeparator = " ";
            //    nfi.CurrencySymbol = "X";
            //    culture.NumberFormat = nfi;

            //    CultureInfo.CurrentCulture = culture;
            //    CultureInfo.CurrentUICulture = culture;

            //    // Call the next delegate/middleware in the pipeline
            //    await next();
            //});
        }
    }
}
