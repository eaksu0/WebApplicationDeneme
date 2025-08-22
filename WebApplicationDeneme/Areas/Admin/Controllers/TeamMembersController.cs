using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeamMembersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public TeamMembersController(AppDbContext db, IWebHostEnvironment env)
        { _db = db; _env = env; }

        // LIST
        public async Task<IActionResult> Index()
        {
            var list = await _db.TeamMembers.AsNoTracking()
                                            .OrderBy(t => t.DisplayOrder)
                                            .ToListAsync();
            return View(list);
        }

        // CREATE GET
        [HttpGet] public IActionResult Create() => View(new TeamMember());

        // CREATE POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamMember model, IFormFile? photo)
        {
            if (!ModelState.IsValid) return View(model);

            if (photo is { Length: > 0 })
            {
                var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("PhotoPath", "Sadece jpg, jpeg, png, gif, webp kabul edilir.");
                    return View(model);
                }
                if (photo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PhotoPath", "Dosya boyutu en fazla 5 MB.");
                    return View(model);
                }

                var root = Path.Combine(_env.WebRootPath, "uploads", "team");
                Directory.CreateDirectory(root);
                var name = Guid.NewGuid() + ext;
                using var fs = new FileStream(Path.Combine(root, name), FileMode.Create);
                await photo.CopyToAsync(fs);

                model.PhotoPath = "/uploads/team/" + name;
            }

            _db.TeamMembers.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var m = await _db.TeamMembers.FindAsync(id);
            if (m == null) return NotFound();
            return View(m);
        }

        // EDIT POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeamMember model, IFormFile? photo)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var existing = await _db.TeamMembers.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.Surname = model.Surname;
            existing.Title = model.Title;
            existing.DisplayOrder = model.DisplayOrder;

            if (photo is { Length: > 0 })
            {
                var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("PhotoPath", "Sadece jpg, jpeg, png, gif, webp kabul edilir.");
                    return View(model);
                }
                if (photo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PhotoPath", "Dosya boyutu en fazla 5 MB.");
                    return View(model);
                }

                // eski resmi sil
                if (!string.IsNullOrWhiteSpace(existing.PhotoPath))
                {
                    var old = Path.Combine(_env.WebRootPath,
                        existing.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }

                var root = Path.Combine(_env.WebRootPath, "uploads", "team");
                Directory.CreateDirectory(root);
                var name = Guid.NewGuid() + ext;
                using var fs = new FileStream(Path.Combine(root, name), FileMode.Create);
                await photo.CopyToAsync(fs);
                existing.PhotoPath = "/uploads/team/" + name;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DELETE GET (confirm)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.TeamMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (m == null) return NotFound();
            return View(m);
        }

        // DELETE POST (confirmed)
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var m = await _db.TeamMembers.FindAsync(id);
            if (m == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(m.PhotoPath))
            {
                var p = Path.Combine(_env.WebRootPath,
                    m.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
            }

            _db.TeamMembers.Remove(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
