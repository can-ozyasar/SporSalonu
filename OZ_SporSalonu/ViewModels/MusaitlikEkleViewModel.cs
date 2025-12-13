using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OZ_SporSalonu.Models;

namespace OZ_SporSalonu.ViewModels
{
    public class MusaitlikEkleViewModel
    {
        public int AntrenorId { get; set; }
        public string? AntrenorAd { get; set; }

        
        public List<GunSecimItem> Gunler { get; set; } = new List<GunSecimItem>();

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan Baslangic { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan Bitis { get; set; }

       
        public List<AntrenorMusaitlik>? MevcutMusaitlikler { get; set; }
    }

   
    public class GunSecimItem
    {
        public DayOfWeek GunDegeri { get; set; }
        public string GunAdi { get; set; }
        public bool SeciliMi { get; set; }
    }
}