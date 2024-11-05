using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using OfficeOpenXml;
using System.Net;
using System.Net.Mail;
using UyeTakipMVC.Models;
using UyeTakipMVC.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using SelectPdf;

namespace UyeTakipMVC.Controlles
{
    public class UyeController : Controller
    {
        /// <summary>
        /// Yazar: Yağız Akyüz
        /// Açıklama: Üyeleri konrol ettiğimiz ve onların adreslerine gidebildiğimiz sayfa, ayrıca mail gönderme işlemi yapılıyor.
        /// Tarih: 25.01.2023
        /// </summary>

        private string m_kod = "1.0";
        UyeTakipContext c = new UyeTakipContext();
        UyeRepository uyeR = new UyeRepository();

        //  Uyarı mesajları için
        private readonly IToastNotification _toastNotification;
        public UyeController(IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);
            if(yetki == null)
            {
                var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                
                if(rol.okuma == true)
                    return View(uyeR.TList());
                else
                {
                    _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (yetki.okuma == true)
                return View(uyeR.TList());
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult NewUye()
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
                    return RedirectToAction("Index", "Uye");
                }
            }
            else if (yetki.ekleme == true)
                return View();
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Uye");
            }
        }

        [HttpPost]
        public IActionResult NewUye(UyeClass p)
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
                        uyeR.TAdd(p);
                        _toastNotification.AddSuccessToastMessage("Üye ekleme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Uye");
                    }
                }
                else if (yetki.ekleme == true)
                {
                    uyeR.TAdd(p);
                    _toastNotification.AddSuccessToastMessage("Üye ekleme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Ekleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Uye");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Üye ekleme başarısız!");
                return View(p);
            }
            return RedirectToAction("Index", "Uye");
        }


        [HttpGet]
        public IActionResult GetUye(string id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            var x = uyeR.TGet(id);
            UyeClass uc = new UyeClass()
            {
                tckn = x.tckn,
                ad = x.ad,
                soyad = x.soyad,
                unvan = x.unvan,
                telefon_no = x.telefon_no,
                eposta = x.eposta,
                sifre = x.sifre
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
                    return RedirectToAction("Index", "Uye");
                }
            }
            else if (yetki.guncelleme == true)
                return View(uc);
            else
            {
                _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                return RedirectToAction("Index", "Uye");
            }
        }

        [HttpPost]
        public IActionResult UpdateUye(UyeClass p)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = uyeR.TGet(p.tckn);
                x.ad = p.ad;
                x.soyad = p.soyad;
                x.unvan = p.unvan;
                x.telefon_no = p.telefon_no;
                x.eposta = p.eposta;

                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.guncelleme == true)
                    {
                        _toastNotification.AddSuccessToastMessage("Güncelleme başarılı.");
                        uyeR.TUpdate(x);
                    }
                        
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Bu alana giriş yapmak için yetkiniz yok.");
                        return RedirectToAction("Index", "Uye");
                    }
                }
                else if (yetki.guncelleme == true)
                {
                    _toastNotification.AddSuccessToastMessage("Güncelleme başarılı.");
                    uyeR.TUpdate(x);
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Güncelleme yapmak için yetkiniz yok.");
                    return RedirectToAction("Index", "Uye");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddSuccessToastMessage("Güncelleme başarılı!");
                return View(p);
            }
            return RedirectToAction("Index", "Uye");
        }

        public IActionResult DeleteUye(string id)
        {
            var menuCheck = c.tbl_menu.FirstOrDefault(x => x.menu_kod == m_kod);
            var k_id = HttpContext.Request.Cookies["k_id"];
            var yetki = c.tbl_kullanici_menu_yetki.Include("Kullanici").FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id) && x.menu_id == menuCheck.menu_id);

            try
            {
                var x = uyeR.TGet(id);
                if (yetki == null)
                {
                    var rolCheck = c.tbl_kullanici_rol.FirstOrDefault(x => x.kullanici_id == Convert.ToInt32(k_id));
                    var rol = c.tbl_rol_menu_yetki.FirstOrDefault(x => x.rol_id == rolCheck.rol_id && x.menu_id == menuCheck.menu_id);
                    if (rol.silme == true)
                    {
                        uyeR.TDelete(x);
                        c.SaveChanges();
                        _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                        return RedirectToAction("Index", "Uye");
                    }
                }
                else if (yetki.silme == true)
                {
                    uyeR.TDelete(x);
                    c.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Silme başarılı.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Silmek için yetkiniz yok.");
                    return RedirectToAction("Index", "Uye");
                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Bu üyeyi silemezsiniz!");
            }
            return RedirectToAction("Index", "Uye");
        }

        [HttpGet]
        public IActionResult GetAddressDetail(string id)
        {
            ViewBag.tckn = id;
            var address = c.tbl_adres.Where(x => x.uye_tckn == id).Include("Uye").Include("Ulke").Include("Sehir").Include("Ilce").Include("Belde").ToList();
            var x = new UyeClass
            {
                AdresC = address
            };
            return View(x);
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                List<UyeClass> s = c.tbl_uye.Select(p => new UyeClass
                {
                    tckn = p.tckn,
                    ad = p.ad,
                    soyad = p.soyad,
                    unvan = p.unvan,
                    telefon_no = p.telefon_no,
                    eposta = p.eposta
                }).ToList();

                ExcelWorksheet ew = package.Workbook.Worksheets.Add("Report");

                ew.Cells["A1"].Value = "Üye TCKN";
                ew.Cells["B1"].Value = "Üye Ad";
                ew.Cells["C1"].Value = "Üye Soyad";
                ew.Cells["D1"].Value = "Ünvan";
                ew.Cells["E1"].Value = "Telefon No";
                ew.Cells["F1"].Value = "E-Posta";

                int rowStart = 1;
                foreach (var item in s)
                {
                    rowStart++;
                    ew.Cells[string.Format("A{0}", rowStart)].Value = item.tckn;
                    ew.Cells[string.Format("B{0}", rowStart)].Value = item.ad;
                    ew.Cells[string.Format("C{0}", rowStart)].Value = item.soyad;
                    ew.Cells[string.Format("D{0}", rowStart)].Value = item.unvan;
                    ew.Cells[string.Format("E{0}", rowStart)].Value = item.telefon_no;
                    ew.Cells[string.Format("F{0}", rowStart)].Value = item.eposta;
                }

                ew.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                var excelData = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Üyeler.xlsx";
                return File(excelData, contentType, fileName);
            }
        }

        [HttpGet]
        public IActionResult SendMail(string id)
        {
            var k_id = HttpContext.Request.Cookies["k_id"];
            List<SelectListItem> eposta = (from x in c.tbl_parametre.Where(x => x.kullanici_id == Convert.ToInt32(k_id)).Include("ParametreId").ToList()
                                            select new SelectListItem
                                            {
                                                Text = x.ParametreId.parametre_ad,
                                                Value = x.parametre_icerigi
                                            }).ToList();
            ViewBag.eposta = eposta;

            List<SelectListItem> sifre = (from x in c.tbl_parametre.Where(x => x.kullanici_id == Convert.ToInt32(k_id)).Include("ParametreId").ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.ParametreId.parametre_ad,
                                               Value = x.parametre_icerigi
                                           }).ToList();
            ViewBag.sifre = sifre;

            var z = uyeR.TGet(id);
            EmailClass ec = new EmailClass()
            {
                Email = z.eposta
            };
            return View(ec);
        }

        [HttpPost]
        public IActionResult SendMail(EmailClass p)
        {
            string? fromMail = p.FromEmail;
            string? fromPassword = p.FromPassword;

            string? toMail = p.Email;
            string? subject = p.Subject;
            string? body = p.Body;

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(fromMail, fromPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            smtp.Host = "smtp.office365.com";
            smtp.EnableSsl = true;
            smtp.Timeout = 600000;

            try
            {
                smtp.Send(fromMail, toMail, subject, body);
                _toastNotification.AddSuccessToastMessage("Mail gönderme başarılı.");
                return RedirectToAction("Index", "Uye");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Mail gönderme başarısız! Parametreleri doğru seçtiğinize emin olun.");
                return RedirectToAction("SendMail", "Uye");
            }
        }

        [HttpGet]
        public IActionResult IdCardPDF(string id)
        {
            var address = c.tbl_adres.Where(x => x.uye_tckn == id).Include("Uye").Include("Ulke").Include("Sehir").Include("Ilce").Include("Belde").ToList();
            var uye = uyeR.TGet(id);
            var x = new UyeClass
            {
                AdresC = address,
                tckn = uye.tckn,
                ad = uye.ad,
                soyad = uye.soyad,
                unvan = uye.unvan,
                telefon_no = uye.telefon_no,
                eposta = uye.eposta
            };
            return View(x);
        }

        [HttpPost]
        public IActionResult GeneratePdf(string html)
        {
            html = html.Replace("StrTag", "<").Replace("EndTag", ">");

            HtmlToPdf htmlToPdf = new HtmlToPdf();
            PdfDocument pdfDocument = htmlToPdf.ConvertHtmlString(html);
            byte[] pdf = pdfDocument.Save();
            pdfDocument.Close();

            return File(pdf, "application/pdf", "uyeKart.pdf");
        }
    }
}
