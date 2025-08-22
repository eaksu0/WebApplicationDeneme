using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationDeneme.Data;
using WebApplicationDeneme.Models;

namespace WebApplicationDeneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class WorksController : Controller
    {
        private readonly AppDbContext _db;
        public WorksController(AppDbContext db) => _db = db;

        // LIST
        public async Task<IActionResult> Index()
        {
            var works = await _db.Works.AsNoTracking().ToListAsync();
            return View(works);
        }

        // CREATE GET
        public IActionResult Create() => View(new Work());

        // CREATE POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Work model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Works.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var work = await _db.Works.FindAsync(id);
            if (work == null) return NotFound();
            return View(work);
        }

        // EDIT POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Work model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _db.Works.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var work = await _db.Works.FindAsync(id);
            if (work == null) return NotFound();
            return View(work);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var work = await _db.Works.FindAsync(id);
            if (work == null) return NotFound();
            _db.Works.Remove(work);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
