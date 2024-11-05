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
    public class BeldeController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Beldeleri eklediğimiz, sildiğimiz, güncellediğimiz controller.
        /// Tarih: 24.01.2023
        /// </summary>

        BeldeRepository beldeR = new BeldeRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "3.4";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public BeldeController(IToastNotification toastNotification)
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
                    return View(beldeR.TList("Ilce"));
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(beldeR.TList("Ilce"));
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewBelde()
        {
            //  ComboBox için
            List<SelectListItem> values = (from x in c.tbl_ilce.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.ilce_ad,
                                               Value = x.ilce_id.ToString()
                                           }).ToList();
            ViewBag.ilce = values;
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
                    return RedirectToAction("Index", "Belde");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Belde");
            }
        }

        [HttpPost]
        public IActionResult NewBelde(BeldeClass p)
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
                        beldeR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Belde Ekleme Başarılı");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Belde");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    beldeR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Belde Ekleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Belde");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_ilce.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.ilce_ad,
                                                   Value = x.ilce_id.ToString()
                                               }).ToList();
                ViewBag.ilce = values;
                _toastNotification.AddErrorToastMessage("Belde ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Belde");
        }

        [HttpGet]
        public IActionResult GetBelde(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List<SelectListItem> values = (from x in c.tbl_ilce.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.ilce_ad,
                                               Value = x.ilce_id.ToString()
                                           }).ToList();
            ViewBag.ilce = values;

            var y = beldeR.TGet(id);
            BeldeClass bc = new BeldeClass()
            {
                belde_id = y.belde_id,
                ilce_id = y.ilce_id,
                belde_ad = y.belde_ad
            };

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(bc);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Belde");
                }
            }
            else if (yetki.guncelleme == true)
                return View(bc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Belde");
            }
        }

        [HttpPost]
        public IActionResult UpdateBelde(BeldeClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = beldeR.TGet(p.belde_id);
                x.belde_ad = p.belde_ad;
                x.ilce_id = p.ilce_id;

                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        beldeR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Belde");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    beldeR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Belde");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_ilce.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.ilce_ad,
                                                   Value = x.ilce_id.ToString()
                                               }).ToList();
                ViewBag.ilce = values;
                _toastNotification.AddErrorToastMessage("Belde güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Belde");
        }

        public IActionResult DeleteBelde(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = beldeR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        beldeR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Belde");
                    }
                }
                else if (yetki.silme == true)
                {
                    beldeR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Belde");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu beldeyi silemezsiniz!");
            }
            return RedirectToAction("Index", "Belde");
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<BeldeClass> s = c.tbl_belde.Select(p => new BeldeClass
                {
                    belde_id = p.belde_id,
                    belde_ad = p.belde_ad,
                    Ilce = p.Ilce
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "Belde ID";
                ew.Cells["B1"].Value = "Belde Ad";
                ew.Cells["C1"].Value = "İlçesi";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.belde_id;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.belde_ad;
                    ew.Cells[string.Format("C{0}", rowStart)].Value = item.Ilce.ilce_ad;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Beldeler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }
    }
}
