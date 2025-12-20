using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.GenAI;
using Google.GenAI.Types;
using Newtonsoft.Json; 

namespace OZ_SporSalonu.Services
{
    public class GeminiService : IYapayZekaService
    {
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<string> EgzersizOnerisiAl(int kilo, int boy, string vucutTipi, string hedef, Stream resimStream)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("BURAYA"))
            {
                return JsonConvert.SerializeObject(new { tavsiye = "HATA: API Anahtarı bulunamadı.", gorsel_prompt = "" });
            }

            try
            {
                var client = new Client(apiKey: _apiKey);
                var parts = new List<Part>();

                //  promt  kısmı
                string promptText = $@"
                    Sen uzman bir fitness koçusun.
                    Üye Bilgileri: Boy: {boy} cm, Kilo: {kilo} kg, Vücut Tipi: {vucutTipi}, Hedef: {hedef}.
                    
                    GÖREVLERİN:
                    1. Yüklenen fotoğrafı ve verileri analiz et.
                    2. Kişiye özel beslenme ve antrenman programı hazırla (Türkçe, Markdown formatında).
                    3. Bu kişi hedefine ulaştığında (3-6 ay sonra) vücudunun nasıl görüneceğini tarif eden KISA, DETAYLI ve İNGİLİZCE bir görsel promptu yaz. 
                       (Örn: 'Realistic photo of a fit man, 80kg, broad shoulders, defined abs, gym lighting, cinematic shot')

                    ÖNEMLİ: Cevabı SADECE aşağıdaki JSON formatında ver, başka hiçbir açıklama yapma:
                    {{
                        ""tavsiye"": ""...buraya markdown formatındaki türkçe tavsiye gelecek..."",
                        ""gorsel_prompt"": ""...buraya ingilizce görsel tarifi gelecek...""
                    }}
                ";

                parts.Add(new Part { Text = promptText });

                // resim işleme 
                if (resimStream != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await resimStream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        
                        parts.Add(new Part 
                        { 
                            InlineData = new Blob 
                            { 
                                MimeType = "image/jpeg", 
                               
                                Data = imageBytes 
                            } 
                        });
                    }
                }

                // İstek Gönderimi
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash", 
                    contents: new List<Content> { new Content { Parts = parts } }
                );

                if (response?.Candidates != null && response.Candidates.Count > 0)
                {
                    string rawText = response.Candidates[0].Content.Parts[0].Text;
                    
                   
                    rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
                    
                    return rawText;
                }

                return JsonConvert.SerializeObject(new { tavsiye = "Yapay zeka boş döndü.", gorsel_prompt = "" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { tavsiye = $"Hata: {ex.Message}", gorsel_prompt = "" });
            }
        }

        public Task<string> GorselOnerisiAl(string prompt)
        {
            return Task.FromResult("");
        }
    }
}