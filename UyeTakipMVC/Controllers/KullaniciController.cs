using AuthenticationPlugin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controlles
{
    public class KullaniciController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Bu controller herhangi bir view e bağlı değil. postman üzerinden veritabanına veri ekliyoruz /NewKullanici
        /// Postman ile Kullanıcı ekleneceği zaman [AllowAnonymous] kısmını açın.
        /// Tarih: 27.01.2023
        /// </summary>
        UyeTakipContext c = new UyeTakipContext();
        KullaniciRepository kullaniciR = new KullaniciRepository();
        private string m_kod = "6.0";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public KullaniciController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);

                if (rol.okuma == true)
                    return View(c.tbl_kullanici_rol.Include("Kullanici").Include("Rol").OrderBy(x => x.kullanici_id).ToList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(c.tbl_kullanici_rol.Include("Kullanici").Include("Rol").ToList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewKullanici()
        {
            List<SelectListItem> role = (from y in c.tbl_rol.OrderBy(x => x.rol_id).ToList()
                                         select new SelectListItem
                                         {
                                             Text = y.rol_ad,
                                             Value = y.rol_id.ToString()
                                         }).ToList();
            ViewBag.role = role;
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);

                if (rol.ekleme == true)
                    return View();
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult NewKullanici(MyTuple p)
        {
            try
            {
                var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
                var k_id = HttpContext.Request.Cookies["k_id"];
                var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.ekleme == true)
                    {
                        var userWithSameNickname = c.tbl_kullanici.Where(u => u.kullanici_ad == p.kullanici_ad).SingleOrDefault();
                        if (userWithSameNickname != null)
                        {
                            _toastNotification.AddErrorToastMessage("Aynı kullanıcı adı mevcut!");
                            return View(p);
                        }

                        using (var db = new UyeTakipContext())
                        {
                            var k = new KullaniciClass { kullanici_ad = p.kullanici_ad, sifre = SecurePasswordHasherHelper.Hash(p.sifre) };
                            c.tbl_kullanici.Add(k);
                            c.SaveChanges();

                            var x = c.tbl_kullanici.OrderBy(x => x.kullanici_id).LastOrDefault();
                            var kr = new KullaniciRolClass { kullanici_id = x.kullanici_id, rol_id = p.rol_id };
                            c.tbl_kullanici_rol.Add(kr);
                            c.SaveChanges();
                        }
                        _toastNotification.AddSuccessToastMessage("Kullanıcı ekleme başarılı.");
                        return RedirectToAction("Index", "Kullanici");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Kullanici");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    var userWithSameNickname = c.tbl_kullanici.Where(u => u.kullanici_ad == p.kullanici_ad).SingleOrDefault();
                    if (userWithSameNickname != null)
                    {
                        _toastNotification.AddErrorToastMessage("Aynı kullanıcı adı mevcut!");
                        return View(p);
                    }

                    using (var db = new UyeTakipContext())
                    {
                        var k = new KullaniciClass { kullanici_ad = p.kullanici_ad, sifre = SecurePasswordHasherHelper.Hash(p.sifre) };
                        c.tbl_kullanici.Add(k);
                        c.SaveChanges();

                        var x = c.tbl_kullanici.OrderBy(x => x.kullanici_id).LastOrDefault();
                        var kr = new KullaniciRolClass { kullanici_id = x.kullanici_id, rol_id = p.rol_id };
                        c.tbl_kullanici_rol.Add(kr);
                        c.SaveChanges();
                    }
                    _toastNotification.AddSuccessToastMessage("Kullanıcı ekleme başarılı.");
                    return RedirectToAction("Index", "Kullanici");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Address");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Kullanıcı ekleme başarısız!");
                RedirectToAction("NewKullanici", "Kullanici");
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteKullanici(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = kullaniciR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        kullaniciR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Kullanici");
                    }
                }
                else if (yetki.silme == true)
                {
                    kullaniciR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Kullanici");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu kullanıcıyı silemezsiniz.");
            }
            return RedirectToAction("Index", "Kullanici");
        }
    }
}
