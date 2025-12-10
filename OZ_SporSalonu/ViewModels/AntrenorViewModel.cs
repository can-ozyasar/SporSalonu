using System.ComponentModel.DataAnnotations;

namespace OZ_SporSalonu.ViewModels
{
    public class AntrenorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Antrenör Adı ve Soyadı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string UzmanlikAlani { get; set; }

        [Display(Name = "Çalıştığı Salon")]
        public int? SalonId { get; set; }

                public string? SalonAdi { get; set; }



        
public List<string> MusaitlikOzetleri { get; set; } = new List<string>();
    }
}