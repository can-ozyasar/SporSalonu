using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OZ_SporSalonu.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string Ad { get; set; }
        [PersonalData]
        public string Soyad { get; set; }
        
        public virtual ICollection<Randevu> Randevular { get; set; }
    }
}