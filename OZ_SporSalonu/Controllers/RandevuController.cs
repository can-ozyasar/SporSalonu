
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace OZ_SporSalonu.Controllers
{
    [Authorize(Roles = "Uye,Admin")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Randevu (Listeleme)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var randevular = await _context.Randevular
                                .Where(r => r.UyeId == userId)
                                .Include(r => r.Antrenor) 
                                .Include(r => r.Hizmet)   
                                .OrderBy(r => r.RandevuBaslangic)
                                .ToListAsync();
            
            return View(randevular);
        }

        // GET: Randevu/Al
        [Authorize(Roles = "Uye")] 
        public async Task<IActionResult> Al()
        {
            // Antrenörleri, verdikleri hizmetler ve çalıştığı salon ile birlikte çekiyoruz
            var antrenorler = await _context.Antrenorler
                .Include(a => a.AntrenorHizmetleri)
                .Include(a => a.Salon)
                .Include(a => a.Musaitlikler) 
                .ToListAsync();

            ViewBag.AntrenorListesi = antrenorler; 
            
            var viewModel = new RandevuAlViewModel
            {
                HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Id", "Ad")
            };
            return View(viewModel);
        }

        // POST: Randevu/Al
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Uye")]
        public async Task<IActionResult> Al(RandevuAlViewModel viewModel)
        {
            // Hizmet Kontrolü
            var secilenHizmet = await _context.Hizmetler.FindAsync(viewModel.HizmetId);
            if (secilenHizmet == null)
            {
                ModelState.AddModelError("", "Hizmet bulunamadı.");
                return await HataIleDon(viewModel);
            }

            // Antrenör Yetkinlik Kontrolü
            bool antrenorBuHizmetiVeriyorMu = await _context.AntrenorHizmetleri
                .AnyAsync(ah => ah.AntrenorId == viewModel.AntrenorId && ah.HizmetId == viewModel.HizmetId);

            if (!antrenorBuHizmetiVeriyorMu)
            {
                ModelState.AddModelError("AntrenorId", "Seçilen antrenör bu hizmeti vermemektedir.");
                return await HataIleDon(viewModel);
            }

            // Tarih Zaman Ayarlamaları
            var localRandevuBaslangic = DateTime.SpecifyKind(viewModel.RandevuBaslangic, DateTimeKind.Local);
            DateTime localRandevuBitis = localRandevuBaslangic.AddMinutes(secilenHizmet.SureDakika);
            
            var utcBaslangic = localRandevuBaslangic.ToUniversalTime();
            var utcBitis = localRandevuBitis.ToUniversalTime();
            var utcSuAn = DateTime.UtcNow;

            var antrenorBilgisi = await _context.Antrenorler
        .Include(a => a.Salon)
        .FirstOrDefaultAsync(a => a.Id == viewModel.AntrenorId);

    // Eğer antrenör bir salona bağlıysa kontrol et
    if (antrenorBilgisi != null && antrenorBilgisi.Salon != null)
    {
        var randevuSaati = localRandevuBaslangic.TimeOfDay;      
        var randevuBitisSaati = localRandevuBitis.TimeOfDay;   
        
        var salonAcilis = antrenorBilgisi.Salon.AcilisSaati;     
        var salonKapanis = antrenorBilgisi.Salon.KapanisSaati;   


        // Randevu salon açılmadan önce mi VEYA salon kapandıktan sonra mı bitiyor?
        if (randevuSaati < salonAcilis || randevuBitisSaati > salonKapanis)
        {
            ModelState.AddModelError("RandevuBaslangic", 
                $"Seçilen antrenörün bulunduğu salon ({antrenorBilgisi.Salon.Ad}) " +
                $"{salonAcilis:hh\\:mm} - {salonKapanis:hh\\:mm} saatleri arasında hizmet vermektedir.");
            
            return await HataIleDon(viewModel);
        }
    }


            // Müsaitlik  Kontrolü 
            var istenenGun = localRandevuBaslangic.DayOfWeek;
            var istenenSaat = localRandevuBaslangic.TimeOfDay; 
            var istenenBitisSaat = localRandevuBitis.TimeOfDay;

            var calismaSaati = await _context.AntrenorMusaitlikleri
                .FirstOrDefaultAsync(m => m.AntrenorId == viewModel.AntrenorId && m.Gun == istenenGun);

            if (calismaSaati == null)
            {
                ModelState.AddModelError("RandevuBaslangic", "Antrenör bu gün çalışmamaktadır.");
                return await HataIleDon(viewModel);
            }

            // Saat aralığı kontrolü
            if (istenenSaat < calismaSaati.BaslangicSaati || istenenBitisSaat > calismaSaati.BitisSaati)
            {
                ModelState.AddModelError("RandevuBaslangic", $"Antrenör sadece {calismaSaati.BaslangicSaati:hh\\:mm} - {calismaSaati.BitisSaati:hh\\:mm} saatleri arasında hizmet vermektedir.");
                return await HataIleDon(viewModel);
            }
            

            // Zaman Kısıtlamaları
            if (utcBaslangic < utcSuAn)
            {
                ModelState.AddModelError("RandevuBaslangic", "Geçmiş bir tarihe randevu alamazsınız.");
                return await HataIleDon(viewModel);
            }

            if (utcBaslangic < utcSuAn.AddHours(1))
            {
                ModelState.AddModelError("RandevuBaslangic", "Randevular en erken 1 saat sonrasına alınabilir.");
                return await HataIleDon(viewModel);
            }

            //  GLOBAL ÇAKIŞMA
            bool antrenorDolu = await _context.Randevular
                .AnyAsync(r => r.AntrenorId == viewModel.AntrenorId &&
                r.RedMesaji == null && 
                               r.RandevuBaslangic < utcBitis && r.RandevuBitis > utcBaslangic);

            if (antrenorDolu)
            {
                ModelState.AddModelError("RandevuBaslangic", "Seçtiğiniz antrenör bu saat aralığında maalesef dolu.");
                return await HataIleDon(viewModel);
            }

            // KİŞİSEL ÇAKIŞMA
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            bool kullaniciDolu = await _context.Randevular
                .AnyAsync(r => r.UyeId == userId &&
                r.RedMesaji == null && 
                               r.RandevuBaslangic < utcBitis && r.RandevuBitis > utcBaslangic);

            if (kullaniciDolu)
            {
                ModelState.AddModelError("RandevuBaslangic", "Bu saat aralığında zaten başka bir randevunuz var.");
                return await HataIleDon(viewModel);
            }



            // Kaydetme işlemleri 
            if (ModelState.IsValid)
            {
                var randevu = new Randevu
                {
                    UyeId = userId,
                    AntrenorId = viewModel.AntrenorId,
                    HizmetId = viewModel.HizmetId,
                    RandevuBaslangic = utcBaslangic,
                    RandevuBitis = utcBitis,
                    Onaylandi = false, 
                    
                    //  Snapshot (Detay Saklama) 
                    KayitliUcret = secilenHizmet.Ucret,
                    KayitliSureDakika = secilenHizmet.SureDakika
                    
                };

                _context.Add(randevu);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Randevu talebiniz alındı. Onay bekleniyor.";
                return RedirectToAction(nameof(Index));
            }

            return await HataIleDon(viewModel);
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Uye")]
        public async Task<IActionResult> Iptal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var randevu = await _context.Randevular
                .FirstOrDefaultAsync(r => r.Id == id && r.UyeId == userId);

            if (randevu != null)
            {
                if (randevu.RandevuBaslangic < DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "Geçmiş randevular iptal edilemez.";
                }
                else
                {
                    _context.Randevular.Remove(randevu);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Randevu iptal edildi.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
            }

            return RedirectToAction(nameof(Index));
        }

        


        private async Task<IActionResult> HataIleDon(RandevuAlViewModel viewModel)
        {
            viewModel.HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Id", "Ad", viewModel.HizmetId);
            
            var antrenorler = await _context.Antrenorler
                .Include(a => a.AntrenorHizmetleri)
                .Include(a => a.Salon)
                .Include(a => a.Musaitlikler) 
                .ToListAsync();
            ViewBag.AntrenorListesi = antrenorler;

            return View(viewModel);
        }
    }
}