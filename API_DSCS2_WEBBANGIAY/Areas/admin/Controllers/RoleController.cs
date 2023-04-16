using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public RoleController(ShoesEcommereContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = _context.Roles;
                return Ok(roles ?? null);
            }
            catch(Exception err)
            {
                return BadRequest();
            }
        }
    }
}
