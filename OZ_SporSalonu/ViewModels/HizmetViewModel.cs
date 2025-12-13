using System.ComponentModel.DataAnnotations;

namespace OZ_SporSalonu.ViewModels
{
    public class HizmetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        public string Ad { get; set; } 

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Süre zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
        [Range(15, 180, ErrorMessage = "Süre 15-180 dakika arasında olmalıdır.")]
        public int SureDakika { get; set; } 

        [Required(ErrorMessage = "Ücret zorunludur.")]
        [Display(Name = "Ücret (₺)")]
        [Range(0, 10000, ErrorMessage = "Geçerli bir ücret girin.")]
        [DataType(DataType.Currency)]
        public decimal Ucret { get; set; }
    }
}