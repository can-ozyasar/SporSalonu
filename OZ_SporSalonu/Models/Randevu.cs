using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace OZ_SporSalonu.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        [Required]
        public DateTime RandevuBaslangic { get; set; }
        public DateTime RandevuBitis { get; set; }
        public bool Onaylandi { get; set; } = false;

        [Required]
        public string UyeId { get; set; } 
        [ForeignKey("UyeId")]
        public virtual ApplicationUser Uye { get; set; }

        [Required]
        public int AntrenorId { get; set; }
        [ForeignKey("AntrenorId")]
        public virtual Antrenor Antrenor { get; set; }
        
        [Required]
        public int HizmetId { get; set; }
        [ForeignKey("HizmetId")]
        public virtual Hizmet Hizmet { get; set; }


        
        public string? RedMesaji { get; set; }


        [Column(TypeName = "decimal(18,2)")]
    public decimal KayitliUcret { get; set; } 
    
    public int KayitliSureDakika { get; set; }
    }
}