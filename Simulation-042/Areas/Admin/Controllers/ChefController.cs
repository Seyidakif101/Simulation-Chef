using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Simulation_042.Context;
using Simulation_042.Helper;
using Simulation_042.Models;
using Simulation_042.ViewModels.ChefViewModels;

namespace Simulation_042.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChefController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _folderPath;

        public ChefController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _folderPath = Path.Combine(_environment.WebRootPath, "images");
        }
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

        public async Task<IActionResult> Create()
        {
            await _sendCategoryViewBag();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ChefCreateVM vm)
        {
            await _sendCategoryViewBag();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);
            if (!existCategory)
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category Yoxdur");
                return View(vm);
            }
            if (!vm.Image.CheckSize(2))
            {
                ModelState.AddModelError("Image", "2mb boyuyk olmamalidi");
                return View(vm);

            }
            if (!vm.Image.CheckType("image"))
            {
                ModelState.AddModelError("Image", "Image tipinde olmalidi file");
                return View(vm);
            }
            string uniqueFileName = await vm.Image.FileUploadAsync(_folderPath);
            Chef chef = new()
            {
                Name = vm.Name,
                ImagePath = uniqueFileName,
                CategoryId = vm.CategoryId
            };
            await _context.Chefs.AddAsync(chef);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var chef = await _context.Chefs.FindAsync(id);
            if (chef is null)
            {
                return NotFound();
            }
            ChefUpdateVM vm = new()
            {
                Name = chef.Name,
                CategoryId = chef.CategoryId
            };
            await _sendCategoryViewBag();
            return View(vm);
        }
        public async Task<IActionResult> Update(ChefUpdateVM vm)
        {
            await _sendCategoryViewBag();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var existCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);
            if (!existCategory)
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category Yoxdur");
                return View(vm);
            }
            if (!vm.Image?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("Image", "2mb boyuyk olmamalidi");
                return View(vm);

            }
            if (!vm.Image?.CheckType("image") ?? false)
            {
                ModelState.AddModelError("Image", "Image tipinde olmalidi file");
                return View(vm);
            }
            var existChef = await _context.Chefs.FindAsync(vm.Id);
            existChef.Name = vm.Name;
            existChef.CategoryId = vm.CategoryId;
            if (vm.Image is { })
            {
                string uniqueFileName = await vm.Image.FileUploadAsync(_folderPath);
                string oldFileName = Path.Combine(_folderPath, existChef.ImagePath);
                FileHelper.FileDelete(oldFileName);
                existChef.ImagePath = uniqueFileName;
            }
            _context.Chefs.Update(existChef);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var chef = await _context.Chefs.FindAsync(id);
            if (chef is null)
            {
                return NotFound();
            }
            _context.Chefs.Remove(chef);
            await _context.SaveChangesAsync();
            string deleteFileName = Path.Combine(_folderPath, chef.ImagePath);
            FileHelper.FileDelete(deleteFileName);
            return RedirectToAction(nameof(Index));

        }
        private async Task _sendCategoryViewBag()
        {
            var categories = await _context.Categories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToListAsync();
            ViewBag.Categories = categories;
        }
    }
}

