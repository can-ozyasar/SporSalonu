using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; 

namespace OZ_SporSalonu.Models
{
    public class Antrenor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required]
        public string UzmanlikAlani { get; set; }
        
        // Mevcut İlişkiler
        public virtual ICollection<Randevu> Randevular { get; set; }
        public virtual ICollection<AntrenorHizmet> AntrenorHizmetleri { get; set; }

        
        public int? SalonId { get; set; } // Hangi salonda çalışıyor?
        
        [ForeignKey("SalonId")] 
        public virtual Salon Salon { get; set; }

        public virtual ICollection<AntrenorMusaitlik> Musaitlikler { get; set; }
    }
}