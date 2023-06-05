using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    //[Authorize(Roles = "HOMEADMIN")]
    [Area("admin")]
    [Route("api/[area]/[controller]")]

    [ApiController]
    public class HomeAdminController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public HomeAdminController(ShoesEcommereContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var tongthu = _context.PhieuNhapXuats.Where(x => x.LoaiPhieu.Trim() == "PHIEUTHU"&&x.createdAt.Date==DateTime.Now.Date).Sum(x=>x.TienDaThanhToan);
            var thongchi = _context.PhieuNhapXuats.Where(x => x.LoaiPhieu.Trim() == "PHIEUCHI" && x.createdAt.Date == DateTime.Now.Date).Sum(x => x.TienDaThanhToan);
            return Ok(new
            {
                tongthu,
                thongchi
            });

        }
    }
}
