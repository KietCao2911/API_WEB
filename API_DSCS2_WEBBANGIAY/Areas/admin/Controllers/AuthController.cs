﻿using API_DSCS2_WEBBANGIAY.Areas.admin.Models;
using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils.Mail;
using API_DSCS2_WEBBANGIAY.Utils.Mail.TemplateHandle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        private readonly IHostingEnvironment _HostEnvironment;
        private IConfiguration _config;
        private readonly IOptions<MailSettings> mailSettings;

        public AuthController(ShoesEcommereContext context, IHostingEnvironment hostEnvironment, IConfiguration config, IOptions<MailSettings> mailSettings)
        {
            _context = context;
            _HostEnvironment = hostEnvironment;
            _config = config;
            this.mailSettings = mailSettings;
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
                var user = _context.TaiKhoans/*.Include(r=>r.RoleDetails)*//*.Include(x => x.DiaChis)*/.FirstOrDefault(x => x.TenTaiKhoan == currentUser.TenTaiKhoan);
                if (user is null) return Unauthorized();
                return Ok(new
                {
                    user = new
                    {
                        userName = user.TenTaiKhoan,
                        role = user.Role,
                        info = user.DiaChis,
                        hoadons = user.PhieuNhapXuats,
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
        [HttpPost("EmailVerify")]
        public async Task<IActionResult> EmailVerify(String token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                if (jwtSecurityToken is not null)
                {
                    var userClaim = jwtSecurityToken.Claims;
                    var TenTaiKhoan = userClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (TenTaiKhoan is null) return Unauthorized();
                    var user = _context.TaiKhoans.FirstOrDefault(x=>x.TenTaiKhoan == TenTaiKhoan);
                    user.isActive = true;
                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception err)
            {
                return BadRequest();
            }
        }
        [HttpPost("EmailRegister")]
        public async Task<IActionResult> EmailRegister(TaiKhoan body)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan == body.TenTaiKhoan);
                if (user is  null)
                {
                    body.Role = 0;
                    _context.TaiKhoans.Add(body);
                    _context.SaveChanges();
                    var token = Generate(body, DateTime.Now.AddDays(15));
                    var link = "http://localhost:3000/verify_email/" + token;
                    var bodyString = await new EmailVerify(_HostEnvironment).RenderBody(link);
                    var mailBody = new MailRequest()
                    {
                        Body = bodyString.ToString(),
                        Subject = "XÁC NHẬN EMAIL",
                        ToEmail = body.TenTaiKhoan,
                    };
                    var mailSend = new MailService(mailSettings);
                    mailSend.SendEmailAsync(mailBody);
                    await trans.CommitAsync();
                    return Ok();
                }
                else
                {
                     trans.RollbackAsync();
                    return BadRequest("Email đã có người dùng.");
                }
            }
            catch (Exception err)
            {
                trans.RollbackAsync();
                return NotFound();
            }
        }
        [HttpPost("EmailLogin")]
        public async Task<IActionResult> EmailLogin(TaiKhoan body)
        {
            try
            {
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan == body.TenTaiKhoan&&x.isActive==true);
                if (user is not null)
                {
                    var token = Generate(user, DateTime.Now.AddSeconds(15));
                    var refreshToken = Generate(user, DateTime.Now.AddDays(30));
                    return Ok(new
                    {
                        token,
                        refreshToken,
                        info = user,
                    });
                }
                else
                {
                    return BadRequest("Sai tài khoản hoặc mật khẩu.");
                }
            }
            catch(Exception err)
            {
                return NotFound();
            }
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(LoginModel body)
        {
            var user = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.TenTaiKhoan == body.UserName&&x.isActive==true);
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
                var user = _context.TaiKhoans/*.Include(x=>x.DiaChis)*/.FirstOrDefault(x => x.TenTaiKhoan == currentUser.TenTaiKhoan);
                return Ok(new
                {
                    user = new
                    {
                        userName = user.TenTaiKhoan,
                        role = user.Role,
                        //info = user.DiaChis

                    }

                }); ;
            }
            catch (Exception ex)
            {
                return Unauthorized();
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
                new Claim(ClaimTypes.Role, user.Role.ToString()),

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

