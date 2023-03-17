using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;
        private  IConfiguration _configuration { get;  set; }

        public SanPhamController(ShoesEcommereContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("GetAll/{id}")]
        public async Task<ActionResult> GetSanPhams(string? sort, [FromQuery(Name = "size")] string size, [FromQuery(Name = "color")] string color, int pageSize, int? page, string id, [FromQuery(Name = "s")] string s)
        {
            try
            {
                var baseURL = _configuration.GetSection("BaseURL").Value;
                pageSize = pageSize == 0 ? 5 : pageSize;
                IQueryable<SanPham> products = Enumerable.Empty<SanPham>().AsQueryable();
                var getID = await _context.DanhMucs.FirstOrDefaultAsync(x => x.Slug == id);


                products = _context.SanPhams.
                   Include(x => x.IdBstNavigation).
                   Include(x => x.ChiTietHinhAnhs).
                   ThenInclude(x => x.IdHinhAnhNavigation)
                   .Include(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x=>x.IdHinhAnhNavigation)
                   .Include(x => x.TypeNavigation)
                   .Include(x => x.BrandNavigation)
                   .Include(x => x.VatNavigation)
                   .Include(x => x.DanhMucDetails)
                   .Where(x => x.DanhMucDetails.Any(x => x.danhMucId == getID.Id)).Where(x => x.ParentID == null);
                if (s is not null && s.Length > 0)
                {
                    products = products.Where(x => x.TenSanPham.Trim().ToLower().Contains(s.Trim().ToLower()));
                }
                if (size is not null && size.Length > 0)
                {
                    products = products.Where(x => x.SanPhams.Any(x => x.IDSize == size));
                }
                if (color is not null && color.Length > 0)
                {
                    products = products.Where(x => x.SanPhams.Any(x => x.IDColor == color));
                }
                switch (sort)
                {
                    case "price-hight-to-low":
                        products = products.OrderByDescending(s => s.GiaBanLe);
                        break;
                    case "date-oldest":
                        products = products.OrderBy(s => s.CreatedAt);
                        break;
                    case "date-newest":
                        products = products.OrderByDescending(s => s.CreatedAt);
                        break;
                    default:
                        products = products.OrderBy(s => s.GiaBanLe);
                        break;
                }
                var result = await PaggingService<SanPham>.CreateAsync((IQueryable<SanPham>)products, page ?? 1, pageSize);
                var select = result.Select(x => new
                {
                    Id = x?.Id,
                    MaSanPham = x.MaSanPham.Trim(),
                    TenSanPham = x?.TenSanPham.Trim(),
                    GiaBanLe = x?.GiaBanLe,
                    GiamGia = x?.GiamGia,
                    Slug = x?.Slug,
                    IdBstNavigation = x.IdBstNavigation,
                    IDVAT = x.IDVat,
                    VatNavigation = x?.VatNavigation,
                    ChiTietHinhAnhs = x?.ChiTietHinhAnhs.Select(x => new
                    {
                        uid = x.IdHinhAnh,
                        name = x.IdHinhAnhNavigation.FileName,
                        status = "done",
                        url = baseURL+"\\wwwroot\\res\\SanPhamRes\\Imgs\\" + x.MaSanPham.Trim() + "\\" + x.IdMaMau.Trim() + "\\" + x.IdHinhAnhNavigation.FileName.Trim(),
                        IdMaMau = x.IdMaMau,
                    }).GroupBy(x => x.IdMaMau),
                    SoLuongTon = x.SoLuongTon,
                    SoLuongCoTheban = x.SoLuongCoTheban,
                    TypeNavigation = x?.TypeNavigation,
                    BrandNavigation = x?.BrandNavigation,
                    MauSacNavigation = x?.MauSacNavigation,
                    SizeNavigation = x?.SizeNavigation,
                    IDType = x?.IDType,
                    IDBrand = x?.IDBrand,
                    SanPhams = x.SanPhams,

                }); ; ;
                return Ok(new
                {
                    products,
                    totalRow = products.Count(),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("Get/{slug}")]
        public async Task<ActionResult<SanPham>> GetSanPham(string slug)
        {
            var baseURL = _configuration.GetSection("BaseURL").Value;
            var sanPham = await _context.SanPhams.

                      Include(x => x.IdBstNavigation).Include(x => x.SanPhams).
                      ThenInclude(x => x.SizeNavigation)
                      .Include(x => x.SanPhams)
                      .ThenInclude(x => x.MauSacNavigation)
                       .Include(x => x.SanPhams)
                       .ThenInclude(x => x.KhoHangs).ThenInclude(x => x.BranchNavigation)
                      .Include(x => x.SanPhams).ThenInclude(x => x.ChiTietNhapXuats).
                      Include(x => x.SanPhams).ThenInclude(x => x.ChiTietNhapXuats).ThenInclude(x => x.PhieuNhapXuatNavigation)
                      .Include(x => x.SanPhams).ThenInclude(x=>x.ChiTietHinhAnhs).ThenInclude(x=>x.IdHinhAnhNavigation)
                       .Include(x => x.VatNavigation)
                      .Include(x => x.TypeNavigation)
                      .Include(x => x.BrandNavigation).
                       Include(x => x.ChiTietHinhAnhs)
                       .ThenInclude(x => x.IdHinhAnhNavigation)
                       .FirstOrDefaultAsync(x => x.Slug.Trim() == slug.Trim() && x.ParentID == null);
            if (sanPham == null)
            {
                return NotFound();
            }

            return Ok(sanPham); ; ;
        }

    }
}

