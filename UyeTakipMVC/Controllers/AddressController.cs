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
    public class AddressController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Adresleri eklediğimiz, sildiğimiz, güncellediğimiz controller.
        /// Tarih: 25.01.2023
        /// </summary>

        private string m_kod = "2.0";
        UyeTakipContext c = new UyeTakipContext();
        AdresRepository addressR = new AdresRepository();

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public AddressController(IToastNotification toastNotification)
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
                {
                    var address = c.tbl_adres.Include("Uye").Include("Ulke").Include("Sehir").Include("Ilce").Include("Belde").ToList();
                    var x = new UyeClass
                    {
                        AdresC = address
                    };
                    return View(x);
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
            {
                var address = c.tbl_adres.Include("Uye").Include("Ulke").Include("Sehir").Include("Ilce").Include("Belde").ToList();
                var x = new UyeClass
                {
                    AdresC = address
                };
                return View(x);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult LoadCountry()
        {
            //var countries = ulkeR.TList();
            List<SelectListItem> ulke = (from x in c.tbl_ulke.ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ulke_ad,
                                             Value = x.ulke_id.ToString()
                                         }).ToList();
            ViewBag.ulke = ulke;
            return Json(ulke, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }

        // View kısmında olan jQuery/ajax kodu bunlara bağlı. ülke>şehir>ilçe>belde bu sırayla dropdownlistler dolduruluyor.
        public JsonResult GetCities(int countryId)
        {
            var cities = c.tbl_sehir.Where(x => x.ulke_id == countryId).ToList();
            return Json(cities);
        }

        public JsonResult GetIlce(int cityId)
        {
            var ilces = c.tbl_ilce.Where(x => x.sehir_id == cityId).ToList();
            return Json(ilces);
        }

        public JsonResult GetBelde(int ilceId)
        {
            var beldes = c.tbl_belde.Where(x => x.ilce_id == ilceId).ToList();
            return Json(beldes);
        }

        AdresClass ac = new AdresClass();
        [HttpGet]
        public IActionResult NewAddress(string id)
        {
            //  ComboBox için
            if (id == null)
            {
                List<SelectListItem> uye = (from x in c.tbl_uye.ToList()
                                            select new SelectListItem
                                            {
                                                Text = x.tckn + " " + x.ad + " " + x.soyad,
                                                Value = x.tckn
                                            }).ToList();
                ViewBag.uye = uye;
            }
            else
            {
                List<SelectListItem> uye = (from x in c.tbl_uye.Where(x => x.tckn == id).ToList()
                                            select new SelectListItem
                                            {
                                                Text = x.tckn,
                                                Value = x.tckn
                                            }).ToList();
                ViewBag.uye = uye;
            }

            List<SelectListItem> ulke = (from x in c.tbl_ulke.ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ulke_ad,
                                             Value = x.ulke_id.ToString()
                                         }).ToList();
            ViewBag.ulke = ulke;

            List<SelectListItem> adres_turu = new List<SelectListItem>();
            adres_turu.Add(new SelectListItem { Text = "Ev Adresi", Value = "Ev Adresi" });
            adres_turu.Add(new SelectListItem { Text = "İş Adresi", Value = "İş Adresi" });
            adres_turu.Add(new SelectListItem { Text = "KPS Adresi", Value = "KPS Adresi" });
            ViewBag.at = adres_turu;

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
                    return RedirectToAction("Index", "Address");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Address");
            }
        }

        [HttpPost]
        public IActionResult NewAddress(AdresClass p)
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
                        addressR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Adres ekleme başarılı.");
                        return RedirectToAction("Index", "Address");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Address");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    addressR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Adres ekleme başarılı.");
                    return RedirectToAction("Index", "Address");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Address");
                }
            }
            catch (Exception)
            {
                var addressCheck = c.tbl_adres.FirstOrDefault(x => p.uye_tckn == p.uye_tckn);
                var middleAddressCheck = c.tbl_adres.OrderBy(x => x.adres_id).Skip(addressCheck.adres_id).Take(3).FirstOrDefault(x => p.uye_tckn == p.uye_tckn);
                var lastAddressCheck = c.tbl_adres.OrderBy(x => x.adres_id).LastOrDefault(x => p.uye_tckn == p.uye_tckn);
                if (p.adres_tur == addressCheck.adres_tur || p.adres_tur == middleAddressCheck.adres_tur || p.adres_tur == lastAddressCheck.adres_tur)
                {
                    List();
                    _toastNotification.AddErrorToastMessage("Aynı adres türüne kayıtlı bir adres var!");
                    return View(p);
                }
                else
                {
                    List();
                    _toastNotification.AddErrorToastMessage("Adres ekleme başarısız!");
                    return View(p);
                }
            }
        }

        [HttpGet]
        public IActionResult GetAddress(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            var p = addressR.TGet(id);
            AdresClass ac = new AdresClass()
            {
                adres_id = p.adres_id,
                uye_tckn = p.uye_tckn,
                adres_tur = p.adres_tur,
                ulke_id = p.ulke_id,
                sehir_id = p.sehir_id,
                ilce_id = p.ilce_id,
                belde_id = p.belde_id,
                mahalle = p.mahalle,
                bulvar = p.bulvar,
                cadde = p.cadde,
                sokak = p.sokak,
                apartman = p.apartman,
                dis_kapi_no = p.dis_kapi_no,
                ic_kapi_no = p.ic_kapi_no,
                posta_kodu = p.posta_kodu
            };

            //  ComboBox için
            List<SelectListItem> uye = (from x in c.tbl_uye.ToList()
                                        select new SelectListItem
                                        {
                                            Text = x.tckn + " " + x.ad + " " + x.soyad,
                                            Value = x.tckn
                                        }).ToList();
            ViewBag.uye = uye;

            List<SelectListItem> ulke = (from x in c.tbl_ulke.ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ulke_ad,
                                             Value = x.ulke_id.ToString()
                                         }).ToList();
            ViewBag.ulke = ulke;

            List<SelectListItem> sehir = (from x in c.tbl_sehir.Where(x => x.ulke_id == ac.ulke_id).ToList()
                                          select new SelectListItem
                                          {
                                              Text = x.sehir_ad,
                                              Value = x.sehir_id.ToString()
                                          }).ToList();
            ViewBag.sehir = sehir;

            List<SelectListItem> ilce = (from x in c.tbl_ilce.Where(x => x.sehir_id == ac.sehir_id).ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ilce_ad,
                                             Value = x.ilce_id.ToString()
                                         }).ToList();
            ViewBag.ilce = ilce;

            List<SelectListItem> belde = (from x in c.tbl_belde.Where(x => x.ilce_id == ac.ilce_id).ToList()
                                          select new SelectListItem
                                          {
                                              Text = x.belde_ad,
                                              Value = x.belde_id.ToString()
                                          }).ToList();
            ViewBag.belde = belde;

            List<SelectListItem> adres_turu = new List<SelectListItem>();
            adres_turu.Add(new SelectListItem { Text = "Ev Adresi", Value = "Ev Adresi" });
            adres_turu.Add(new SelectListItem { Text = "İş Adresi", Value = "İş Adresi" });
            adres_turu.Add(new SelectListItem { Text = "KPS Adresi", Value = "KPS Adresi" });
            ViewBag.at = adres_turu;

            if (yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                if (rol.guncelleme == true)
                    return View(ac);
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Address");
                }
            }
            else if (yetki.guncelleme == true)
                return View(ac);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Address");
            }
        }

        [HttpPost]
        public IActionResult UpdateAddress(AdresClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            try
            {
                var x = addressR.TGet(p.adres_id);
                x.uye_tckn = p.uye_tckn;
                x.adres_tur = p.adres_tur;
                x.ulke_id = p.ulke_id;
                x.sehir_id = p.sehir_id;
                x.ilce_id = p.ilce_id;
                x.belde_id = p.belde_id;
                x.mahalle = p.mahalle;
                x.bulvar = p.bulvar;
                x.cadde = p.cadde;
                x.sokak = p.sokak;
                x.apartman = p.apartman;
                x.dis_kapi_no = p.dis_kapi_no;
                x.ic_kapi_no = p.ic_kapi_no;
                x.posta_kodu = p.posta_kodu;

                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        addressR.TUpdate(x);
                        _toastNotification.AddSuccessToastMessage("Adres güncelleme başarılı.");
                    }

                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Address");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    addressR.TUpdate(x);
                    _toastNotification.AddSuccessToastMessage("Adres güncelleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Address");
                }
            }
            catch (Exception)
            {
                List();
                _toastNotification.AddErrorToastMessage("Adres güncelleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Address");
        }

        public IActionResult DeleteAddress(int id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            try
            {
                var x = addressR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        addressR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Address");
                    }
                }
                else if (yetki.silme == true)
                {
                    addressR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Address");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Adres silme başarısız!");
            }
            return RedirectToAction("Index", "Address");
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<AdresClass> s = c.tbl_adres.Select(p => new AdresClass
                {
                    uye_tckn = p.uye_tckn,
                    Uye = p.Uye,
                    adres_tur = p.adres_tur,
                    Ulke = p.Ulke,
                    Sehir = p.Sehir,
                    Ilce = p.Ilce,
                    Belde = p.Belde,
                    mahalle = p.mahalle,
                    bulvar = p.bulvar,
                    cadde = p.cadde,
                    sokak = p.sokak,
                    apartman = p.apartman,
                    dis_kapi_no = p.dis_kapi_no,
                    ic_kapi_no = p.ic_kapi_no,
                    posta_kodu = p.posta_kodu
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "Üye TCKN";
                ew.Cells["B1"].Value = "Üye Ad-Soyad";
                ew.Cells["C1"].Value = "Adres Türü";
                ew.Cells["D1"].Value = "Ülke";
                ew.Cells["E1"].Value = "Şehir";
                ew.Cells["F1"].Value = "İlçe";
                ew.Cells["G1"].Value = "Belde";
                ew.Cells["H1"].Value = "Mahalle";
                ew.Cells["I1"].Value = "Bulvar";
                ew.Cells["J1"].Value = "Cadde";
                ew.Cells["K1"].Value = "Sokak";
                ew.Cells["L1"].Value = "Apartman";
                ew.Cells["M1"].Value = "Dış Kapı No";
                ew.Cells["N1"].Value = "İç Kapı No";
                ew.Cells["O1"].Value = "Posta Kodu";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.uye_tckn;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.Uye.ad + " " + item.Uye.soyad;
                    ew.Cells[string.Format("C{0}", rowStart)].Value = item.adres_tur;
                    ew.Cells[string.Format("D{0}", rowStart)].Value = item.Ulke.ulke_ad;
                    ew.Cells[string.Format("E{0}", rowStart)].Value = item.Sehir.sehir_ad;
                    ew.Cells[string.Format("F{0}", rowStart)].Value = item.Ilce.ilce_ad;
                    ew.Cells[string.Format("G{0}", rowStart)].Value = item.Belde.belde_ad;
                    ew.Cells[string.Format("H{0}", rowStart)].Value = item.mahalle;
                    ew.Cells[string.Format("I{0}", rowStart)].Value = item.bulvar;
                    ew.Cells[string.Format("J{0}", rowStart)].Value = item.cadde;
                    ew.Cells[string.Format("K{0}", rowStart)].Value = item.sokak;
                    ew.Cells[string.Format("L{0}", rowStart)].Value = item.apartman;
                    ew.Cells[string.Format("M{0}", rowStart)].Value = item.dis_kapi_no;
                    ew.Cells[string.Format("N{0}", rowStart)].Value = item.ic_kapi_no;
                    ew.Cells[string.Format("O{0}", rowStart)].Value = item.posta_kodu;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Adresler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }

        public void List()
        {
            //  ComboBox için
            List<SelectListItem> uye = (from x in c.tbl_uye.ToList()
                                        select new SelectListItem
                                        {
                                            Text = x.tckn + " " + x.ad + " " + x.soyad,
                                            Value = x.tckn
                                        }).ToList();
            ViewBag.uye = uye;

            List<SelectListItem> ulke = (from x in c.tbl_ulke.ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ulke_ad,
                                             Value = x.ulke_id.ToString()
                                         }).ToList();
            ViewBag.ulke = ulke;

            List<SelectListItem> sehir = (from x in c.tbl_sehir.Where(x => x.ulke_id == ac.ulke_id).ToList()
                                          select new SelectListItem
                                          {
                                              Text = x.sehir_ad,
                                              Value = x.sehir_id.ToString()
                                          }).ToList();
            ViewBag.sehir = sehir;

            List<SelectListItem> ilce = (from x in c.tbl_ilce.Where(x => x.sehir_id == ac.sehir_id).ToList()
                                         select new SelectListItem
                                         {
                                             Text = x.ilce_ad,
                                             Value = x.ilce_id.ToString()
                                         }).ToList();
            ViewBag.ilce = ilce;

            List<SelectListItem> belde = (from x in c.tbl_belde.Where(x => x.ilce_id == ac.ilce_id).ToList()
                                          select new SelectListItem
                                          {
                                              Text = x.belde_ad,
                                              Value = x.belde_id.ToString()
                                          }).ToList();
            ViewBag.belde = belde;

            List<SelectListItem> adres_turu = new List<SelectListItem>();
            adres_turu.Add(new SelectListItem { Text = "Ev Adresi", Value = "Ev Adresi" });
            adres_turu.Add(new SelectListItem { Text = "İş Adresi", Value = "İş Adresi" });
            adres_turu.Add(new SelectListItem { Text = "KPS Adresi", Value = "KPS Adresi" });
            ViewBag.at = adres_turu;
        }
    }
}
