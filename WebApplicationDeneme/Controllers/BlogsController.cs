using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Controllers
{
    [AllowAnonymous]
    public class BlogsController : Controller
    {
        private readonly AppDbContext _db;
        public BlogsController(AppDbContext db) => _db = db;

        // /Blogs?q=arama&page=1
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 9)
        {
            var query = _db.Blogs.AsNoTracking().OrderByDescending(b => b.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(b =>
                    b.Title.Contains(q) ||
                    (b.ShortText ?? "").Contains(q) ||
                    (b.LongText ?? "").Contains(q));
            }

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items);
        }

        // /Blogs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _db.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null) return NotFound();

            // Yan panel / öneriler için 6 adet başka blog
            ViewBag.Related = await _db.Blogs
                .AsNoTracking()
                .Where(b => b.Id != id)
                .OrderByDescending(b => b.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(blog);
        }
    }
}
