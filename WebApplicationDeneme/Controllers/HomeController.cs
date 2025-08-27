using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // BLOG LÝSTESÝ (view tarafýnda .Take(3) yapýyorsun; burada gerek yok ama istersen .Take(6) ekleyebilirsin)
            var blogs = await _db.Blogs
                                 .AsNoTracking()
                                 .OrderByDescending(b => b.CreatedAt)
                                 .ToListAsync();

            // SLIDER (sadece aktifleri, sýraya göre)
            ViewBag.Sliders = await _db.Sliders
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ThenByDescending(s => s.CreatedAt)
                .Select(s => new
                {
                    s.ImagePath,
                    s.Heading,
                    s.Text,
                    s.LinkText,
                    s.LinkUrl
                })
                .ToListAsync();

            // REFERANSLAR
            ViewBag.References = await _db.References
                .AsNoTracking()
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return View(blogs);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> About()
        {
            var team = await _db.TeamMembers
                                .AsNoTracking()
                                .OrderBy(t => t.DisplayOrder)
                                .ToListAsync();

            return View(team);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
