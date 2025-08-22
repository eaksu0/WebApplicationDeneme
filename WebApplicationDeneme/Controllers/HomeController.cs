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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _db.Blogs
                                 .OrderByDescending(b => b.CreatedAt)
                                 .ToListAsync();

            ViewBag.References = await _db.References.AsNoTracking()
                                       .OrderByDescending(r => r.Id)
                                       .ToListAsync();

            return View(blogs); // mevcut modelin blog listesi kalabilir
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
    }
}
