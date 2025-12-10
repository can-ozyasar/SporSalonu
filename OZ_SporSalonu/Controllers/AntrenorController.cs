using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using OZ_SporSalonu.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering; 
using System.Linq;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }


public async Task<IActionResult> Index()
{
    var antrenorler = await _context.Antrenorler
        .Include(a => a.Musaitlikler) 
        .Include(a => a.Salon)  
        .ToListAsync();

    var viewModel = antrenorler.Select(a => new AntrenorViewModel
    {
        Id = a.Id,
        AdSoyad = a.AdSoyad,
        UzmanlikAlani = a.UzmanlikAlani,
        SalonId = a.SalonId,
        SalonAdi = a.Salon != null ? a.Salon.Ad : "Şube Atanmamış",

        // Günleri Türkçeleştirerek listeye ekliyoruz
        MusaitlikOzetleri = a.Musaitlikler
            .OrderBy(m => m.Gun)
            .Select(m => $"{GunTurkce(m.Gun)}: {m.BaslangicSaati:hh\\:mm}-{m.BitisSaati:hh\\:mm}")
            .ToList()
    }).ToList();
    
    return View(viewModel);
}



private string GunTurkce(DayOfWeek gun)
{
    return gun switch {
        DayOfWeek.Monday => "Pzt",
        DayOfWeek.Tuesday => "Sal",
        DayOfWeek.Wednesday => "Çar",
        DayOfWeek.Thursday => "Per",
        DayOfWeek.Friday => "Cum",
        DayOfWeek.Saturday => "Cmt",
        DayOfWeek.Sunday => "Paz",
        _ => gun.ToString().Substring(0,3)
    };
}



public async Task<IActionResult> Create()
{
    
    ViewBag.HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Ad", "Ad");
    
    ViewBag.SalonListesi = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad");
    
    return View();
}




[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(AntrenorViewModel viewModel)
{
    if (ModelState.IsValid)
    {
        
        var antrenor = new Antrenor
        {
            AdSoyad = viewModel.AdSoyad,
            UzmanlikAlani = viewModel.UzmanlikAlani, 
            SalonId = viewModel.SalonId
        };

        _context.Add(antrenor);
        await _context.SaveChangesAsync(); // Burada veritabanı antrenöre bir ID atar

        // Seçilen uzmanlık alanına karşılık gelen Hizmet'i bul
        
        var secilenHizmet = await _context.Hizmetler
            .FirstOrDefaultAsync(h => h.Ad == viewModel.UzmanlikAlani);

        // Eğer hizmet bulunduysa, ilişki tablosuna (AntrenorHizmet) ekle
        if (secilenHizmet != null)
        {
            var yeniIliski = new AntrenorHizmet
            {
                AntrenorId = antrenor.Id, 
                HizmetId = secilenHizmet.Id
            };
            _context.Add(yeniIliski);
            await _context.SaveChangesAsync(); 
        }

        return RedirectToAction(nameof(Index));
    }
    
    // Hata varsa listeleri tekrar dolduralım
    ViewBag.HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Ad", "Ad");
    ViewBag.SalonListesi = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad");
    return View(viewModel);
}


public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();

    var antrenor = await _context.Antrenorler.FindAsync(id);
    if (antrenor == null) return NotFound();

    var viewModel = new AntrenorViewModel
    {
        Id = antrenor.Id,
        AdSoyad = antrenor.AdSoyad,
        UzmanlikAlani = antrenor.UzmanlikAlani,
        
        
        SalonId = antrenor.SalonId 
    };

   
    ViewBag.HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Ad", "Ad");
    
    
    ViewBag.SalonListesi = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", antrenor.SalonId);

    return View(viewModel);
}



