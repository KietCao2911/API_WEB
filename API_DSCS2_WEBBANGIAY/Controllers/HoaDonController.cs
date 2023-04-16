using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils;
using API_DSCS2_WEBBANGIAY.Utils.Mail;
using API_DSCS2_WEBBANGIAY.Utils.Mail.TemplateHandle;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API_DSCS2_WEBBANGIAY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {

        private readonly ShoesEcommereContext _context;
        private readonly IMailService mailService;
        private readonly IConfiguration _configuration;
        private readonly IOptions<MailSettings> mailSettings;
        private readonly IHostingEnvironment _HostEnvironment;

        public HoaDonController(ShoesEcommereContext context, IMailService mailService, IConfiguration configuration, IOptions<MailSettings> mailSettings,IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.mailService = mailService;
            _configuration = configuration;
            this.mailSettings = mailSettings;
            this._HostEnvironment = hostingEnvironment;
        }

        private async Task<IActionResult> VNPAY(PhieuNhapXuat HoaDon)
        {
            
            //Get Config Info
            string vnp_Returnurl = _configuration["VNPAYConfigs:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VNPAYConfigs:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VNPAYConfigs:vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = _configuration["VNPAYConfigs:vnp_HashSecret"]; //Chuoi bi mat
            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                //lblMessage.Text = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config";
                //return;
            }
            //Get payment input
            VNPay vnpay = new VNPay();
            vnpay.AddRequestData("vnp_Version", VNPay.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (HoaDon.ThanhTien * 100).ToString());
            //vnpay.AddRequestData("vnp_BankCode", body.BankCode?? "NCB");
            vnpay.AddRequestData("vnp_CreateDate", HoaDon.createdAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            var ipAddressParams = HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");
            var remote_ADDR = HttpContext.GetServerVariable("REMOTE_ADDR"); ;
            vnpay.AddRequestData("vnp_IpAddr", Utils.Utils.GetIpAddress(ipAddressParams, remote_ADDR));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + HoaDon.Id);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", HoaDon.Id.ToString());
            //Add Params of 2.1.0 Version
            //vnpay.AddRequestData("vnp_ExpireDate", txtExpire.Text);
            //info
            vnpay.AddRequestData("vnp_Bill_Mobile", HoaDon.DiaChiNavigation.Phone.Trim());
            vnpay.AddRequestData("vnp_Bill_Email", HoaDon.DiaChiNavigation.Email.Trim());
            vnpay.AddRequestData("vnp_Bill_Address", $"({HoaDon.DiaChiNavigation.AddressDsc}), {HoaDon.DiaChiNavigation.WardName}, {HoaDon.DiaChiNavigation.DistrictName}, {HoaDon.DiaChiNavigation.ProvinceName}");
            vnpay.AddRequestData("vnp_Bill_City", HoaDon.DiaChiNavigation.ProvinceID.ToString());
            vnpay.AddRequestData("vnp_Bill_Country", HoaDon.DiaChiNavigation.DistrictID.ToString());
            vnpay.AddRequestData("vnp_Bill_State", HoaDon.DiaChiNavigation.WardID.ToString());
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new
            {
                redirect=paymentUrl,
            });
        }
        private decimal CouponVerify(decimal thanhTien,string MaCoupon)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(x=>x.MaCoupon == MaCoupon);
                if(coupon==null)
                {
                    return 0;
                }
                else
                {
                    //0 = % : 1 = giá trị
                    if(coupon.KieuGiaTri)
                    {
                        //gia tri
                        return coupon.GiaTri;
                    }
                    else
                    {
                        //%
                        if(coupon.GiaTri<100&&coupon.GiaTri>0)
                        {
                            var temp = thanhTien * (coupon.GiaTri / 100);
                            return temp;
                        }
                        return 0;
                    }
                }
            }catch (Exception ex)
            {
                return 0;
            }
        }
        private bool CheckQTY(PhieuNhapXuat body)
        {
            try
            {
                foreach(var k in body.ChiTietNhapXuats)
                {
                    var kh = _context.KhoHangs.FirstOrDefault(x => x.MaSanPham == k.MaSanPham && x.MaChiNhanh == body.MaChiNhanh);
                    if(kh != null)
                    {
                        if(kh.SoLuongTon-k.SoLuong<0||kh.SoLuongCoTheban-k.SoLuong<0)
                        {
                            return false;
                        }
                        
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }catch(Exception err)
            {
                return false;
            }
        }
        [HttpGet("Coupon")]
        public async Task<IActionResult> GetCoupons()
        {
            try
            {
                var coupons = _context.Coupons;
                return Ok(coupons);
            }
            catch (Exception err)
            {
                return BadRequest(err);
            }
        }
        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(PhieuNhapXuat body)
        {
            var couponCode = body.CouponCode;
            if (couponCode == null) return NotFound();
            return Ok();
        }
        [HttpGet("VNPAY_RETURN")]
        public async Task<IActionResult> VNPAY_RETURN()
        {
            var vnpayData = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            string vnp_HashSecret = _configuration["VNPAYConfigs:vnp_HashSecret"]; //Chuoi bi mat
            VNPay vnpay = new VNPay();
            foreach (string s in vnpayData)
            {
                //get all querystring data
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, vnpayData[s]);
                }
            }
            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            String vnp_SecureHash = vnpayData["vnp_SecureHash"];
            String TerminalID = vnpayData["vnp_TmnCode"];
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            String bankCode = vnpayData["vnp_BankCode"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                var hd = _context.PhieuNhapXuats.FirstOrDefault(x => x.Id == orderId);
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    hd.status = 1;
                    hd.TienDaThanhToan = vnp_Amount;
                    _context.PhieuNhapXuats.Update(hd);
                    await _context.SaveChangesAsync();
                    return Content("<h1>Thanh toán thành công</h1>");
                }
                else
                {
                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                    _context.PhieuNhapXuats.Remove(hd);
                    await _context.SaveChangesAsync();
                    return BadRequest();
                }
            }
                return Ok();
        }
        //[HttpPost("PostWithUser")]
        //public async Task<IActionResult> PostWithUser([FromBody] DonhangModel body)
        //{
        //    if (body.HoaDon.PhuongThucThanhToan == "COD")
        //    {
        //        var HoaDon = await CreateOrder(body);
        //        if (HoaDon == null)
        //        {
        //            return BadRequest();
        //        }
        //        List<ChiTietHoaDon> cthd = new List<ChiTietHoaDon>();
        //        foreach (var item in body.hoaDonDetails)
        //        {

        //            var chitietHoaDon = _context.ChiTietHoaDons.Include(x => x.MasanPhamNavigation).Include(x => x.SizePhamNavigation)
        //                .Include(x => x.MausacPhamNavigation).ThenInclude(x => x.ChiTietHinhAnhs).ThenInclude(x => x.IdHinhAnhNavigation).FirstOrDefault(x => x.IdHoaDon == item.IdHoaDon);
        //            cthd.Add(chitietHoaDon);
        //        }
        //        await mailService.SendEmailAsync(new MailRequest { ToEmail = "truongkiet.hn289@gmail.com", Subject = "Xác nhận đơn hàng", Body = FillData.Teamplate(HoaDon, cthd) });
        //        return Ok();
        //    }
        //    else
        //    {
        //        var HoaDon = await CreateOrder(body);
        //        var tk = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan.Trim() == HoaDon.HoaDon.idTaiKhoan.Trim());
        //        tk.TienThanhToan = HoaDon.HoaDon.TienThanhToan;
        //        _context.TaiKhoans.Update(tk);
        //        await _context.SaveChangesAsync();
        //        return await VNPAY(HoaDon);
        //    }
        //        return Ok();
        //}
        private PhieuNhapXuat order(PhieuNhapXuat body)
        {
            try
            {
                body.steps = 1;
                foreach (var item in body.ChiTietNhapXuats)
                {
                    _context.Entry(item).State = EntityState.Added;

                }

                _context.Entry(body).State = EntityState.Added;
                if(body.IdDiaChi is  null)
                {
                    _context.Entry(body.DiaChiNavigation).State = EntityState.Added;
                }
                _context.SaveChanges();
                return body;
            }
            catch (Exception ex)
            {
                return null ;
            }
        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(PhieuNhapXuat body)
        {
            try
            {

                if (CheckQTY(body) == false)
                {
                    return BadRequest("Số lượng trong kho không cho phép");
                }
                if(body.CouponCode is not null)
                {
                    body.ThanhTien= CouponVerify((decimal)body.ThanhTien, body.CouponCode);  
                }
                if (body.PhuongThucThanhToan == "COD")
                {
                    var res = order(body);
                   var bodyString =await new Confirm(_HostEnvironment,body).RenderBody();
                    var mailBody = new MailRequest()
                    {
                        Body = bodyString.ToString(),
                        Subject = "XÁC NHẬN ĐƠN HÀNG",
                        ToEmail = body.DiaChiNavigation?.Email,
                    };
                    var mailSend = new MailService(mailSettings);
                     mailSend.SendEmailAsync(mailBody);

                    return Ok(body);
                }
                else if(body.PhuongThucThanhToan=="VNPAY")
                {
                    var hoadon = order(body);
                    await _context.SaveChangesAsync();
                    return await VNPAY(hoadon);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut("UpdateProductOrder/{id}")]
        public async Task<IActionResult> UpdateProductOrder(PhieuNhapXuat body)
        {
            try
            {
                _context.PhieuNhapXuats.Update(body);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception err)
            {
                return BadRequest();
            }
        }

    }
}
