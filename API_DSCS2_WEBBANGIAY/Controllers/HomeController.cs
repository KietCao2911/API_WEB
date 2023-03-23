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
        [HttpGet("ProductsLatesUpdate")]
        public async Task<IActionResult> ProductsLatesUpdate(string MaChiNhanh)
        {
            //var khohangs =  _context.KhoHangs.Include(x => x.SanPhamNavigation).Include(x => x.BranchNavigation);
            //var temp = await _context.KhoHangs.Include(x => x.SanPhamNavigation).ThenInclude(x => x.IdBstNavigation)
            //           .Include(x => x.SanPhamNavigation).ThenInclude(x => x.ChiTietHinhAnhs).
            //           ThenInclude(x => x.IdHinhAnhNavigation)
            //           .Include(x => x.SanPhamNavigation).ThenInclude(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs)
            //           .Include(x => x.SanPhamNavigation).ThenInclude(x => x.TypeNavigation)
            //           .Include(x => x.SanPhamNavigation).ThenInclude(x => x.BrandNavigation)
            //           .Include(x => x.SanPhamNavigation).ThenInclude(x => x.DanhMucDetails)
            //    .Include(x => x.BranchNavigation).Where(x => x.SanPhamNavigation.ParentID == null).Where(x=>x.BranchNavigation.MaChiNhanh.Trim()== MaChiNhanh).OrderBy(x => x.SanPhamNavigation.CreatedAt).ToListAsync();
            var products = await _context.SanPhams
                  .Include(x => x.IdBstNavigation).
                       Include(x => x.ChiTietHinhAnhs).
                       ThenInclude(x => x.IdHinhAnhNavigation)
                       .Include(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x=>x.IdHinhAnhNavigation)
                       .Include(x => x.TypeNavigation)
                       .Include(x => x.BrandNavigation)
                       .Include(x => x.DanhMucDetails)
                       .Include(x => x.KhoHangs)
                       .Where(x => x.ParentID == null)
               .OrderBy(x => x.CreatedAt).ToListAsync();
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
        
    }
}
