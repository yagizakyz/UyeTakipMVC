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
    public class IlceController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: İlçeleri eklediğimiz, sildiğimiz, güncellediğimiz controller.
        /// Tarih: 24.01.2023
        /// </summary>

        IlceRepository ilceR = new IlceRepository();
        UyeTakipContext c = new UyeTakipContext();
        private string m_kod = "3.3";

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public IlceController(IToastNotification toastNotification)
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
                    return View(ilceR.TList("Sehir"));
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(ilceR.TList("Sehir"));
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewIlce()
        {
            //  ComboBox için
            List<SelectListItem> values = (from x in c.tbl_sehir.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.sehir_ad,
                                               Value = x.sehir_id.ToString()
                                           }).ToList();
            ViewBag.sehir = values;
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
                    return RedirectToAction("Index", "Ilce");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Ilce");
            }
        }

        [HttpPost]
        public IActionResult NewIlce(IlceClass p)
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
                        ilceR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("İlçe Ekleme Başarılı");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Ilce");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    ilceR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("İlçe Ekleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Ilce");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_sehir.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.sehir_ad,
                                                   Value = x.sehir_id.ToString()
                                               }).ToList();
                ViewBag.sehir = values;
                _toastNotification.AddErrorToastMessage("İlçe ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Ilce");
        }

        [HttpGet]
        public IActionResult GetIlce(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            List<SelectListItem> values = (from x in c.tbl_sehir.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.sehir_ad,
                                               Value = x.sehir_id.ToString()
                                           }).ToList();
            ViewBag.sehir = values;

            var y = ilceR.TGet(id);
            IlceClass bc = new IlceClass()
            {
                ilce_id = y.ilce_id,
                ilce_ad = y.ilce_ad,
                sehir_id = y.sehir_id
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
                    return RedirectToAction("Index", "Ilce");
                }
            }
            else if (yetki.guncelleme == true)
                return View(bc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Ilce");
            }
        }

        [HttpPost]
        public IActionResult UpdateIlce(IlceClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ilceR.TGet(p.ilce_id);
                x.ilce_ad = p.ilce_ad;
                x.sehir_id = p.sehir_id;
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        ilceR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Ilce");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    ilceR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Güncelleme Başarılı");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Ilce");
                }
            }
            catch (Exception)
            {
                List<SelectListItem> values = (from x in c.tbl_sehir.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.sehir_ad,
                                                   Value = x.sehir_id.ToString()
                                               }).ToList();
                ViewBag.sehir = values;
                _toastNotification.AddErrorToastMessage("İlçe güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Ilce");
        }

        public IActionResult DeleteIlce(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = ilceR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        ilceR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Ilce");
                    }
                }
                else if (yetki.silme == true)
                {
                    ilceR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Ilce");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu ilçeyi silemezsiniz.");
            }
            
            return RedirectToAction("Index", "Ilce");
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<IlceClass> s = c.tbl_ilce.Select(p => new IlceClass
                {
                    ilce_id = p.ilce_id,
                    ilce_ad = p.ilce_ad,
                    Sehir = p.Sehir
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "İlçe ID";
                ew.Cells["B1"].Value = "İlçe Ad";
                ew.Cells["c1"].Value = "Şehri";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.ilce_id;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.ilce_ad;
                    ew.Cells[string.Format("C{0}", rowStart)].Value = item.Sehir.sehir_ad;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "İlçeler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }
    }
}
