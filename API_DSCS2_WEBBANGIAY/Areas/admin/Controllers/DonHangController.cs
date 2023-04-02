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
                var hoadons = _context.PhieuNhapXuats.Include(x => x.DiaChiNavigation).Include(x => x.KhachHangNavigation).Where(x=>x.LoaiPhieu=="PHIEUXUAT").OrderByDescending(x=>x.createdAt);
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
                var hoadon = _context.PhieuNhapXuats.Include(x => x.ChiTietNhapXuats)
                    .ThenInclude(x => x.SanPhamNavigation).Include(x => x.DiaChiNavigation).FirstOrDefault(x => x.Id == id&&x.LoaiPhieu=="PHIEUXUAT");
                return Ok(hoadon);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("XuatKho")]
        public async Task<IActionResult> XuatKho(PhieuNhapXuat body)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                foreach (var x in body.ChiTietNhapXuats)
                {
                    var product = await _context.SanPhams.Include(x => x.SanPhamNavigation).FirstOrDefaultAsync(x => x.MaSanPham == x.MaSanPham);
                    if (product != null)
                    {
                        product.SoLuongDaBan++;
                        product.SoLuongTon--;
                        product.SoLuongCoTheban--;
                        product.SanPhamNavigation.SoLuongCoTheban--;
                        product.SanPhamNavigation.SoLuongTon--;
                        product.SanPhamNavigation.SoLuongDaBan++;
                        _context.Entry(product).State = EntityState.Modified;
                    }
                    else
                    {
                        break;
                    }
                }
                body.DaXuatKho = true;
                body.steps = 4;
                _context.Entry(body).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                trans.Commit(); 
                return Ok(body);

            }
            catch (Exception ex)
            {
                trans.RollbackAsync();
                return BadRequest();
                
            }
        }
        [HttpPut("DaGiaoHang")]
        public async Task<IActionResult> DaGiaoHang(PhieuNhapXuat body)
        {
            try
            {
                body.DaXuatKho = true;
                body.steps = 4;
                body.status = 1;
                _context.Entry(body).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(body);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPut("ThanhToan")]
        public async Task<IActionResult>ThanhToan(PhieuNhapXuat body)
        {
            try
            {
                body.DaThanhToan = true;
                body.status = 1;
                _context.Entry(body).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(body);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPut("HuyDon")]
        public async Task<IActionResult> HuyDon(PhieuNhapXuat body)
        {
            try
            {
                body.LoaiPhieu = "PHIEUHUY";
                body.status = -1;
                _context.Entry(body).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(body);
            }
            catch(Exception err)
                {
                return BadRequest();
            }
        }
    }
}
