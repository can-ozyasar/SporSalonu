using System.ComponentModel.DataAnnotations;

namespace OZ_SporSalonu.ViewModels
{
    public class YapayZekaOneriViewModel
    {
        [Display(Name = "Mevcut Kilonuz (kg)")]
        [Required(ErrorMessage = "Kilo bilgisi zorunludur.")]
        [Range(30, 300, ErrorMessage = "Geçerli bir kilo giriniz.")]
        public int Kilo { get; set; }

        [Display(Name = "Boyunuz (cm)")]
        [Required(ErrorMessage = "Boy bilgisi zorunludur.")]
        [Range(100, 250, ErrorMessage = "Geçerli bir boy giriniz.")]
        public int Boy { get; set; }

        // --- YENİ EKLENEN ALAN ---
        [Display(Name = "Vücut Tipi")]
        [Required(ErrorMessage = "Lütfen vücut tipinizi seçiniz.")]
        public string VucutTipi { get; set; } // Ektomorf, Mezomorf, Endomorf

        [Display(Name = "Hedefiniz")]
        [Required(ErrorMessage = "Hedef belirtmek zorunludur.")]
        public string Hedef { get; set; } 

        public string? OneriMetni { get; set; }
        public string? OneriGorselUrl { get; set; }
    }
}