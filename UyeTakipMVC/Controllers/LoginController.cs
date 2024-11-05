using AuthenticationPlugin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using OfficeOpenXml.ConditionalFormatting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UyeTakipMVC.Models;

namespace UyeTakipMVC.Controlles
{
    public class LoginController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Kullanıcı login yaptıpı controller.
        /// Tarih: 27.01.2023
        /// </summary>

        private UyeTakipContext _c = new UyeTakipContext();
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginUser(KullaniciClass p)
        {
            var server = HttpContext.Request.Cookies["server"];
            var userId = HttpContext.Request.Cookies["userId"];
            var password = HttpContext.Request.Cookies["password"];
            var dbName = HttpContext.Request.Cookies["dbName"];

            //  server bilgilerini connectionStringe atayıp bağlanıyoruz.
            UyeTakipContext uye = new UyeTakipContext();
            uye.ServerInf(server, userId, password, dbName);

            var kullaniciAd = _c.tbl_kullanici.FirstOrDefault(u => u.kullanici_ad == p.kullanici_ad);
            if (kullaniciAd == null || !SecurePasswordHasherHelper.Verify(p.sifre, kullaniciAd.sifre) || server == null || userId == null || password == null || dbName == null)
            {
                ViewBag.wrong = "Kullanıcı adı veya şifre hatalı! \nServer bilgilerinide girmeyi unutmayın!";
                return View("Index");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, p.kullanici_ad),
                new Claim(ClaimTypes.Email, p.kullanici_ad)
            };

            var userIdentity = new ClaimsIdentity(claims, "Login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            HttpContext.SignInAsync(principal);

            HttpContext.Response.Cookies.Append("k_id", Convert.ToString(kullaniciAd.kullanici_id));
            HttpContext.Session.SetString("kullaniciAd", kullaniciAd.kullanici_ad);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ServerInfo()
        {
            var x = new ServerInformation()
            {
                server = HttpContext.Request.Cookies["server"],
                userId = HttpContext.Request.Cookies["userId"],
                password = HttpContext.Request.Cookies["password"],
                dbName = HttpContext.Request.Cookies["dbName"]
            };
            return View(x);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ServerInfo(ServerInformation p)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(730);
            HttpContext.Response.Cookies.Append("server", p.server, cookieOptions);
            HttpContext.Response.Cookies.Append("userId", p.userId, cookieOptions);
            HttpContext.Response.Cookies.Append("password", p.password, cookieOptions);
            HttpContext.Response.Cookies.Append("dbName", p.dbName, cookieOptions);
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("k_id");
            return RedirectToAction("Index", "Login");
        }
    }
}
