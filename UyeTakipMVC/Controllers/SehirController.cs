using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using OfficeOpenXml;
using System.Data;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controlles
{
    public class SehirController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Şehirleri eklediğimiz, sildiğimiz, güncellediğimiz controller.
        /// Tarih: 24.01.2023
        /// </summary>

        SehirRepository sehirR = new SehirRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "3.2";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public SehirController(IToastNotification toastNotification)
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
                    return View(sehirR.TList("Ulke"));
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(sehirR.TList("Ulke"));
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewCity()
        {
            //  ComboBox için
            List<SelectListItem> values = (from x in c.tbl_ulke.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.ulke_ad,
                                               Value = x.ulke_id.ToString()
                                           }).ToList();
            ViewBag.ulke = values;

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
                    return RedirectToAction("Index", "Sehir");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Sehir");
            }
        }

        [HttpPost]
        public IActionResult NewCity(SehirClass p)
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
                        sehirR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Şehir Ekleme Başarılı");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Sehir");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    sehirR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Şehir Ekleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Sehir");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_ulke.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.ulke_ad,
                                                   Value = x.ulke_id.ToString()
                                               }).ToList();
                ViewBag.ulke = values;
                _toastNotification.AddErrorToastMessage("Şehir ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Sehir");
        }

        [HttpGet]
        public IActionResult GetCity(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List<SelectListItem> values = (from x in c.tbl_ulke.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.ulke_ad,
                                               Value = x.ulke_id.ToString()
                                           }).ToList();
            ViewBag.ulke = values;

            var y = sehirR.TGet(id);
            SehirClass sc = new SehirClass()
            {
                sehir_id = y.sehir_id,
                sehir_ad = y.sehir_ad,
                plaka_kodu = y.plaka_kodu,
                ulke_id = y.ulke_id
            };
            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(sc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Sehir");
                }
            }
            else if (yetki.guncelleme == true)
                return View(sc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Sehir");
            }
        }

        [HttpPost]
        public IActionResult UpdateCity(SehirClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = sehirR.TGet(p.sehir_id);
                x.sehir_ad = p.sehir_ad;
                x.plaka_kodu = p.plaka_kodu;
                x.ulke_id = p.ulke_id;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        sehirR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme başarılı.");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Sehir");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    sehirR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Sehir");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_ulke.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.ulke_ad,
                                                   Value = x.ulke_id.ToString()
                                               }).ToList();
                ViewBag.ulke = values;
                _toastNotification.AddErrorToastMessage("Şehir güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Sehir");
        }

        public IActionResult DeleteCity(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = sehirR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        sehirR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Sehir");
                    }
                }
                else if (yetki.silme == true)
                {
                    sehirR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Sehir");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu şehri silemezsiniz.");
            }
            return RedirectToAction("Index", "Sehir");
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<SehirClass> s = c.tbl_sehir.Select(p => new SehirClass
                {
                    sehir_id = p.sehir_id,
                    sehir_ad = p.sehir_ad,
                    Ulke = p.Ulke
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "Şehir ID";
                ew.Cells["B1"].Value = "Şehir Ad";
                ew.Cells["C1"].Value = "Ülkesi";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.sehir_id;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.sehir_ad;
                    ew.Cells[string.Format("C{0}", rowStart)].Value = item.Ulke.ulke_ad;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Şehirler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }
    }
}
