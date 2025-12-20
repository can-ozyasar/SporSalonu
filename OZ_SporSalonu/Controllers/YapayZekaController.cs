using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OZ_SporSalonu.Services; 
using OZ_SporSalonu.ViewModels;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq; // JSON Parçalama için gerekli

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
            // Resim kontrolü
            if (viewModel.YuklenenResim == null || viewModel.YuklenenResim.Length == 0)
            {
                ModelState.AddModelError("YuklenenResim", "Lütfen bir fotoğraf yükleyiniz.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            try
            {
                
                using (var stream = viewModel.YuklenenResim.OpenReadStream())
                {
                    
                    string jsonResponse = await _yapayZekaService.EgzersizOnerisiAl(
                        viewModel.Kilo, 
                        viewModel.Boy, 
                        viewModel.VucutTipi, 
                        viewModel.Hedef,
                        stream); 

                    // Gelen JSON string'i parçala
                    var jsonObject = JObject.Parse(jsonResponse);

                    viewModel.OneriMetni = jsonObject["tavsiye"]?.ToString();
                    viewModel.GelecekGorselPromptu = jsonObject["gorsel_prompt"]?.ToString();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"CONTROLLER HATASI: {ex.Message}");
                ModelState.AddModelError("", "Yapay zeka servisinde bir sorun oluştu.");
            }

            return View(viewModel);
        }
    }
}