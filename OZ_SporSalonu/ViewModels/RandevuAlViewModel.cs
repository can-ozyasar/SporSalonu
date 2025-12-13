using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;


namespace OZ_SporSalonu.ViewModels
{
    public class RandevuAlViewModel
    {
        [Display(Name = "Hizmet Seçin")]
        [Required(ErrorMessage = "Lütfen bir hizmet seçin.")]
        public int HizmetId { get; set; }

        [Display(Name = "Antrenör Seçin")]
        [Required(ErrorMessage = "Lütfen bir antrenör seçin.")]
        public int AntrenorId { get; set; }

        [Display(Name = "Randevu Tarihi")]
        [Required(ErrorMessage = "Lütfen bir tarih seçin.")]
        [DataType(DataType.DateTime)]
        public DateTime RandevuBaslangic { get; set; }

       
        public IEnumerable<SelectListItem> ?HizmetListesi { get; set; }
        public IEnumerable<SelectListItem> ?AntrenorListesi { get; set; }
    }
}