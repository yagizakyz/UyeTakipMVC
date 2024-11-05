using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controllers
{
    public class KullaniciRolController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Kullanıcılara rol tanımladığımız controller.
        /// Tarih: 15.02.2023
        /// </summary>
        
        KullaniciRolRepository krR = new KullaniciRolRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "8.0";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public KullaniciRolController(IToastNotification toastNotification)
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
                    return View(c.tbl_kullanici_rol.Include("Kullanici").Include("Rol").ToList());
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
        public IActionResult NewKr()
        {
            List();
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            KullaniciRolClass krc = new KullaniciRolClass()
            {
                kullanici_id = Convert.ToInt32(k_id)
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.ekleme == true)
                    return View(krc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "KullaniciRol");
                }
            }
            else if (yetki.ekleme == true)
                return View(krc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "KullaniciRol");
            }
        }

        [HttpPost]
        public IActionResult NewKr(KullaniciRolClass p)
        {
            try
            {
                List();
                var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
                var k_id = HttpContext.Request.Cookies["k_id"];
                var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.ekleme == true)
                    {
                        krR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Rol Tanımlama Başarılı");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Rol tanımla yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "KullaniciRol");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    krR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Rol Tanımlama Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Rol tanımla yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "KullaniciRol");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Veri ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "KullaniciRol");
        }

        [HttpGet]
        public IActionResult GetKr(int id)
        {
            List();
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            var x = krR.TGet(id);
            KullaniciRolClass krc = new KullaniciRolClass()
            {
                kr_id = x.kr_id,
                rol_id = x.rol_id,
                kullanici_id = x.kullanici_id
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(krc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "KullaniciRol");
                }
            }
            else if (yetki.guncelleme == true)
                return View(krc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "KullaniciRol");
            }
        }

        [HttpPost]
        public IActionResult UpdateKr(KullaniciRolClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = krR.TGet(p.kr_id);
                x.kullanici_id = p.kullanici_id;
                x.rol_id = p.rol_id;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        krR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "KullaniciRol");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    krR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "KullaniciRol");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Veri güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "KullaniciRol");
        }

        public IActionResult DeleteKr(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = krR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        krR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "KullaniciRol");
                    }
                }
                else if (yetki.silme == true)
                {
                    krR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "KullaniciRol");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu veri silinemez.");
            }
            return RedirectToAction("Index", "KullaniciRol");
        }

        public void List()
        {
            List<SelectListItem> kullanici = (from y in c.tbl_kullanici.OrderBy(x => x.kullanici_id).ToList()
                                              select new SelectListItem
                                              {
                                                  Text = y.kullanici_ad,
                                                  Value = y.kullanici_id.ToString()
                                              }).ToList();
            ViewBag.kullanici = kullanici;

            List<SelectListItem> rol = (from y in c.tbl_rol.OrderBy(x => x.rol_id).ToList()
                                        select new SelectListItem
                                        {
                                            Text = y.rol_ad,
                                            Value = y.rol_id.ToString()
                                        }).ToList();
            ViewBag.role = rol;
        }
    }
}
