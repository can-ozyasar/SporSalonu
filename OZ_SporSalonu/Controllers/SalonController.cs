using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salon (Listeleme)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Salonlar.ToListAsync());
        }

        // GET: Salon/Create (Ekleme Sayfası)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Salon/Create (Kaydetme)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salon salon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salon);
        }
        
        // GET: Salon/Delete/5 (Silme)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null) return NotFound();
            return View(salon);
        }

        // POST: Salon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salon = await _context.Salonlar.FindAsync(id);
            if (salon != null)
            {
                _context.Salonlar.Remove(salon);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}