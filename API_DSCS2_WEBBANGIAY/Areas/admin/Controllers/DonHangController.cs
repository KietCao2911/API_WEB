using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils.Mail.TemplateHandle;
using Microsoft.AspNetCore.Hosting;
using API_DSCS2_WEBBANGIAY.Utils.Mail;
using Microsoft.Extensions.Options;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]

    public class DonHangController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IOptions<MailSettings> mailSettings;
        public DonHangController(ShoesEcommereContext context, IHostingEnvironment hostingEnvironment, IOptions<MailSettings> mailSettings)
        {
            _context = context;
            this._HostEnvironment = hostingEnvironment;
            this.mailSettings = mailSettings;
        }

        // GET: api/DonHang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuNhapXuat>>> GetHoaDons([FromQuery(Name = "status")] string status)
        {
            try
            {
                IQueryable<PhieuNhapXuat> phieuxuats = Enumerable.Empty<PhieuNhapXuat>().AsQueryable();
                phieuxuats = _context.PhieuNhapXuats.Include(x => x.ChiTietNhapXuats)
                    .ThenInclude(x => x.SanPhamNavigation).Include(x => x.DiaChiNavigation).Where(x=> x.LoaiPhieu == "PHIEUXUAT");
                switch (status)
                {
                    case "DaHuy":
                        phieuxuats = phieuxuats.Where(x => x.status == -1);
                        break;
                    case "HoanThanh":
                        phieuxuats = phieuxuats.Where(x => x.status == 1);
                        break;
                    default:
                        phieuxuats = phieuxuats.Where(x => x.status == 0);
                        break;

                }
                return Ok(phieuxuats);
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
                List<SanPham> parents = new List<SanPham>();
                foreach (var x in body.ChiTietNhapXuats)
                {
                    var product = x.SanPhamNavigation;
                    var khohang = await _context.KhoHangs.Include(x => x.SanPhamNavigation).FirstOrDefaultAsync(k => k.MaSanPham.Trim() == x.MaSanPham.Trim()&&k.MaChiNhanh.Trim() == body.MaChiNhanh.Trim());
                    if (product != null&&khohang !=null)
                    {
                        //KhoHang update
                        if(khohang.SoLuongCoTheban>=x.SoLuong&&khohang.SoLuongTon>=x.SoLuong)
                        {
                            khohang.SoLuongTon -=x.SoLuong;
                            khohang.SoLuongCoTheban-=x.SoLuong;
                        }
                        else
                        {
                            return BadRequest("Số lượng hàng trong kho không cho phép");
                        }
                        //SanPham Update
                        if(product.SoLuongTon>=x.SoLuong&&product.SoLuongCoTheban>=x.SoLuong)
                        {
                            product.SoLuongDaBan+=x.SoLuong;
                            product.SoLuongTon-= x.SoLuong;
                            product.SoLuongCoTheban-= x.SoLuong;
                        }
                        else
                        {
                            return BadRequest("Số lượng hàng trong kho không cho phép");
                        }
                        //Parent Update
                        if(product is not null && product.SoLuongTon >= 0 && product.SoLuongCoTheban >= 0)
                        {
                            var parent = _context.SanPhams.FirstOrDefault(x => x.MaSanPham == x.SanPhamNavigation.ParentID);
                            if (parent is not null && parents.Any(x => x.MaSanPham == parent.MaSanPham))
                            {
                                var pro = parents.FirstOrDefault(x => x.MaSanPham == parent.MaSanPham);
                                if (pro != null)
                                {
                                    var index = parents.IndexOf(pro);
                                    if (index >= 0)
                                    {
                                        parents[index].SoLuongCoTheban -= x.SoLuong;
                                        parents[index].SoLuongTon -= x.SoLuong;
                                    }
                                }
                            }
                            else
                            {
                                if (parent is not null)
                                {
                                    parent.SoLuongCoTheban -= x.SoLuong;
                                    parent.SoLuongTon -= x.SoLuong;
                                    parents.Add(parent);
                                }
                            }

                        }
                        else
                        {
                            return BadRequest();
                        }
                        _context.Entry(product).State = EntityState.Modified;
                        _context.Entry(khohang).State = EntityState.Modified;
                    }
             
                    else
                    {
                        break;
                    }
                   
                }
                body.DaXuatKho = true;
                body.steps = 4;
                _context.Entry(body).State = EntityState.Modified;
                _context.SanPhams.UpdateRange(parents);
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
                var bodyString = await new RatingStar(_HostEnvironment,body).RenderBody("");
                var mailBody = new MailRequest()
                {
                    Body = bodyString.ToString(),
                    Subject = "XÁC NHẬN ĐƠN HÀNG",
                    ToEmail = body.DiaChiNavigation?.Email,
                };
                var mailSend = new MailService(mailSettings);
                mailSend.SendEmailAsync(mailBody);
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
                body.TienDaThanhToan = body.ThanhTien;
                body.status = 1;
                _context.Entry(body).State = EntityState.Modified;
                PhieuNhapXuat phieuThu = new PhieuNhapXuat();
                phieuThu.TienDaThanhToan = body.ThanhTien;
                phieuThu.LoaiPhieu = "PHIEUTHU";
                phieuThu.PhuongThucThanhToan = body.PhuongThucThanhToan;
                phieuThu.MaChiNhanh = body.MaChiNhanh;
                _context.PhieuNhapXuats.Add(phieuThu);
                await _context.SaveChangesAsync();
                return Ok(body);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPut("HoanTien")]
        public async Task<IActionResult> HoanTien(PhieuNhapXuat body)
        {
            try
            {
                if ((bool)!body.DaThanhToan)
                {
                    return BadRequest("Không thể hoàn tiền khi chưa thanh toán");
                }
                body.TienDaThanhToan -= body.ThanhTien;
                PhieuNhapXuat phieuChi = new PhieuNhapXuat();
                phieuChi.TienDaThanhToan = body.ThanhTien;
                phieuChi.LoaiPhieu = "PHIEUCHI";
                phieuChi.PhuongThucThanhToan = body.PhuongThucThanhToan;
                phieuChi.MaChiNhanh = body.MaChiNhanh;
                _context.PhieuNhapXuats.Add(phieuChi);
                await _context.SaveChangesAsync();
                return Ok(body);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public async Task<IActionResult> PutHoaDon(PhieuNhapXuat body)
        {
            try
            {
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
        public async Task<IActionResult> HuyDon(PhieuNhapXuat body )
        {
            try
            {
                body.status = -1;
                _context.Entry(body).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(body); 
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPut("TraHang")]
        public async Task<IActionResult>  TraHang(PhieuNhapXuat body)
        {
                var trans = _context.Database.BeginTransaction();
            try
            {
                List<SanPham> parents = new List<SanPham>();
                foreach (var item in body.ChiTietNhapXuats)
                {
                    var khohang = _context.KhoHangs.FirstOrDefault(x => x.MaSanPham == item.MaSanPham && x.MaChiNhanh == body.MaChiNhanh);
                    if (khohang is not null && khohang.SoLuongTon - item.SoLuong == 0 && khohang.SoLuongCoTheban - item.SoLuong == 0)
                    {

                        var sanpham = item.SanPhamNavigation;
                        khohang.SoLuongTon -= item.SoLuong;
                        khohang.SoLuongCoTheban -= item.SoLuong;
                        if (sanpham is not null && sanpham.SoLuongTon >= 0 && sanpham.SoLuongCoTheban >= 0)
                        {
                            sanpham.SoLuongTon += item.SoLuong;
                            sanpham.SoLuongCoTheban += item.SoLuong;
                            //
                            var parent = _context.SanPhams.FirstOrDefault(x => x.MaSanPham == item.SanPhamNavigation.ParentID);
                            if (parent is not null && parents.Any(x => x.MaSanPham == parent.MaSanPham))
                            {
                                var pro = parents.FirstOrDefault(x => x.MaSanPham == parent.MaSanPham);
                                if (pro != null)
                                {
                                    var index = parents.IndexOf(pro);
                                    if (index >= 0)
                                    {
                                        parents[index].SoLuongCoTheban += item.SoLuong;
                                        parents[index].SoLuongTon += item.SoLuong;
                                    }
                                }
                            }
                            else
                            {
                                if (parent is not null)
                                {
                                    parent.SoLuongCoTheban += item.SoLuong;
                                    parent.SoLuongTon += item.SoLuong;
                                    parents.Add(parent);
                                }
                            }

                            _context.Entry(sanpham).State = EntityState.Modified;
                            _context.Entry(khohang).State = EntityState.Modified;
                        }
                        else
                        {
                            return BadRequest();
                        }
                        _context.ChiTietNhapXuats.Remove(item);
                        body.ChiTietNhapXuats.Remove(item);
                    }
                    else if (khohang is not null && khohang.SoLuongTon - item.SoLuong >= 0 && khohang.SoLuongCoTheban - item.SoLuong >= 0)
                    {
                        var sanpham = item.SanPhamNavigation;
                        khohang.SoLuongTon += item.SoLuong;
                        khohang.SoLuongCoTheban += item.SoLuong;
                        khohang.SoLuongHangDangVe += item.SoLuong;
                        if (sanpham is not null && sanpham.SoLuongTon >= 0 && sanpham.SoLuongCoTheban >= 0)
                        {
                            sanpham.SoLuongTon += item.SoLuong;
                            sanpham.SoLuongCoTheban += item.SoLuong;
                            //
                            var parent = _context.SanPhams.FirstOrDefault(x => x.MaSanPham == item.SanPhamNavigation.ParentID);
                            if (parent is not null && parents.Any(x => x.MaSanPham == parent.MaSanPham))
                            {
                                var pro = parents.FirstOrDefault(x => x.MaSanPham == parent.MaSanPham);
                                if (pro != null)
                                {
                                    var index = parents.IndexOf(pro);
                                    if (index >= 0)
                                    {
                                        parents[index].SoLuongCoTheban += item.SoLuong;
                                        parents[index].SoLuongTon += item.SoLuong;
                                    }
                                }
                            }
                            else
                            {
                                if (parent is not null)
                                {
                                    parent.SoLuongCoTheban += item.SoLuong;
                                    parent.SoLuongTon += item.SoLuong;
                                    parents.Add(parent);
                                }
                            }

                            _context.Entry(sanpham).State = EntityState.Modified;
                            _context.Entry(khohang).State = EntityState.Modified;
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }
 
                _context.SanPhams.UpdateRange(parents);
                _context.Entry(body).State = EntityState.Modified;
                body.status = -1;
                body.DaTraHang =true;
                _context.SaveChanges();

                _context.Entry(body).State = EntityState.Modified;
                await trans.CommitAsync();
                await _context.SaveChangesAsync();
                return Ok(body);
            }
            catch (Exception err)
                {
                trans.RollbackAsync();  
                return BadRequest();
            }
        }
    }
}
