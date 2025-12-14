using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers
{
    // Sadece "Admin" rolündeki kullanıcılar erişebilir.
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler.ToListAsync();
            // Model'i ViewModel'e dönüştür
            var viewModels = hizmetler.Select(h => new HizmetViewModel
            {
                Id = h.Id,
                Ad = h.Ad,
                Aciklama = h.Aciklama,
                SureDakika = h.SureDakika,
                Ucret = h.Ucret
            }).ToList();
            
            return View(viewModels); // ViewModel listesini hizmet/index View'a gönderiyoruz ki görüntülenebilsin
        }

        


        public IActionResult Create() // Ekleme Sayfası hizmet/crate
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HizmetViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                
                var hizmet = new Hizmet
                {
                    Ad = viewModel.Ad,
                    Aciklama = viewModel.Aciklama,
                    SureDakika = viewModel.SureDakika,
                    Ucret = viewModel.Ucret
                };

                _context.Add(hizmet); // formdan aldığımız hizmeti veritabanına ekliyoruz.
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null) return NotFound();

            
            var viewModel = new HizmetViewModel
            {
                Id = hizmet.Id,
                Ad = hizmet.Ad,
                Aciklama = hizmet.Aciklama,
                SureDakika = hizmet.SureDakika,
                Ucret = hizmet.Ucret
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HizmetViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var hizmet = await _context.Hizmetler.FindAsync(id);
                    hizmet.Ad = viewModel.Ad;
                    hizmet.Aciklama = viewModel.Aciklama;
                    hizmet.SureDakika = viewModel.SureDakika;
                    hizmet.Ucret = viewModel.Ucret;
                    
                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Hizmetler.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hizmet = await _context.Hizmetler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hizmet == null) return NotFound();

             var viewModel = new HizmetViewModel
            {
                Id = hizmet.Id,
                Ad = hizmet.Ad,
                SureDakika = hizmet.SureDakika,
                Ucret = hizmet.Ucret
            };

            return View(viewModel);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet != null)
            {
                _context.Hizmetler.Remove(hizmet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}