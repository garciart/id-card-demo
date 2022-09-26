using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using IDCardDemo.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace IDCardDemo
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
            string appDBSource = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"App_Data\app.db");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(string.Format("DataSource={0}", appDBSource)));
            string holdersDBSource = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"App_Data\Holders.db");
            services.AddDbContext<IDCardDemoContext>(options =>
                options.UseSqlite(string.Format("DataSource={0}", holdersDBSource)));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages(options => {
                options.Conventions.AuthorizeFolder("/Holders");
            });
            // Needed to allow the app to save files to the wwwroot
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions() {
                OnPrepareResponse = s => {
                    if ((
                    s.Context.Request.Path.StartsWithSegments(new PathString("/images")) ||
                    s.Context.Request.Path.StartsWithSegments(new PathString("/photos")) ||
                    s.Context.Request.Path.StartsWithSegments(new PathString("/temp"))) &&
                       !s.Context.User.Identity.IsAuthenticated) {
                        s.Context.Response.StatusCode = 401;
                        s.Context.Response.Body = Stream.Null;
                        s.Context.Response.ContentLength = 0;
                    }
                }
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapGet("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
                endpoints.MapPost("/Identity/Account/Register", context => Task.Factory.StartNew(() => context.Response.Redirect("/Identity/Account/Login", true, true)));
            });
        }
    }
}
