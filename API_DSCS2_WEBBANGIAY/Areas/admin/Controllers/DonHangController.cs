using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_DSCS2_WEBBANGIAY.Models;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]

    public class DonHangController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;
        public DonHangController(ShoesEcommereContext context)
        {
            _context = context;
        }

        // GET: api/DonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuNhapXuat>>> GetHoaDons()
        {
            try
            {
                var hoadons = _context.PhieuNhapXuats.Include(x => x.DiaChiNavigation).Include(x => x.KhachHangNavigation);
                return Ok(hoadons);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<PhieuNhapXuat>>> GetHoaDons(int id)
        {
            try
            {
                var hoadon = _context.PhieuNhapXuats.Include(x => x.ChiTietNhapXuats).ThenInclude(x => x.SanPhamNavigation).Include(x => x.DiaChiNavigation).FirstOrDefault(x => x.Id == id);
                return Ok(hoadon);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
