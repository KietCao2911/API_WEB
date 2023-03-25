using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils;
using System.IO;
using API_DSCS2_WEBBANGIAY.BodyRequest.SanPhamController;
using Microsoft.Extensions.Configuration;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;
        private IConfiguration _configuration { get; set; }
        public SanPhamController(ShoesEcommereContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/SanPham
        [HttpGet]

        public async Task<ActionResult> GetSanPhams(string? sort, int pageSize, int? page, [FromQuery(Name = "s")]  string s)
        {

            try
            {
                pageSize = pageSize == 0 ? 20: pageSize;
                IQueryable<SanPham> products = Enumerable.Empty<SanPham>().AsQueryable();
                products = _context.SanPhams.
                      Include(x => x.ChiTietHinhAnhs).
                      ThenInclude(x => x.IdHinhAnhNavigation)
                      .Include(x => x.SanPhams).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x=>x.IdHinhAnhNavigation)
                      .Where(x => x.ParentID == null).OrderBy(x=>x.CreatedAt);
                if (s is not null && s.Length > 0)
                {
                    products = products.Where(x => x.TenSanPham.Trim().ToLower().Contains(s.Trim().ToLower()));
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
                    MaSanPham=x.MaSanPham.Trim(),
                    TenSanPham = x?.TenSanPham.Trim(),
                    GiaBan = x?.GiaBanLe,
                    GiamGia = x?.GiamGia,
                    Slug = x?.Slug,
                    IDVAT = x.IDVat,
                    VatNavigation = x?.VatNavigation,
                    ChiTietHinhAnhs = x?.ChiTietHinhAnhs.Select(x => new
                    {
                        uid = x.IdHinhAnh,
                        name = x.IdHinhAnhNavigation.FileName,
                        status = "done",
                        url = "https:\\localhost:44328\\wwwroot\\res\\SanPhamRes\\Imgs\\" + x.MaSanPham.Trim() + "\\" + x.IdMaMau.Trim() + "\\" + x.IdHinhAnhNavigation.FileName.Trim(),
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
                    SanPhams= x.SanPhams,
                    
                }); ; ;
                return Ok(new
                {
                    products=select.Take(pageSize),
                    totalRow = products.Count(),
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/SanPham/5
        [HttpGet]
        [Route("san-pham/{slug}")]
        public async Task<ActionResult<SanPham>> GetSanPham(string slug)
        {
            var sanPham = await _context.SanPhams

                      .Include(x => x.SanPhams).
                      ThenInclude(x=>x.SizeNavigation)
                      .Include(x=>x.DanhMucDetails)
                      .Include(x => x.SanPhams)
                      .ThenInclude(x => x.MauSacNavigation)
                       .Include(x => x.SanPhams)
                       .ThenInclude(x=>x.KhoHangs).ThenInclude(x=>x.BranchNavigation)
                      .Include(x => x.SanPhams).ThenInclude(x=>x.ChiTietNhapXuats).
                      Include(x => x.SanPhams).ThenInclude(x => x.ChiTietNhapXuats).ThenInclude(x=>x.PhieuNhapXuatNavigation).
                      Include(x => x.SanPhams)
                       .ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x => x.IdHinhAnhNavigation)
                       .Include(x => x.VatNavigation)
                      .Include(x => x.TypeNavigation)
                      .Include(x => x.BrandNavigation).
                       Include(x => x.ChiTietHinhAnhs)
                       .ThenInclude(x => x.IdHinhAnhNavigation)
                       .FirstOrDefaultAsync(x=>x.Slug.Trim()==slug.Trim()&&x.ParentID==null);
            if (sanPham == null)
            {
                return NotFound();
            }

            return Ok(sanPham);; ;
        }

        // PUT: api/SanPham/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutSanPham( SanPham sanPham)
        {
            try
            {
                var product = _context.SanPhams.Include(x => x.DanhMucDetails).AsNoTracking().FirstOrDefault(x => x.MaSanPham == sanPham.MaSanPham);
                if (product.DanhMucDetails.Count > 0)
                {
                    bool check = IsAny(product.DanhMucDetails.ToList(), sanPham.DanhMucDetails.ToList());
                    if (!check)
                    {
                        
                        //_context.DanhMucDetails.RemoveRange(product.DanhMucDetails);
                        foreach(var removeItem in product.DanhMucDetails)
                        {

                        _context.Entry(removeItem).State = EntityState.Deleted;
                        }
                        foreach (var addItem in sanPham.DanhMucDetails)
                        {

                            _context.Entry(addItem).State = EntityState.Added;
                        }
                        //_context.Entry(sanPham.DanhMucDetails).State = EntityState.Added;
                        //_context.DanhMucDetails.AddRange(sanPham.DanhMucDetails);
                    }
                }
                else
                {
                    _context.DanhMucDetails.AddRange(sanPham.DanhMucDetails);
                }
                foreach (var item in sanPham.SanPhams)
                {
                    item.SanPhamNavigation = null;
                    item.ChiTietNhapXuats = null;
                    item.KhoHangs = null;
                    item.DanhMucDetails = null;
                    _context.Entry(item).State = EntityState.Modified;
                }
                _context.Entry(sanPham).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(sanPham);
            }
            catch(Exception err)
            {
                return BadRequest(err.Message);
            }
        }
        private bool IsAny(List<DanhMucDetails> s1, List<DanhMucDetails>   s2)
        {
            if(s1.Count!=s2.Count)
            {
                return false;
            }
            for (int i = 0;i < s1.Count;i++)
            {
                if (s1[i].danhMucId != s2[i].danhMucId)
                {
                    return false;
                }
            }
            return true;
        }
        // POST: api/SanPham
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SanPham>> PostSanPham(SanPham body)
        {
            var trans = _context.Database.BeginTransaction();
            ChiNhanh_SanPham khohang = new ChiNhanh_SanPham();
            try
            {

                if (body.MaSanPham == null || body.MaSanPham.Length == 0)
                {
                    var ID = new GenKey();
                    _context.Keys.Add(ID);
                    await _context.SaveChangesAsync();
                    body.MaSanPham = "CTK0" + ID.ID.ToString();
                    body.Slug = CustomSlug.Slugify(body.TenSanPham) + "_" + body.MaSanPham;
                }
                
                var products = body.SanPhams.ToList();
               if(products.Count>0)
                {
                    for (int i = 0; i < body.SanPhams.Count; i++)
                    {
                        if (products[i].MaSanPham == null || products[i].MaSanPham.Length == 0)
                        {
                            var ID = new GenKey();
                            _context.Keys.Add(ID);
                            await _context.SaveChangesAsync();
                            //products[i].IDVat = body.IDVat;
                            //products[i].IDBrand = body.IDBrand;
                            //products[i].IDType = body.IDType;
                            products[i].MaSanPham = "CTK0" + ID.ID.ToString();
                            products[i].ParentID = body.MaSanPham;
                            products[i].Slug = CustomSlug.Slugify(products[i].TenSanPham) + "_" + products[i].MaSanPham;

                        }
                    }
                }
               
                _context.SanPhams.Add(body);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Ok(body);
            }
            catch (Exception err)
            {
                await  trans.RollbackAsync();
                return BadRequest(err);
            }

        }

        // DELETE: api/SanPham/5
        [HttpDelete("{maSanPham}")]
        public async Task<IActionResult> DeleteSanPham(string maSanPham)
        {
            var sanPham = await _context.SanPhams.Include(x=>x.SanPhams).FirstOrDefaultAsync(x=>x.MaSanPham== maSanPham);
           
            if (sanPham == null)
            {
                return NotFound();
            }
            try {
                var path = Path.Combine(
                  Directory.GetCurrentDirectory(), "wwwroot//res//SanPhamRes//Imgs//" + sanPham.MaSanPham.Trim());
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                _context.SanPhams.RemoveRange(sanPham.SanPhams);
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.Id == id);
        }
        [HttpPost("Upload-Single")]
        public async Task<IActionResult> UploadSingle(IFormFile file,int id)
        {
            var files = Request.Form.Files;
            if (file != null)
            {
                var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot//res//SanPhamRes//Thumb",
                file.FileName);
                if(Directory.Exists(path))
                {
                    Directory.Delete(path);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                        var product = _context.SanPhams.FirstOrDefault(x => x.Id == id);
                        if (product != null)
                        {
                            var res = await _context.SaveChangesAsync();
                            if (res > 0)
                            {
                                return Ok(new
                                {
                                    success = true,
                                    img = file.FileName,

                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    success = false,


                                });
                            }
                        }

                    }
                    catch (Exception err)
                    {
                        return BadRequest(new
                        {
                            success = false,
                           
                        });
                    }

                }
            }
           
            return BadRequest();
        }
        [HttpDelete("RemoveImg")]
        public async Task<IActionResult>RemoveImg(string fileName, int _id,string maSP,string maMau)
        {
            if (fileName != null)
            {
                var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot//res//SanPhamRes//Imgs//" + maSP + "//" + maMau + "//",
                fileName);
                
                FileInfo file = new FileInfo(path);    
                try
                {
                    if (file.Exists)
                    {
                        file.Delete();
                        var hinhAnh = await _context.HinhAnhs.FindAsync(_id);
                        if (hinhAnh != null)
                        {
                            _context.HinhAnhs.Remove(hinhAnh);
                            await _context.SaveChangesAsync();
                            return Ok(new
                            {
                                success = true,
                                path = path
                            }); ;
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest();

                    }
                }
                catch(Exception err)
                {
                    return BadRequest();
                }

            }
            else
            {

            return BadRequest();
            }
        }
        [HttpPost("Upload/{MaSP}/{MaMau}")]
        public async Task<IActionResult> Upload(IFormFile file, string maSP,string MaMau)
        {
            if (file is null) return BadRequest();

            var folder = "wwwroot/res/SanPhamRes/Imgs/" + maSP.Trim() + "/" + MaMau + "/";
            var path = Path.Combine(
      Directory.GetCurrentDirectory(), folder,
      file.FileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(folder);
            }
            if (System.IO.File.Exists(path))
            {
                return BadRequest("Đã tồn tại hình ảnh này!");
            }
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var baseURL = _configuration.GetSection("BaseURL").Value;
                var trans = _context.Database.BeginTransaction();
                try
                {
                    var sanPhamRoot = _context.SanPhams.Include(x=>x.SanPhams).FirstOrDefault(x=>x.MaSanPham==maSP);
                    if(sanPhamRoot!=null&&sanPhamRoot.SanPhams.Count>0)
                    {
                        var colors =  sanPhamRoot.SanPhams.Where(x => x.IDColor.Trim() == MaMau);
                        if(colors.Count()>0)
                        {
                            
                                HinhAnh anh = new HinhAnh();
                                anh.FileName = file.FileName;
                            _context.HinhAnhs.Add(anh);
                            await _context.SaveChangesAsync();

                            foreach (var color in colors)
                            {
                                ChiTietHinhAnh hinhAnhSanPham = new ChiTietHinhAnh();
                                hinhAnhSanPham.IdHinhAnh = anh.Id;
                                hinhAnhSanPham.MaSanPham = color.MaSanPham;
                                hinhAnhSanPham.IdMaMau = MaMau;
                                _context.ChiTietHinhAnhs.Add(hinhAnhSanPham);
                            }
                            await file.CopyToAsync(stream);
                            await _context.SaveChangesAsync();
                            await trans.CommitAsync();
                            return Ok(new
                            {
                                uid= anh.Id,
                                name = anh.FileName,
                                status="done",
                                url = baseURL + "\\wwwroot\\res\\SanPhamRes\\Imgs\\" + maSP.Trim() + "\\" + MaMau.Trim() + "\\" +anh.FileName.Trim()

                            });;
                        }
                    }
                    return BadRequest();

                  
                    

                }
                catch (Exception err)
                {
                   await  trans.RollbackAsync();
                    return BadRequest(new
                    {
                        success = false
                    });
                }

            }



        }
        [HttpDelete("Remove-Mutiple")]
        public async Task<IActionResult> RemoveMutiple(string fileName, string _id)
        {
            if (fileName != null)
            {
                var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot//res//SanPhamRes//Imgs//"+_id+"//",
                fileName);

                FileInfo file = new FileInfo(path);
                    if (file.Exists)
                {
                   
                    var img = await _context.HinhAnhs.FirstOrDefaultAsync(x => x.FileName == fileName);
                    if (img != null)
                    {
                        _context.HinhAnhs.Remove(img);
                        try
                        {
                            await _context.SaveChangesAsync();
                            file.Delete();
                            return Ok(new
                            {
                                success = true,
                                path = path
                            }); ;
                        }
                        catch(Exception err)
                        {
                            return BadRequest();
                        }
                      
                    }

                }
            }
            return BadRequest();
        }
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(List<DanhMucDetails> dm)
        {
            try
            {
               foreach(var item in dm)
                {
                    _context.DanhMucDetails.Add(item);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            catch(Exception err)
            {
                return BadRequest(err.Message);
            }
            
        }



    }
}
