using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // şimdilik kapatabilirsin
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // --- Listeleme ---
        public async Task<IActionResult> Index()
        {
            var users = await _db.AppUsers
                .Include(u => u.Role)
                .AsNoTracking()
                .ToListAsync();
            return View(users);
        }

        // --- Kullanıcı Ekle (GET) ---
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View();
        }

        // --- Kullanıcı Ekle (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync(model.RoleId);
                return View(model);
            }

            _db.AppUsers.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- Düzenle (GET) ---
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.AppUsers.FindAsync(id);
            if (user == null) return NotFound();

            await LoadRolesAsync(user.RoleId);
            return View(user);
        }

        // --- Düzenle (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppUser model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadRolesAsync(model.RoleId);
                return View(model);
            }

            var user = await _db.AppUsers.FindAsync(id);
            if (user == null) return NotFound();

            // Güncelleme
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.RoleId = model.RoleId;

            // Şifre boş değilse güncelle
            if (!string.IsNullOrWhiteSpace(model.Password))
                user.Password = model.Password; // burada hashleme yapmanı tavsiye ederim!

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- Sil (GET) ---
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.AppUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();
            return View(user);
        }

        // --- Sil (POST) ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _db.AppUsers.FindAsync(id);
            if (user == null) return NotFound();

            _db.AppUsers.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Roller dropdown için yardımcı metot
        private async Task LoadRolesAsync(int? selectedRoleId = null)
        {
            var roles = await _db.Roles.AsNoTracking().ToListAsync();
            ViewBag.RoleList = new SelectList(roles, "Id", "Name", selectedRoleId);
        }
    }
}
