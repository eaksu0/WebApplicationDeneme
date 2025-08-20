using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]// istersen şimdilik açık bırakabilirsin, giriş şartı için güzel
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var users = await _db.AppUsers.Include(u => u.Role).AsNoTracking().ToListAsync();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppUser model)
        {
            _db.AppUsers.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.AppUsers.FindAsync(id);
            if (user == null) return NotFound();
            await LoadRolesAsync(user.RoleId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AppUser model)
        {
            if (id != model.Id) return BadRequest();
            _db.AppUsers.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.AppUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _db.AppUsers.FindAsync(id);
            if (user == null) return NotFound();
            _db.AppUsers.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRolesAsync(int? selectedRoleId = null)
        {
            var roles = await _db.Roles.AsNoTracking().ToListAsync();
            ViewBag.RoleList = new SelectList(roles, "Id", "Name", selectedRoleId);
        }
    }
}
