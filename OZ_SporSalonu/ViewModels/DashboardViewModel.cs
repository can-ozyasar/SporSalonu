using OZ_SporSalonu.Models;
using System.Collections.Generic;

namespace OZ_SporSalonu.ViewModels
{
    public class DashboardViewModel
    {
        //  Sayaçlar
        public int ToplamUyeSayisi { get; set; }
        public int ToplamAntrenorSayisi { get; set; }
        public int BekleyenRandevuSayisi { get; set; }
        
        // Finansal 
        public decimal ToplamCiro { get; set; }       // Tüm zamanlar
        public decimal BuAykiCiro { get; set; }       // Sadece bu ay
        
        //  Veriler
        public int BugunkuRandevuSayisi { get; set; }
        
        // Listeler
        public List<Randevu> BekleyenRandevular { get; set; } 
        public List<Randevu> BugunkuRandevular { get; set; }  
        public Dictionary<string, int> PopulerHizmetler { get; set; } 
    }
}