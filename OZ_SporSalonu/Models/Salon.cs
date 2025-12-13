using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OZ_SporSalonu.Models
{
    public class Salon
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Salon Adı")]
        public string Ad { get; set; } 

        [Display(Name = "Adres")]
        public string Adres { get; set; }

        // Bir salonda birden fazla antrenör çalışır
        public virtual ICollection<Antrenor> Antrenorler { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Açılış Saati")]
        public TimeSpan AcilisSaati { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Kapanış Saati")]
        public TimeSpan KapanisSaati { get; set; }
    }
}