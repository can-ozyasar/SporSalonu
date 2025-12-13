using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic; // List<> için gerekli
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

        public async Task<string> EgzersizOnerisiAl(int kilo, int boy, string hedef)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("BURAYA"))
            {
                return "HATA: API Anahtarı bulunamadı.";
            }

            try
            {
                
                var client = new Client(apiKey: _apiKey);

                var prompt = $"Sen bir spor hocasısın. {boy} cm boyunda ve {kilo} kg ağırlığında bir üye için '{hedef}' hedefine yönelik; 1. Egzersiz Tavsiyesi, 2. Beslenme İpucu, 3. Motivasyon Sözü içeren kısa, maddeli bir plan hazırla. Türkçe cevap ver.";

                
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
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
            return Task.FromResult("https://via.placeholder.com/400x300.png?text=AI+Onerisi");
        }
    }
}