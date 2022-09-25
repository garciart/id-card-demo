using System;
using IDCardDemo.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IDCardDemo.Areas.Identity.IdentityHostingStartup))]
namespace IDCardDemo.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            /*
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IDCardDemoIdentityDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("IDCardDemoIdentityDbContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<IDCardDemoIdentityDbContext>();
            });
            */
        }
    }
}