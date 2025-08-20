using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly AppDbContext _db;
        public RolesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
            => View(await _db.Roles.AsNoTracking().ToListAsync());
        [HttpGet]
        public IActionResult Create() => View(new Role());

        [HttpPost]
        public async Task<IActionResult> Create(Role model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ViewBag.Error = "Rol adı boş olamaz.";
                return View(model);
            }
            _db.Roles.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            return role is null ? NotFound() : View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Role model)
        {
            if (id != model.Id) return BadRequest();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ViewBag.Error = "Rol adı boş olamaz.";
                return View(model);
            }
            _db.Roles.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
            return role is null ? NotFound() : View(role);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role is null) return NotFound();
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
