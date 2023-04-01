using API_DSCS2_WEBBANGIAY.Models;
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
    public class Branchs : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public Branchs(ShoesEcommereContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var branchs = _context.Branchs.ToList();
                return Ok(branchs);
            }
            catch(Exception err)
            {
                return NotFound(err.Message);
            }
        }
    }
}
