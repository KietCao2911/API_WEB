using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class KhoHang : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public KhoHang(ShoesEcommereContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var khohangs = _context.KhoHangs.Include(x=>x.BranchNavigation).ThenInclude(x=>x.PhieuNhapXuats).Where(x => x.MaSanPham == id);
                return Ok(khohangs);
            }
            catch(Exception err)
            {
                return BadRequest(err);
            }
        }
        [HttpGet("GetProducts/{maChiNhanh}")]
        public async Task<IActionResult> GetProducts(string maChiNhanh, [FromQuery(Name = "s")] string s)
        {
            try
            {
                if (maChiNhanh == null)
                {
                    maChiNhanh = "CN01";
                }
                var products = _context.KhoHangs.Include(x => x.SanPhamNavigation).Include(x => x.SanPhamNavigation).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x => x.IdHinhAnhNavigation).Where(x => x.MaChiNhanh.Trim() == maChiNhanh.Trim() && x.SanPhamNavigation.ParentID != null);
                if (s is not null && s.Length > 0)
                {
                    products = products.Where(x => x.SanPhamNavigation.TenSanPham.Trim().Contains(s.Trim()));

                }
                return Ok(products);
            }catch(Exception err)
            {
                return BadRequest(err.Message); 
            }
        }
    }
}
