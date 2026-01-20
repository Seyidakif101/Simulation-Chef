using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Simulation_042.Context;
using Simulation_042.Helper;
using Simulation_042.Models;
using System.Threading.Tasks;

namespace Simulation_042
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<DbContextInitalizer>();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            var app = builder.Build();
            var scope = app.Services.CreateScope();
            var contextInitalizer = scope.ServiceProvider.GetRequiredService<DbContextInitalizer>();
            await contextInitalizer.InitalizerRole();



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                 name: "areas",
                 pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
