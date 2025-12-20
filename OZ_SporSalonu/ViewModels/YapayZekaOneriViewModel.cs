using Microsoft.AspNetCore.Http;
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

        [Display(Name = "Vücut Tipi")]
        [Required(ErrorMessage = "Lütfen vücut tipinizi seçiniz.")]
        public string VucutTipi { get; set; }

        [Display(Name = "Hedefiniz")]
        [Required(ErrorMessage = "Hedef belirtmek zorunludur.")]
        public string Hedef { get; set; }

        
        [Display(Name = "Vücut Fotoğrafınız")]
        [Required(ErrorMessage = "Analiz için fotoğraf yüklemeniz gerekmektedir.")]
        public IFormFile YuklenenResim { get; set; }

        
        public string? OneriMetni { get; set; }
        public string? GelecekGorselPromptu { get; set; } 
    }
}