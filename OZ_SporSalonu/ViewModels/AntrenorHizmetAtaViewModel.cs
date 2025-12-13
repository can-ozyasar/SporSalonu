using System.Collections.Generic;

namespace OZ_SporSalonu.ViewModels
{
    public class AntrenorHizmetAtaViewModel
    {
        public int AntrenorId { get; set; }
        public string AntrenorAdSoyad { get; set; }
        public List<HizmetSecimItem> Hizmetler { get; set; }
    }

    public class HizmetSecimItem
    {
        public int HizmetId { get; set; }
        public string HizmetAd { get; set; }
        public bool SeciliMi { get; set; } 
    }
}