using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IDCardDemo.Areas.Identity.IdentityHostingStartup))]
namespace IDCardDemo.Areas.Identity {
    public class IdentityHostingStartup : IHostingStartup {
        public void Configure(IWebHostBuilder builder) {
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