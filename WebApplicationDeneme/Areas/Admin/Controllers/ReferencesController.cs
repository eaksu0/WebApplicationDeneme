using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReferencesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ReferencesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Listeleme
        public async Task<IActionResult> Index()
        {
            var list = await _db.References.AsNoTracking()
                                           .OrderByDescending(r => r.Id)
                                           .ToListAsync();
            return View(list);
        }

        // Yeni ekleme GET
        [HttpGet]
        public IActionResult Create() => View(new Reference());

        // Yeni ekleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reference model, IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Ad zorunludur.");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ViewBag.Error = "Sadece jpg, jpeg, png, gif, webp kabul edilir.";
                    return View(model);
                }

                // uploads kökü (Program.cs ile uyumlu)
                var uploadsRoot = Path.Combine(_env.ContentRootPath, "AppUploads");
                Directory.CreateDirectory(uploadsRoot);

                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(stream);

                model.ImagePath = "/uploads/" + fileName; // istemci yolu
            }

            _db.References.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Silme GET (emin misiniz sayfası)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var reference = await _db.References.FirstOrDefaultAsync(r => r.Id == id);
            if (reference == null) return NotFound();
            return View(reference);
        }

        // Silme POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reference = await _db.References.FindAsync(id);
            if (reference == null) return NotFound();

            // Fiziksel resmi sil
            if (!string.IsNullOrWhiteSpace(reference.ImagePath))
            {
                // ImagePath: /uploads/xxxx.ext
                var physicalPath = Path.Combine(_env.ContentRootPath, "", Path.GetFileName(reference.ImagePath));
                if (System.IO.File.Exists(physicalPath))
                    System.IO.File.Delete(physicalPath);
            }

            _db.References.Remove(reference);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
