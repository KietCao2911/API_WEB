//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using API_DSCS2_WEBBANGIAY.Models;

//namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
//{
//    [Area("admin")]
//    [Route("api/[area]/[controller]")]
//    [ApiController]

//    public class DonHangController : ControllerBase
//    {
//        private readonly ShoesEcommereContext _context;
//        public DonHangController(ShoesEcommereContext context)
//        {
//            _context = context;
//        }

//        // GET: api/DonHang
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDons()
//        {
//            var hoadons = _context.HoaDons.Include(x => x.ChiTietHoaDons).ThenInclude(x=>x.MasanPhamNavigation).Include(x=>x.DiaChiNavigation).OrderByDescending(x=>x.createdAt);
//            return Ok(hoadons);
//        }
//        [HttpGet("chi-tiet-hoa-don/{ID}")]
//        public async Task<ActionResult<IEnumerable<HoaDon>>> GetChiTietHoaDon(int ID)
//        {
//            var cthds = _context.ChiTietHoaDons.Include(x => x.MasanPhamNavigation).ThenInclude(x=>x.IdBstNavigation)
//                .Include(x => x.MasanPhamNavigation).ThenInclude(x => x.ChiTietHinhAnhs)
//                .Include(x=>x.MausacPhamNavigation).ThenInclude(x=>x.ChiTietHinhAnhs)
//                .ThenInclude(x=>x.IdHinhAnhNavigation)
//                /*.Include(x=>x.SizePhamNavigation)*/.Where(x=>x.IdHoaDon == ID);
//            var hoadon = await _context.HoaDons.Include(x=>x.DiaChiNavigation).FirstOrDefaultAsync(x => x.Id == ID);
//            var details = cthds.Select(x => new
//            {
               
//                IDSanPham = x.MasanPhamNavigation.Id,
//                TenSanPham = x.MasanPhamNavigation.TenSanPham,
//                GiaBan = x.MasanPhamNavigation.GiaBanLe,
//                GiamGia = x.MasanPhamNavigation.GiamGia,
//                Slug = x.MasanPhamNavigation.Slug.Trim(),
//                //BoSuuTap = new { key = x.MasanPhamNavigation.IdBstNavigation.Id, value = x.MasanPhamNavigation.IdBstNavigation.TenBoSuuTap },
//                Qty = x.Qty,
//                Color = x.MasanPhamNavigation.ChiTietHinhAnhs.Where(g=>g.IdMaMau==x.Color).Select(x => new
//                {
//                    color = new
//                    {
//                        sizeLabel = x.MauSacNavigation.TenMau.Trim(),
//                        maMau = x.MauSacNavigation.MaMau.Trim(),
//                    },
//                    uid = x.IdHinhAnh,
//                    name = x.IdHinhAnhNavigation.FileName,
//                    status = "done",
//                    url = "https:\\localhost:44328\\wwwroot\\res\\SanPhamRes\\Imgs\\" + x.IDSanPham + "\\" + x.IdMaMau.Trim() + "\\" + x.IdHinhAnhNavigation.FileName.Trim()
//                }).First(),
//                Size = new
//                {
//                    //Id = x.SizePhamNavigation.Id,
//                    //sizeLabel = x.SizePhamNavigation.Size1,
//                }


//            });; ; ;
//            return Ok(new
//            {
//                hoadon,
//                details
//            });
//        }
//        // GET: api/DonHang/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<HoaDon>> GetHoaDon(int id)
//        {
//            var hoaDon = await _context.HoaDons.FindAsync(id);

//            if (hoaDon == null)
//            {
//                return NotFound();
//            }

//            return hoaDon;
//        }


//        // POST: api/DonHang
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<HoaDon>> PostHoaDon(HoaDon hoaDon)
//        {
//            _context.HoaDons.Add(hoaDon);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetHoaDon", new { id = hoaDon.Id }, hoaDon);
//        }
//        private bool HoaDonExists(int id)
//        {
//            return _context.HoaDons.Any(e => e.Id == id);
//        }
//        [HttpPut]
//        public async Task<IActionResult> PutOrderDetails(ChiTietHoaDon body)
//        {
//            try
//            {
//                _context.ChiTietHoaDons.Update(body);
//                await _context.SaveChangesAsync();
//                var hd = _context.HoaDons.FirstOrDefault(x => x.Id == body.IdHoaDon);
//                var cthd = _context.ChiTietHoaDons.Where(x => x.IdHoaDon == body.IdHoaDon);
               
//                hd.Thanhtien = (decimal)((body.giaBan * cthd.Sum(x => x.Qty))+hd.Phiship);
//                hd.totalQty = (int)(cthd.Sum(x => x.Qty));
//                _context.HoaDons.Update(hd);
//                await _context.SaveChangesAsync();
//                return Ok(new
//                {
//                    hd,body,
//                });
//            }catch (Exception ex)
//            {
//                return BadRequest(ex);
//            }
//        }
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> CancelOrder(int id)
//        {
//            try
//            {
//                var hd = await _context.HoaDons.FindAsync(id);
//                var cthd = _context.ChiTietHoaDons.Where(x => x.IdHoaDon == id);
//                    hd.DeliveryStatus = -1;
//                _context.HoaDons.Update(hd);
//                await _context.SaveChangesAsync();
//                return Ok(id);
//            }
//            catch(Exception ex)
//            {
//                return BadRequest();
//            }
//        }
//    }
//}
