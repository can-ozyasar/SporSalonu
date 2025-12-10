
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.ViewModels;
using System; // DateTime için gerekli
using System.Linq;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            
            var bugun = DateTime.UtcNow.Date; 
            
            var buAyinBasi = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var model = new DashboardViewModel
            {
                // 1. Sayaçlar
                ToplamUyeSayisi = await _userManager.GetUsersInRoleAsync("Uye").ContinueWith(t => t.Result.Count),
                
                ToplamAntrenorSayisi = await _context.Antrenorler.CountAsync(),
                
                BekleyenRandevuSayisi = await _context.Randevular.CountAsync(r => !r.Onaylandi && r.RedMesaji == null),
                
                // Bugünün randevuları
                BugunkuRandevuSayisi = await _context.Randevular.CountAsync(r => 
                    r.RandevuBaslangic >= bugun && 
                    r.RandevuBaslangic < bugun.AddDays(1)),

                //  paralar
                ToplamCiro = await _context.Randevular
                    .Where(r => r.Onaylandi)
                    .SumAsync(r => (decimal?)r.KayitliUcret) ?? 0,

                BuAykiCiro = await _context.Randevular
                    .Where(r => r.Onaylandi && r.RandevuBaslangic >= buAyinBasi)
                    .SumAsync(r => (decimal?)r.KayitliUcret) ?? 0,

               
                //  Bekleyen son 5 talep
                BekleyenRandevular = await _context.Randevular
                    .Include(r => r.Uye)
                    .Include(r => r.Hizmet)
                    .Where(r => !r.Onaylandi && r.RedMesaji == null)
                    .OrderBy(r => r.RandevuBaslangic)
                    .Take(5)
                    .ToListAsync(),

                // Bugünün Programı
                BugunkuRandevular = await _context.Randevular
                    .Include(r => r.Uye)
                    .Include(r => r.Antrenor)
                    .Include(r => r.Hizmet)
                    .Where(r => r.RandevuBaslangic >= bugun && r.RandevuBaslangic < bugun.AddDays(1))
                    .OrderBy(r => r.RandevuBaslangic)
                    .ToListAsync(),
                    
                // Popüler Hizmetler
                PopulerHizmetler = await _context.Randevular
                    .GroupBy(r => r.Hizmet.Ad)
                    .Select(g => new { HizmetAdi = g.Key, Sayi = g.Count() })
                    .OrderByDescending(x => x.Sayi)
                    .Take(4)
                    .ToDictionaryAsync(x => x.HizmetAdi, x => x.Sayi)
            };

            return View(model);
        }

        // GET: Admin/Randevular
        public async Task<IActionResult> Randevular()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .OrderByDescending(r => r.RandevuBaslangic)
                .ToListAsync();

            return View(randevular);
        }

        // POST: Admin/RandevuOnayla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuOnayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.Onaylandi = true;
                randevu.RedMesaji = null; 
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Randevular));
        }

        // POST: Admin/RandevuReddet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuReddet(int id, string redMesaji)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.Onaylandi = false;
                randevu.RedMesaji = string.IsNullOrWhiteSpace(redMesaji) ? "Admin tarafından iptal edildi." : redMesaji;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Randevular));
        }
    }
}