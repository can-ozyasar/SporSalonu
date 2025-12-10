// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using OZ_SporSalonu.Data;
// using OZ_SporSalonu.Models;
// using OZ_SporSalonu.ViewModels;
// using System.Linq;
// using System.Threading.Tasks;

// namespace OZ_SporSalonu.Controllers
// {
//     [Authorize(Roles = "Admin")]
//     public class AdminController : Controller
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<ApplicationUser> _userManager;

//         public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }

//         public async Task<IActionResult> Dashboard()
//         {
//             // İstatistikleri Hesapla
//             var model = new DashboardViewModel
//             {
//                 // "Uye" rolündeki kullanıcı sayısı (Basitçe tüm users - 1 admin diyebiliriz veya sorgu atabiliriz)
//                 ToplamUyeSayisi = await _userManager.Users.CountAsync(), 
                
//                 ToplamAntrenorSayisi = await _context.Antrenorler.CountAsync(),
                
//                 ToplamRandevuSayisi = await _context.Randevular.CountAsync(),
                
//                 BekleyenRandevuSayisi = await _context.Randevular.CountAsync(r => !r.Onaylandi),
                
//                 // Hizmet ücretlerini topla
//                 ToplamTahminiGelir = await _context.Randevular
//                     .Include(r => r.Hizmet)
//                     .SumAsync(r => r.Hizmet.Ucret),

//                 // Son 5 randevuyu getir
//                 SonRandevular = await _context.Randevular
//                     .Include(r => r.Uye)
//                     .Include(r => r.Antrenor)
//                     .Include(r => r.Hizmet)
//                     .OrderByDescending(r => r.RandevuBaslangic)
//                     .Take(5)
//                     .ToListAsync()
//             };

//             return View(model);
//         }




//         // GET: Admin/Randevular (Listeleme)
// public async Task<IActionResult> Randevular()
// {
//     var randevular = await _context.Randevular
//         .Include(r => r.Uye)
//         .Include(r => r.Antrenor)
//         .Include(r => r.Hizmet)
//         .OrderByDescending(r => r.RandevuBaslangic)
//         .ToListAsync();

//     return View(randevular);
// }

// // POST: Admin/RandevuOnayla
// [HttpPost]
// public async Task<IActionResult> RandevuOnayla(int id)
// {
//     var randevu = await _context.Randevular.FindAsync(id);
//     if (randevu != null)
//     {
//         randevu.Onaylandi = true;
//         randevu.RedMesaji = null; // Onaylandıysa red mesajını temizle
//         await _context.SaveChangesAsync();
//     }
//     return RedirectToAction(nameof(Randevular));
// }

// // POST: Admin/RandevuReddet
// [HttpPost]
// public async Task<IActionResult> RandevuReddet(int id, string redMesaji)
// {
//     var randevu = await _context.Randevular.FindAsync(id);
//     if (randevu != null)
//     {
//         randevu.Onaylandi = false;
//         randevu.RedMesaji = string.IsNullOrWhiteSpace(redMesaji) ? "Admin tarafından iptal edildi." : redMesaji;
//         await _context.SaveChangesAsync();
//     }
//     return RedirectToAction(nameof(Randevular));
// }





//     }
// }


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.ViewModels;
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
            var model = new DashboardViewModel
            {
                // İstatistikleri veritabanından çekiyoruz
                ToplamUyeSayisi = await _userManager.GetUsersInRoleAsync("Uye").ContinueWith(t => t.Result.Count), 
                // Not: Eğer "Uye" rolü yoksa hata verebilir, alternatif olarak: await _context.Users.CountAsync(),

                ToplamAntrenorSayisi = await _context.Antrenorler.CountAsync(),
                
                ToplamRandevuSayisi = await _context.Randevular.CountAsync(),
                
                BekleyenRandevuSayisi = await _context.Randevular.CountAsync(r => !r.Onaylandi && r.RedMesaji == null),
                
                // Onaylanmış randevuların kayıtlı ücretlerini topla (Yoksa 0)
                ToplamTahminiGelir = await _context.Randevular
                    .Where(r => r.Onaylandi)
                    .SumAsync(r => (decimal?)r.KayitliUcret) ?? 0,

                // Son 5 randevuyu getir
                SonRandevular = await _context.Randevular
                    .Include(r => r.Uye)
                    .Include(r => r.Antrenor)
                    .Include(r => r.Hizmet)
                    .OrderByDescending(r => r.RandevuBaslangic)
                    .Take(5)
                    .ToListAsync()
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