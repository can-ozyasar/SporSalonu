using System.Threading.Tasks;

namespace OZ_SporSalonu.Services
{
    public interface IYapayZekaService
    {
        Task<string> EgzersizOnerisiAl(int kilo, int boy, string vucutTipi, string hedef);
        Task<string> GorselOnerisiAl(string prompt);
    }
}