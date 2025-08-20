using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public BlogsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db; _env = env;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var blogs = await _db.Blogs.AsNoTracking()
                                       .OrderByDescending(b => b.Id)
                                       .ToListAsync();
            return View(blogs);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create() => View();

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog model, IFormFile? ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);

                    var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    if (!allowed.Contains(ext))
                    {
                        ViewBag.Error = "Sadece jpg, jpeg, png, gif, webp kabul edilir.";
                        return View(model);
                    }

                    var fileName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(uploads, fileName);
                    using var fs = new FileStream(filePath, FileMode.Create);
                    await ImageFile.CopyToAsync(fs);
                    model.ImagePath = "/uploads/" + fileName;
                }

                _db.Blogs.Add(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        // DELETE GET (onay)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _db.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        // DELETE POST (onaylandı)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            // Görseli sil
            if (!string.IsNullOrWhiteSpace(blog.ImagePath))
            {
                var physical = Path.Combine(_env.WebRootPath,
                    blog.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physical))
                    System.IO.File.Delete(physical);
            }

            _db.Blogs.Remove(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
