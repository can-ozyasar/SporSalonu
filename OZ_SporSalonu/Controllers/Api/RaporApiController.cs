using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaporApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RaporApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm Antrenörleri Listeleme 
        // GET: api/RaporApi/TumAntrenorler
        [HttpGet("TumAntrenorler")]
        public async Task<ActionResult<IEnumerable<object>>> GetTumAntrenorler()
        {
            // LINQ Select ile sadece gerekli verileri (DTO mantığı) çekiyoruz
            var antrenorler = await _context.Antrenorler
                .Select(a => new {
                    a.Id,
                    a.AdSoyad,
                    a.UzmanlikAlani,
                    SalonAdi = a.Salon != null ? a.Salon.Ad : "Belirtilmemiş"
                })
                .ToListAsync();

            return Ok(antrenorler);
        }

        // Belirli Bir Tarihte Çalışan (Uygun) Antrenörleri Getirme (Filtreleme LINQ)
        // GET: api/RaporApi/MusaitAntrenorler?tarih=2025-11-25
        [HttpGet("MusaitAntrenorler")]
        public async Task<ActionResult<IEnumerable<object>>> GetMusaitAntrenorler(DateTime tarih)
        {
            var istenenGun = tarih.DayOfWeek;

            
            //  Antrenörün o gün (Musaitlikler tablosunda) kaydı var mı?
            //  O günkü çalışma saatlerini de sonucun içine ekle.
            var uygunAntrenorler = await _context.Antrenorler
                .Where(a => a.Musaitlikler.Any(m => m.Gun == istenenGun)) 
                .Select(a => new 
                {
                    AntrenorId = a.Id,
                    Isim = a.AdSoyad,
                    Uzmanlik = a.UzmanlikAlani,
                    CalismaSaatleri = a.Musaitlikler
                        .Where(m => m.Gun == istenenGun)
                        .Select(m => $"{m.BaslangicSaati:hh\\:mm} - {m.BitisSaati:hh\\:mm}")
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (!uygunAntrenorler.Any())
            {
                return NotFound($"{tarih.ToShortDateString()} tarihinde çalışan antrenör bulunamadı.");
            }

            return Ok(uygunAntrenorler);
        }

        //  Bir Üyenin Randevularını Getirme (İlişkisel LINQ)
        // GET: api/RaporApi/UyeRandevulari?email=ogrenci@sakarya.edu.tr
        [HttpGet("UyeRandevulari")]
        public async Task<ActionResult<IEnumerable<object>>> GetUyeRandevulari(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email boş olamaz.");

            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            
            var randevular = await _context.Randevular
                .Where(r => r.UyeId == user.Id)
                .OrderByDescending(r => r.RandevuBaslangic) // Tarihe göre sırala
                .Select(r => new 
                {
                    RandevuId = r.Id,
                    Tarih = r.RandevuBaslangic,
                    Hizmet = r.Hizmet.Ad,
                    Antrenor = r.Antrenor.AdSoyad,
                    Ucret = r.KayitliUcret, 
                    Durum = r.Onaylandi ? "Onaylandı" : (r.RedMesaji != null ? "İptal: " + r.RedMesaji : "Bekliyor")
                })
                .ToListAsync();

            return Ok(randevular);
        }
    }
}