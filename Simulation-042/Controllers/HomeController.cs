using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simulation_042.Context;
using Simulation_042.ViewModels.ChefViewModels;
using System.Diagnostics;

namespace Simulation_042.Controllers
{
    public class HomeController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var chefs = await _context.Chefs.Select(x => new ChefGetVM()
            {
                Id = x.Id,
                Name = x.Name,
                ImagePath = x.ImagePath,
                CategoryName = x.Category.Name
            }).ToListAsync();
            return View(chefs);
        }
    }
}
