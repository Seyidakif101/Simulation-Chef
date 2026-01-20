using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Simulation_042.Enums;
using Simulation_042.Models;
using Simulation_042.ViewModels.AccountViewModels;

namespace Simulation_042.Helper
{
    public class DbContextInitalizer
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AdminVM _admin;

        public DbContextInitalizer(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _admin = _configuration.GetSection("AdminSettings").Get<AdminVM>() ?? new();

        }
        public async Task InitalizerRole()
        {
            await CreateAdmin();
            await CreateRole();
        }
        private async Task CreateAdmin()
        {
            AppUser user = new()
            {
                UserName=_admin.UserName,
                Email=_admin.Email,
            };
            var result = await _userManager.CreateAsync(user, _admin.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RolesEnum.Admin.ToString());
            }
        }
        private async Task CreateRole()
        {
            foreach(var role in Enum.GetNames(typeof(RolesEnum)))
            {
                await _roleManager.CreateAsync(new()
                {
                    Name = role
                });
            }
        }
    }
}
