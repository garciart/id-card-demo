using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IDCardDemo {
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Create roles
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
                await roleManager.CreateAsync(new IdentityRole("Manager"));
                await roleManager.CreateAsync(new IdentityRole("Sponsor"));

                // Assign roles to users
                var user = await userManager.FindByNameAsync("ADMIN");
                var role = await roleManager.FindByNameAsync("Administrator");
                await userManager.AddToRolesAsync(user, new[] { "Administrator", "Manager", "Sponsor" });

                user = await userManager.FindByNameAsync("MANAGER");
                role = await roleManager.FindByNameAsync("Manager");
                await userManager.AddToRolesAsync(user, new[] { "Manager", "Sponsor" });

                user = await userManager.FindByNameAsync("SPONSOR");
                role = await roleManager.FindByNameAsync("Sponsor");
                await userManager.AddToRolesAsync(user, new[] { "Sponsor" });
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
