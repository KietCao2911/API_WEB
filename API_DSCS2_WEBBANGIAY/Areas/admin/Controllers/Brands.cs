using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class Brands : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public Brands(ShoesEcommereContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var brands = _context.Brands.ToList();
            return Ok(brands);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Brand brand)
        {
            try
            {
                brand.Slug = CustomSlug.Slugify(brand.Name);
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                return Ok(brand);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
    }
}
