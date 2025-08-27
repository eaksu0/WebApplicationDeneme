using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SlidersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SlidersController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: Admin/Sliders
        public async Task<IActionResult> Index()
        {
            var list = await _db.Sliders
                .OrderBy(s => s.SortOrder)
                .ThenByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(list);
        }

        // GET: Admin/Sliders/Create
        public IActionResult Create() => View();

        // POST: Admin/Sliders/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider model, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                model.ImagePath = await SaveImageAsync(image);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.Sliders.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Sliders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var s = await _db.Sliders.FindAsync(id);
            if (s == null) return NotFound();
            return View(s);
        }

        // POST: Admin/Sliders/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slider model, IFormFile? image)
        {
            if (id != model.Id) return BadRequest();

            var s = await _db.Sliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            if (image != null && image.Length > 0)
            {
                // eski resmi sil (varsa)
                if (!string.IsNullOrWhiteSpace(s.ImagePath))
                    DeleteImageIfExists(s.ImagePath);

                model.ImagePath = await SaveImageAsync(image);
            }
            else
            {
                // resim yüklenmediyse eskisini koru
                model.ImagePath = s.ImagePath;
            }

            if (!ModelState.IsValid) return View(model);

            _db.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Sliders.FindAsync(id);
            if (s == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(s.ImagePath))
                DeleteImageIfExists(s.ImagePath);

            _db.Sliders.Remove(s);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Sliders/ToggleActive/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var s = await _db.Sliders.FindAsync(id);
            if (s == null) return NotFound();
            s.IsActive = !s.IsActive;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private async Task<string> SaveImageAsync(IFormFile file)
        {
            // wwwroot/assets/img/slider
            var root = Path.Combine(_env.WebRootPath, "assets", "img", "slider");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var name = $"{Guid.NewGuid():N}{ext}";
            var full = Path.Combine(root, name);

            using (var fs = new FileStream(full, FileMode.Create))
                await file.CopyToAsync(fs);

            // Razor'da kullanılacak sanal yol
            return $"~/assets/img/slider/{name}";
        }

        private void DeleteImageIfExists(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            var rel = path.Replace("~/", "").Replace("/", Path.DirectorySeparatorChar.ToString());
            var full = Path.Combine(_env.WebRootPath, rel);
            if (System.IO.File.Exists(full))
                System.IO.File.Delete(full);
        }
    }
}
