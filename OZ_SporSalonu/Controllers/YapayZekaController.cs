using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OZ_SporSalonu.Services; 
using OZ_SporSalonu.ViewModels;
using System.Threading.Tasks;
using System;

namespace SporSOZ_SporSalonualonu.Controllers
{
    [Authorize(Roles = "Uye,Admin")]
    public class YapayZekaController : Controller
    {
        private readonly IYapayZekaService _yapayZekaService;

        public YapayZekaController(IYapayZekaService yapayZekaService)
        {
            _yapayZekaService = yapayZekaService;
        }

        // GET: YapayZeka/OneriAl
        public IActionResult OneriAl()
        {
            return View(new YapayZekaOneriViewModel());
        }

        // POST: YapayZeka/OneriAl
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OneriAl(YapayZekaOneriViewModel viewModel)
        {
            Console.WriteLine("--- CONTROLLER: Butona Basıldı ---");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("--- CONTROLLER HATASI: ModelState Geçersiz! ---");
                // Hataları logla
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"HATA: {item.Key} - {error.ErrorMessage}");
                    }
                }
                return View(viewModel);
            }

            try
            {
                Console.WriteLine("--- CONTROLLER: Servis Çağrılıyor... ---");
                
                // Servis çağrısına viewModel.VucutTipi EKLENDİ
                string oneri = await _yapayZekaService.EgzersizOnerisiAl(
                    viewModel.Kilo, 
                    viewModel.Boy, 
                    viewModel.VucutTipi, 
                    viewModel.Hedef ?? "Hedef belirtilmedi"); 
                
                viewModel.OneriMetni = oneri;

                // Görsel promptu da güncellendi
                string gorselPrompt = $"Fitness body type {viewModel.VucutTipi}, weight {viewModel.Kilo}kg, height {viewModel.Boy}cm, goal {viewModel.Hedef}, gym workout, cinematic lighting";
                viewModel.OneriGorselUrl = await _yapayZekaService.GorselOnerisiAl(gorselPrompt);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"CONTROLLER KRİTİK HATA: {ex.Message}");
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }

            return View(viewModel);
        }
    }
}