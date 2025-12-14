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
            if (ModelState.IsValid) // eğer tüm alanlar doğru girilmişse 
            {
                _context.Add(salon);     //veritababıba ekliyoruz.
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salon); // salon modelini geri döndürür create sayfasına
        }
        



        // GET: Salon/Delete/5 (Silme)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null) return NotFound();
            return View(salon);
        }



        /// POST: Salon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             // Salonu, içindeki Antrenörlerle birlikte getir 
             var salon = await _context.Salonlar
                .Include(s => s.Antrenorler) 
               .FirstOrDefaultAsync(s => s.Id == id);

            if (salon != null)
            {
                  if (salon.Antrenorler != null)
                {
                  foreach (var antrenor in salon.Antrenorler)
                      {
                      antrenor.SalonId = null; // Antrenör silinmez, sadece salonsuz kalır
                  }
                }

                _context.Salonlar.Remove(salon);
                  await _context.SaveChangesAsync();
            }   
    
            return RedirectToAction(nameof(Index));
        }
    }
}