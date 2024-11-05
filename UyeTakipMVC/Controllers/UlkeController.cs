using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using OfficeOpenXml;
using System.Data;
using System.Drawing;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;

namespace UyeTakipMVC.Controlles
{
    public class UlkeController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Ülkeleri eklediğimiz, sildiğimiz, güncellediğimiz controller.
        /// Tarih: 24.01.2023
        /// </summary>

        UlkeRepository ulkeR =  new UlkeRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "3.1";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public UlkeController(IToastNotification toastNotification)
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
                    return View(ulkeR.TList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(ulkeR.TList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewCountry()
        {
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
                    return RedirectToAction("Index", "Ulke");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Ulke");
            }
        }

        [HttpPost]
        public IActionResult NewCountry(UlkeClass p)
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
                        ulkeR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Ülke Ekleme Başarılı");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Ulke");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    ulkeR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Ülke Ekleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Ulke");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Ülke ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Ulke");
        }

        [HttpGet]
        public IActionResult GetCountry(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            var x = ulkeR.TGet(id);
            UlkeClass uc = new UlkeClass()
            {
                ulke_id = x.ulke_id,
                ulke_ad = x.ulke_ad
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(uc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Ulke");
                }
            }
            else if (yetki.guncelleme == true)
                return View(uc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Ulke");
            }
        }

        [HttpPost]
        public IActionResult UpdateCountry(UlkeClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ulkeR.TGet(p.ulke_id);
                x.ulke_ad = p.ulke_ad;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        ulkeR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Ulke");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    ulkeR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Ulke");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Ülke güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Ulke");
        }

        public IActionResult DeleteCountry(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ulkeR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        ulkeR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Ulke");
                    }
                }
                else if (yetki.silme == true)
                {
                    ulkeR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Ulke");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu ülkeyi silemezsiniz.");
            }
            return RedirectToAction("Index", "Ulke");
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<UlkeClass> s = c.tbl_ulke.Select(p => new UlkeClass
                {
                    ulke_id = p.ulke_id,
                    ulke_ad = p.ulke_ad
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "Ülke ID";
                ew.Cells["B1"].Value = "Ülke Ad";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.ulke_id;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.ulke_ad;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Ülkeler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }
    }
}
