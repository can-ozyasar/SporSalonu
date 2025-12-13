using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OZ_SporSalonu.Models
{
    public class Hizmet
    {
        public int Id { get; set; }
        [Required]
        public string Ad { get; set; } 
        public string Aciklama { get; set; }
        public int SureDakika { get; set; } 
        public decimal Ucret { get; set; }

        public virtual ICollection<AntrenorHizmet> AntrenorHizmetleri { get; set; }
    }
}