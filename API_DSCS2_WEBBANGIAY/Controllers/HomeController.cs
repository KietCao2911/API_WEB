using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public HomeController(ShoesEcommereContext context)
        {
            _context = context;
        }
        //[HttpGet("ProductByBrand")]
        //public async Task<IActionResult>  ()
        //{
        //    try
        //    {
        //        var res  = _context.Types.Include(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs).
        //               ThenInclude(x => x.IdHinhAnhNavigation).Take(8);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);  
        //    }
        //}
        [HttpGet("ProductsLatesUpdate")]
        public async Task<IActionResult> ProductsLatesUpdate(string MaChiNhanh)
        {

            try
            {
                var products = await _context.SanPhams
                      .Include(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x => x.IdHinhAnhNavigation)
                      .Where(x => x.ParentID == null)
              .OrderByDescending(x => x.CreatedAt).ToListAsync();
                //var select = products.Select(x => new
                //{
                //    Id = x.Id,
                //    MaSanPham = x.MaSanPham.Trim(),
                //    TenSanPham = x.TenSanPham.Trim(),
                //    GiaBan = x.GiaBanLe,
                //    GiamGia = x.GiamGia,
                //    Slug = x.Slug,
                //    BoSuuTap = x.IdBstNavigation,
                //    HinhAnhs = x.ChiTietHinhAnhs.Select(x => new
                //    {
                //        uid = x.IdHinhAnh,
                //        name = x.IdHinhAnhNavigation.FileName,
                //        status = "done",
                //        url = "https:\\localhost:44328\\wwwroot\\res\\SanPhamRes\\Imgs\\" + x.MaSanPham.Trim() + "\\" + x.IdMaMau.Trim() + "\\" + x.IdHinhAnhNavigation.FileName.Trim(),
                //        IdMaMau = x.IdMaMau,
                //    }).GroupBy(x => x.IdMaMau),
                //    SoLuongTon = x.SoLuongTon,
                //    CoTheBan = x.SoLuongCoTheban,
                //    LoaiHang = x.TypeNavigation,
                //    NhanHieu = x.BrandNavigation,
                //    MauSac = x.MauSacNavigation,
                //    KichThuoc = x.SizeNavigation,
                //    IDType = x.IDType,
                //    IDBrand = x.IDBrand,
                //    SanPhams = x.SanPhams,
                //    KhoHangs= x.KhoHangs,
                //}); ; ;
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
