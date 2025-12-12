using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OZ_SporSalonu.Data;
using OZ_SporSalonu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OZ_SporSalonu.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AntrenorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AntrenorApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AntrenorApi/TumAntrenorler
        [HttpGet("TumAntrenorler")]
        public async Task<ActionResult<IEnumerable<Antrenor>>> GetTumAntrenorler()
        {
            return await _context.Antrenorler.ToListAsync();
        }

        // GET: api/AntrenorApi/UzmanligaGoreFiltrele?uzmanlik=Yoga
        [HttpGet("UzmanligaGoreFiltrele")]
        public async Task<ActionResult<IEnumerable<Antrenor>>> GetAntrenorlerByUzmanlik(string uzmanlik)
        {
            if (string.IsNullOrWhiteSpace(uzmanlik))
            {
                return BadRequest("Uzmanlık alanı boş olamaz.");
            }

            var antrenorler = await _context.Antrenorler
                                    .Where(a => a.UzmanlikAlani.ToLower().Contains(uzmanlik.ToLower()))
                                    .ToListAsync();

            if (!antrenorler.Any())
            {
                return NotFound("Bu uzmanlık alanında antrenör bulunamadı.");
            }

            return Ok(antrenorler);
        }
    }
}