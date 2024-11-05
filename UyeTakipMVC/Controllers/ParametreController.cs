using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;
using System.Security.Claims;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controlles
{
    public class ParametreController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Parametreleri eklediğimiz, sildiğimiz, güncellediğimiz controller. Bu parametreler mail gönderme sayfasında karşımıza çıkacaktır.
        /// Tarih: 27.01.2023
        /// </summary>

        ParametreRepository parametreR = new ParametreRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "4.0";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public ParametreController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var k_id = HttpContext.Request.Cookies["k_id"];
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);

                if (rol.okuma == true)
                    return View(c.tbl_parametre.Where(x => x.kullanici_id == Convert.ToInt32(k_id)).Include("ParametreId").Include("Kullanici").ToList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(c.tbl_parametre.Where(x => x.kullanici_id == Convert.ToInt32(k_id)).Include("ParametreId").Include("Kullanici").ToList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewParametre()
        {
            List<SelectListItem> values = (from y in c.tbl_parametre_ad.ToList()
                                           select new SelectListItem
                                           {
                                               Text = y.parametre_ad,
                                               Value = y.parametre_ad_id.ToString()
                                           }).ToList();
            ViewBag.pa = values;

            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            ParametreClass pc = new ParametreClass()
            {
                kullanici_id = Convert.ToInt32(k_id)
            };
            
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.ekleme == true)
                    return View(pc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Parametre");
                }
            }
            else if (yetki.ekleme == true)
                return View(pc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Parametre");
            }
        }

        [HttpPost]
        public IActionResult NewParametre(ParametreClass p)
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
                        parametreR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Parametre ekleme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Parametre");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    parametreR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Parametre ekleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Parametre");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from y in c.tbl_parametre_ad.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = y.parametre_ad,
                                                   Value = y.parametre_ad_id.ToString()
                                               }).ToList();
                ViewBag.pa = values;
                _toastNotification.AddErrorToastMessage("Parametre ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetParametre(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List<SelectListItem> values = (from y in c.tbl_parametre_ad.ToList()
                                           select new SelectListItem
                                           {
                                               Text = y.parametre_ad,
                                               Value = y.parametre_ad_id.ToString()
                                           }).ToList();
            ViewBag.pa = values;

            var x = parametreR.TGet(id);
            ParametreClass pc = new ParametreClass()
            {
                parametre_id = x.parametre_id,
                kullanici_id = x.kullanici_id,
                parametre_ad_id = x.parametre_ad_id,
                parametre_icerigi = x.parametre_icerigi
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
                    return RedirectToAction("Index", "Parametre");
                }
            }
            else if (yetki.guncelleme == true)
                return View(pc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Parametre");
            }
        }

        [HttpPost]
        public IActionResult UpdateParametre(ParametreClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = parametreR.TGet(p.parametre_id);
                x.parametre_ad_id = p.parametre_ad_id;
                x.parametre_icerigi = p.parametre_icerigi;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        parametreR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Parametre");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    parametreR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Parametre");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from y in c.tbl_parametre_ad.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = y.parametre_ad,
                                                   Value = y.parametre_ad_id.ToString()
                                               }).ToList();
                ViewBag.pa = values;
                _toastNotification.AddErrorToastMessage("Güncelleme işlemi başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Parametre");
        }

        public IActionResult DeleteParametre(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = parametreR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        parametreR.TDelete(x);
                        _toastNotification.AddSuccessToastMessage("Parametre silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Parametre");
                    }
                }
                else if (yetki.silme == true)
                {
                    parametreR.TDelete(x);
                    _toastNotification.AddSuccessToastMessage("Parametre silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Parametre");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Silme işlemi başarısız!");
            }
            return RedirectToAction("Index", "Parametre");
        }
    }
}
