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
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuNhapsController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public PhieuNhapsController(ShoesEcommereContext context)
        {
            _context = context;
        }

        // GET: api/PhieuNhaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuNhapXuat>>> GetPhieuNhaps()
        {
            return await _context.PhieuNhapXuats.Where(x=>x.LoaiPhieu=="PHIEUNHAP").ToListAsync();
        }

        // GET: api/PhieuNhaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuNhapXuat>> GetPhieuNhap(int id)
        {
            var phieuNhap = await _context.PhieuNhapXuats.Include(x=>x.ChiTietNhapXuats).ThenInclude(x=>x.SanPhamNavigation).Include(x=>x.NhaCungCapNavigation).ThenInclude(x=>x.DiaChiNavigation).FirstOrDefaultAsync(x=>x.Id ==id);
            if (phieuNhap == null)
            {
                return NotFound();
            }
            return phieuNhap;
        }



        // POST: api/PhieuNhaps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        // DELETE: api/PhieuNhaps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhieuNhap(string id)
        {
            var phieuNhap = await _context.PhieuNhapXuats.FindAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            _context.PhieuNhapXuats.Remove(phieuNhap);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("ThanhToan")]
        public async Task<IActionResult> ThanhToan(PhieuNhapXuat body)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
             
                if ((bool)body.DaNhapHang)
                {
                    body.steps = 4;
                    body.status = 1;
                }
                body.DaNhapHang = true;
                _context.Entry(body).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Ok();
            }
            catch (Exception err)
            {
                return BadRequest();
                await trans.RollbackAsync();
            }
        }
        [HttpPost]
        public async Task<ActionResult<PhieuNhapXuat>> PostPhieuNhap(PhieuNhapXuat body)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                if (body.steps <= 2)
                {
                    foreach (var item in body.ChiTietNhapXuats)
                    {
                        var khohang = _context.KhoHangs.FirstOrDefault(x => x.MaSanPham == item.MaSanPham && x.MaChiNhanh == body.MaChiNhanh);
                        item.TenPhieu = "Nhập hàng vào kho";
                        if (khohang is not null)
                        {
                            khohang.SoLuongHangDangVe += item.SoLuong;
                            _context.KhoHangs.Update(khohang);
                        }

                    }
                }

                body.status = 1;
                body.NhaCungCapNavigation = null;
                _context.PhieuNhapXuats.Add(body);

                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Ok(body);
            }
            catch (Exception err)
            {
                return BadRequest(err.InnerException.Message);
                await trans.RollbackAsync();
            }

        }
        [HttpPut("NhapKho")]
        public async Task<IActionResult> NhapKho(PhieuNhapXuat body)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                foreach(var item in body.ChiTietNhapXuats)
                {
                    var khohang = _context.KhoHangs.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);
                    
                    if(khohang is not null)
                    {
                        var sanpham = _context.SanPhams.FirstOrDefault(x => x.MaSanPham == khohang.MaSanPham);
                        khohang.SoLuongTon += item.SoLuong;
                        khohang.SoLuongCoTheban += item.SoLuong;
                        khohang.SoLuongHangDangVe -= item.SoLuong;
                        if(sanpham is not null)
                        {
                            sanpham.SoLuongTon += item.SoLuong;
                            sanpham.SoLuongCoTheban += item.SoLuong;
                            _context.SanPhams.Update(sanpham);
                        }
                        _context.Entry(khohang).State = EntityState.Modified;
                    }
                }
                body.DaNhapHang = true;
                body.steps = 3;
                if((bool)body.DaThanhToan)
                {
                    body.steps = 4;
                    body.status = 1;
                }
                body.ChiTietNhapXuats = null;
                _context.PhieuNhapXuats.Update(body);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Ok();
            }
            catch(Exception err)
            {
                return BadRequest();
                await trans.RollbackAsync();
            }
        }
        

    }
}