[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MusaitlikSil(int id)
{
    // Silinecek kaydı bul
    var musaitlik = await _context.AntrenorMusaitlikleri.FindAsync(id);
    
    if (musaitlik != null)
    {
        // Silmeden önce Antrenör ID'sini sakla (Geri dönüş için lazım)
        int antrenorId = musaitlik.AntrenorId;
        
        // Sil ve Kaydet
        _context.AntrenorMusaitlikleri.Remove(musaitlik);
        await _context.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Çalışma saati silindi.";
        
        // Yine aynı sayfaya geri dön
        return RedirectToAction("MusaitlikEkle", new { id = antrenorId });
    }
    
    // Kayıt bulunamazsa listeye dön
    return RedirectToAction(nameof(Index));
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, AntrenorViewModel viewModel)
{
    if (id != viewModel.Id) return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            
            // Verileri güncelle
            antrenor.AdSoyad = viewModel.AdSoyad;
            antrenor.UzmanlikAlani = viewModel.UzmanlikAlani;
        
            // Seçilen SalonId'yi veritabanına kaydet 
            antrenor.SalonId = viewModel.SalonId;
            
            _context.Update(antrenor);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Antrenorler.Any(e => e.Id == id))
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
    
    // Hata olursa listeleri tekrar doldur
    ViewBag.HizmetListesi = new SelectList(await _context.Hizmetler.ToListAsync(), "Ad", "Ad");
    ViewBag.SalonListesi = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", viewModel.SalonId);
    
    return View(viewModel);
}

        
        
      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var antrenor = await _context.Antrenorler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (antrenor == null) return NotFound();

             var viewModel = new AntrenorViewModel
            {
                Id = antrenor.Id,
                AdSoyad = antrenor.AdSoyad,
                UzmanlikAlani = antrenor.UzmanlikAlani
            };

            return View(viewModel);
        }

        

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor != null)
            {
                _context.Antrenorler.Remove(antrenor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        
        

        public async Task<IActionResult> HizmetAta(int? id)
        {
            if (id == null) return NotFound();

            var antrenor = await _context.Antrenorler
                .Include(a => a.AntrenorHizmetleri)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null) return NotFound();

            var tumHizmetler = await _context.Hizmetler.ToListAsync();

            var model = new AntrenorHizmetAtaViewModel
            {
                AntrenorId = antrenor.Id,
                AntrenorAdSoyad = antrenor.AdSoyad,
                Hizmetler = tumHizmetler.Select(h => new HizmetSecimItem
                {
                    HizmetId = h.Id,
                    HizmetAd = h.Ad,
                    SeciliMi = antrenor.AntrenorHizmetleri.Any(ah => ah.HizmetId == h.Id)
                }).ToList()
            };

            return View(model);
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HizmetAta(AntrenorHizmetAtaViewModel model)
        {
            var mevcutIliskiler = _context.AntrenorHizmetleri.Where(ah => ah.AntrenorId == model.AntrenorId);
            _context.AntrenorHizmetleri.RemoveRange(mevcutIliskiler);

            foreach (var item in model.Hizmetler)
            {
                if (item.SeciliMi)
                {
                    _context.AntrenorHizmetleri.Add(new AntrenorHizmet
                    {
                        AntrenorId = model.AntrenorId,
                        HizmetId = item.HizmetId
                    });
                }
            }

            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Uzmanlık alanları güncellendi.";
            return RedirectToAction(nameof(Index));
        }







public async Task<IActionResult> MusaitlikEkle(int id)
{
    var antrenor = await _context.Antrenorler
        .Include(a => a.Musaitlikler)
        .Include(a => a.Salon)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (antrenor == null) return NotFound();

    if (antrenor.Salon != null)
    {
        ViewBag.SalonBilgisi = $"Salon: {antrenor.Salon.Ad} ({antrenor.Salon.AcilisSaati:hh\\:mm} - {antrenor.Salon.KapanisSaati:hh\\:mm})";
    }
    else
    {
        ViewBag.SalonBilgisi = "Uyarı: Bu antrenöre henüz bir Salon atanmamış!";
    }

    var model = new MusaitlikEkleViewModel
    {
        AntrenorId = antrenor.Id,
        AntrenorAd = antrenor.AdSoyad,
        MevcutMusaitlikler = antrenor.Musaitlikler.OrderBy(m => m.Gun).ToList(),
        
        
        Gunler = new List<GunSecimItem>
        {
            new GunSecimItem { GunDegeri = DayOfWeek.Monday, GunAdi = "Pazartesi" },
            new GunSecimItem { GunDegeri = DayOfWeek.Tuesday, GunAdi = "Salı" },
            new GunSecimItem { GunDegeri = DayOfWeek.Wednesday, GunAdi = "Çarşamba" },
            new GunSecimItem { GunDegeri = DayOfWeek.Thursday, GunAdi = "Perşembe" },
            new GunSecimItem { GunDegeri = DayOfWeek.Friday, GunAdi = "Cuma" },
            new GunSecimItem { GunDegeri = DayOfWeek.Saturday, GunAdi = "Cumartesi" },
            new GunSecimItem { GunDegeri = DayOfWeek.Sunday, GunAdi = "Pazar" }
        }
    };

    return View(model);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MusaitlikEkle(MusaitlikEkleViewModel model)
{
    //  Antrenörü Çek
    var antrenor = await _context.Antrenorler.Include(a => a.Salon).FirstOrDefaultAsync(a => a.Id == model.AntrenorId);
    if (antrenor == null) return NotFound();

    // Gün Seçimi Kontrolü
    if (!model.Gunler.Any(g => g.SeciliMi))
    {
        ModelState.AddModelError("", "Lütfen en az bir gün seçiniz.");
    }

    //  Salon Saat Kontrolü
    if (antrenor.Salon != null)
    {
        if (model.Baslangic < antrenor.Salon.AcilisSaati) ModelState.AddModelError("Baslangic", "Başlangıç saati salon açılışından önce olamaz.");
        if (model.Bitis > antrenor.Salon.KapanisSaati) ModelState.AddModelError("Bitis", "Bitiş saati salon kapanışından sonra olamaz.");
    }

    if (model.Baslangic >= model.Bitis)
    {
        ModelState.AddModelError("Bitis", "Bitiş saati başlangıçtan sonra olmalıdır.");
    }

    if (ModelState.IsValid)
    {
        foreach (var gunItem in model.Gunler.Where(g => g.SeciliMi))
        {
            // Veritabanında bu hocanın, bu gün, bu saatler arasında kaydı var mı?
            
            bool cakismaVar = await _context.AntrenorMusaitlikleri.AnyAsync(m =>
                m.AntrenorId == model.AntrenorId &&
                m.Gun == gunItem.GunDegeri &&
                (model.Baslangic < m.BitisSaati && model.Bitis > m.BaslangicSaati)
            );

            if (cakismaVar)
            {
                ModelState.AddModelError("", $"{gunItem.GunAdi} günü için {model.Baslangic:hh\\:mm}-{model.Bitis:hh\\:mm} aralığında zaten bir kayıt veya çakışma var.");
            }
        }
    }

    if (ModelState.IsValid)
    {
        foreach (var gunItem in model.Gunler.Where(g => g.SeciliMi))
        {
            var musaitlik = new AntrenorMusaitlik
            {
                AntrenorId = model.AntrenorId,
                Gun = gunItem.GunDegeri,
                BaslangicSaati = model.Baslangic,
                BitisSaati = model.Bitis
            };
            _context.AntrenorMusaitlikleri.Add(musaitlik);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Çalışma saatleri başarıyla eklendi.";
        return RedirectToAction("MusaitlikEkle", new { id = model.AntrenorId });
    }

    // Hata varsa listeleri tekrar doldur ve sayfayı göster
    model.MevcutMusaitlikler = await _context.AntrenorMusaitlikleri
        .Where(m => m.AntrenorId == model.AntrenorId)
        .OrderBy(m => m.Gun)
        .ToListAsync();

    if (antrenor.Salon != null)
    {
        ViewBag.SalonBilgisi = $"Salon: {antrenor.Salon.Ad} ({antrenor.Salon.AcilisSaati:hh\\:mm} - {antrenor.Salon.KapanisSaati:hh\\:mm})";
    }

    return View(model);
}


private List<SelectListItem> GetTurkceGunler()
{
    return new List<SelectListItem>
    {
        new SelectListItem { Value = "Monday", Text = "Pazartesi" },
        new SelectListItem { Value = "Tuesday", Text = "Salı" },
        new SelectListItem { Value = "Wednesday", Text = "Çarşamba" },
        new SelectListItem { Value = "Thursday", Text = "Perşembe" },
        new SelectListItem { Value = "Friday", Text = "Cuma" },
        new SelectListItem { Value = "Saturday", Text = "Cumartesi" },
        new SelectListItem { Value = "Sunday", Text = "Pazar" }
    };
}
    }
}