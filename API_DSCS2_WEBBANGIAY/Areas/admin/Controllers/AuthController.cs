using API_DSCS2_WEBBANGIAY.Areas.admin.Models;
using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;
        private IConfiguration _config;

        public AuthController(ShoesEcommereContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [Authorize(Roles = "1")]
        [HttpGet]
        public async Task<IActionResult> Auth()
        {
            try
            {
                var currentUser = GetCurrentUser();
                //var user = _context.TaiKhoans.Include(x => x.SdtKhNavigation).FirstOrDefault(x => x.TenTaiKhoan == currentUser.TenTaiKhoan);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var currentUser = GetCurrentUser();
                var user = _context.TaiKhoans.Include(x => x.DiaChis).Include(x => x.PhieuNhapXuats).FirstOrDefault(x => x.TenTaiKhoan == currentUser.TenTaiKhoan);
                if (user is null) return Unauthorized();
                return Ok(new
                {
                    user = new
                    {
                        userName = user.TenTaiKhoan,
                        role = user.Role,
                        info = user.DiaChis,
                        hoadons= user.PhieuNhapXuats,
                        addressDefault = user.addressDefault,
                        avatar= user.Avatar,
                        nameDisplay = user.TenHienThi,
                    }

                });;;
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
           
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(LoginModel body)
        {
            var user = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan == body.UserName);
            if(user is not null)
            {
                var token = Generate(user, DateTime.Now.AddSeconds(15));
                var refreshToken = Generate(user, DateTime.Now.AddDays(30));
                return Ok(new
                {
                    token,
                    refreshToken,
                    info=user,
                });
            }
            else
            {
                try
                {
                    _context.KhachHangs.Add(body.info);
                    await _context.SaveChangesAsync();
                    TaiKhoan tk = new TaiKhoan();
                    tk.TenTaiKhoan = body.UserName;
                    _context.TaiKhoans.Add(tk);
                    await _context.SaveChangesAsync();
                    var token = Generate(tk, DateTime.Now.AddSeconds(15));
                    var refreshToken = Generate(tk, DateTime.Now.AddDays(30));
                    return Ok(new
                    {
                        token,
                        refreshToken,
                        info = body.info,
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {

            try
            {
                var currentUser = GetCurrentUser();
                var user = _context.TaiKhoans.Include(x=>x.DiaChis).FirstOrDefault(x => x.TenTaiKhoan == currentUser.TenTaiKhoan);
                return Ok(new
                {
                    user = new
                    {
                        userName = user.TenTaiKhoan,
                        role = user.Role,
                        info = user.DiaChis

                    }

                }); ;
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(LoginModel body)
        {
            var user = await Authenticate(body);
            if (user != null)
            {
                var token = Generate(user,DateTime.Now.AddSeconds(15));
                var refreshToken = Generate(user, DateTime.Now.AddDays(30));
                return Ok(token);
            }
            return NotFound();
        }
        [HttpPost("UserRegister")]
        public async Task<IActionResult> UserRegister(LoginModel taiKhoan)
        {
            try
            {
                 _context.KhachHangs.Add(taiKhoan.info);
                await _context.SaveChangesAsync();
                TaiKhoan tk = new TaiKhoan();
                tk.idKH = taiKhoan.info.Id;
                tk.TenTaiKhoan = taiKhoan.UserName;
                tk.MatKhau = taiKhoan.Password;
                _context.TaiKhoans.Add(tk); 
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        private string Generate(TaiKhoan user,DateTime expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier,user.TenTaiKhoan),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: expires,
              signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<TaiKhoan> Authenticate(LoginModel body)
        {
            try
            {
                var taikhoan = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan.ToLower().Trim() == body.UserName.ToLower().Trim() && x.MatKhau.Trim() == body.Password.Trim());
                if (taikhoan != null)
                {
                    return taikhoan;

                }
                return null;
            }
            catch (Exception err)
            {
                return null;
            }
        }
        private TaiKhoan GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new TaiKhoan
                {
                    TenTaiKhoan = userClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = Int32.Parse(userClaim.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value)
                };
            }
            return null;
        }
    }
}

