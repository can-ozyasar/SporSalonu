using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.GenAI;
using Google.GenAI.Types;

namespace OZ_SporSalonu.Services
{
    public class GeminiService : IYapayZekaService
    {
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"];
        }

        // İmza güncellendi: vucutTipi eklendi
        public async Task<string> EgzersizOnerisiAl(int kilo, int boy, string vucutTipi, string hedef)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("BURAYA"))
            {
                return "HATA: API Anahtarı bulunamadı.";
            }

            try
            {
                var client = new Client(apiKey: _apiKey);

                // Prompt güncellendi: Vücut tipi ve metabolik özellik vurgusu yapıldı
                var prompt = $@"
                    Sen uzman bir spor koçu ve diyetisyensin.
                    Danışan Bilgileri:
                    - Boy: {boy} cm
                    - Kilo: {kilo} kg
                    - Vücut Tipi: {vucutTipi} (Lütfen bu vücut tipinin metabolizma hızını ve kas yapısını dikkate al)
                    - Hedef: {hedef}

                    Bu bilgilere göre;
                    1. Vücut Tipine Uygun Egzersiz Stratejisi
                    2. Beslenme Tavsiyeleri (Makro besin dağılımı ipuçları)
                    3. Motivasyon Sözü
                    içeren, Markdown formatında, samimi ama profesyonel bir plan hazırla. Türkçe cevap ver.";

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash", // Daha güncel model varsa onu kullanabilirsiniz
                    contents: new List<Content> 
                    { 
                        new Content 
                        { 
                            Parts = new List<Part> 
                            { 
                                new Part { Text = prompt } 
                            } 
                        } 
                    }
                );

                if (response?.Candidates != null && response.Candidates.Count > 0)
                {
                    return response.Candidates[0].Content.Parts[0].Text;
                }

                return "Yapay zeka boş döndü.";
            }
            catch (Exception ex)
            {
                return $"Yapay Zeka Hatası: {ex.Message}";
            }
        }

        public Task<string> GorselOnerisiAl(string prompt)
        {
            // Şimdilik placeholder dönüyor
            return Task.FromResult("https://via.placeholder.com/400x300.png?text=AI+Fitness");
        }
    }
}