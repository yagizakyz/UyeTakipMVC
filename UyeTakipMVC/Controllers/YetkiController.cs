using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Reflection.Metadata;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controlles
{
    public class YetkiController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Kullanıcılara ve Rollere yetki tanımladığımız sayfa.
        /// Tarih: 06.02.2023
        /// </summary>

        private string m_kod = "5.0";
        UyeTakipContext c = new UyeTakipContext();
        KullaniciYetkiRepository kyR = new KullaniciYetkiRepository();

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public YetkiController(IToastNotification toastNotification)
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
                    return View(c.tbl_kullanici_menu_yetki.Include("Menu").Include("Kullanici").ToList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(c.tbl_kullanici_menu_yetki.Include("Menu").Include("Kullanici").ToList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewYetki()
        {
            List();

            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            KullaniciMenuYetkiClass kc = new KullaniciMenuYetkiClass()
            {
                kullanici_id = Convert.ToInt32(k_id),
                menu_id = menuCheck.menu_id
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.ekleme == true)
                    return View(kc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            else if (yetki.ekleme == true)
                return View(kc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Yetki");
            }
        }

        [HttpPost]
        public IActionResult NewYetki(KullaniciMenuYetkiClass p)
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
                        kyR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Yetki ekleme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Yetki");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    kyR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Yetki ekleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Yetki ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetYetki(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List();

            var x = kyR.TGet(id);
            KullaniciMenuYetkiClass pc = new KullaniciMenuYetkiClass()
            {
                kmy_id = x.kmy_id,
                kullanici_id = x.kullanici_id,
                menu_id = x.menu_id,
                ekleme = x.ekleme,
                okuma = x.okuma,
                guncelleme = x.guncelleme,
                silme = x.silme
            };
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(pc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            else if (yetki.guncelleme == true)
                return View(pc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Yetki");
            }
        }

        [HttpPost]
        public IActionResult UpdateYetki(KullaniciMenuYetkiClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = kyR.TGet(p.kmy_id);
                x.kullanici_id = p.kullanici_id;
                x.menu_id = p.menu_id;
                x.ekleme = p.ekleme;
                x.okuma = p.okuma;
                x.guncelleme = p.guncelleme;
                x.silme = p.silme;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        kyR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Yetki");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    kyR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Güncelleme işlemi başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Yetki");
        }

        public IActionResult DeleteYetki(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = kyR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        kyR.TDelete(x);
                        _toastNotification.AddSuccessToastMessage("Yetki silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Yetki");
                    }
                }
                else if (yetki.silme == true)
                {
                    kyR.TDelete(x);
                    _toastNotification.AddSuccessToastMessage("Yetki silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Silme işlemi başarısız!");
            }
            return RedirectToAction("Index", "Yetki");
        }

        //  Bundan sonrası roller için yetkilendirme
        RolYetkiRepository ryR = new RolYetkiRepository();

        [HttpGet]
        public IActionResult RoleYetki()
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);

                if (rol.okuma == true)
                    return View(c.tbl_rol_menu_yetki.Include("Menu").Include("Rol").ToList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(c.tbl_rol_menu_yetki.Include("Menu").Include("Rol").ToList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewRoleYetki()
        {
            List();

            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            var rolList = c.tbl_rol.FirstOrDefault();

            RolMenuYetkiClass rc = new RolMenuYetkiClass()
            {
                menu_id = menuCheck.menu_id
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.ekleme == true)
                    return View(rc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("RoleYetki", "Yetki");
                }
            }
            else if (yetki.ekleme == true)
                return View(rc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("RoleYetki", "Yetki");
            }
        }

        [HttpPost]
        public IActionResult NewRoleYetki(RolMenuYetkiClass p)
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
                        ryR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Yetki ekleme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("RoleYetki", "Yetki");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    ryR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Yetki ekleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("RoleYetki", "Yetki");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Yetki ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("RoleYetki");
        }

        [HttpGet]
        public IActionResult GetRoleYetki(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List();

            var x = ryR.TGet(id);
            RolMenuYetkiClass pc = new RolMenuYetkiClass()
            {
                rmy_id = x.rmy_id,
                rol_id = x.rol_id,
                menu_id = x.menu_id,
                ekleme = x.ekleme,
                okuma = x.okuma,
                guncelleme = x.guncelleme,
                silme = x.silme
            };
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(pc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Yetki");
                }
            }
            else if (yetki.guncelleme == true)
                return View(pc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Yetki");
            }
        }

        [HttpPost]
        public IActionResult UpdateRoleYetki(RolMenuYetkiClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ryR.TGet(p.rmy_id);
                x.rol_id = p.rol_id;
                x.menu_id = p.menu_id;
                x.ekleme = p.ekleme;
                x.okuma = p.okuma;
                x.guncelleme = p.guncelleme;
                x.silme = p.silme;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        ryR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("RoleYetki", "Yetki");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    ryR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("RoleYetki", "Yetki");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Güncelleme işlemi başarısız!");
                return View(p);
            }
            return RedirectToAction("RoleYetki", "Yetki");
        }

        public IActionResult DeleteRoleYetki(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ryR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        ryR.TDelete(x);
                        _toastNotification.AddSuccessToastMessage("Yetki silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("RoleYetki", "Yetki");
                    }
                }
                else if (yetki.silme == true)
                {
                    ryR.TDelete(x);
                    _toastNotification.AddSuccessToastMessage("Yetki silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("RoleYetki", "Yetki");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Silme işlemi başarısız!");
            }
            return RedirectToAction("RoleYetki", "Yetki");
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

            List<SelectListItem> menu = (from y in c.tbl_menu.OrderBy(x => x.menu_id).ToList()
                                         select new SelectListItem
                                         {
                                             Text = y.menu_ad,
                                             Value = y.menu_id.ToString()
                                         }).ToList();
            ViewBag.menu = menu;

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
