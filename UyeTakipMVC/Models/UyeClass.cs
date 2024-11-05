using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class UyeClass
    {
        [Key]
        public string? tckn { get; set; }

        public string? ad { get; set; }
        public string? soyad { get; set; }
        public string? unvan { get; set; }
        public string? telefon_no { get; set; }
        public string? eposta { get; set; }
        public string? sifre { get; set; }

        public List<AdresClass>? AdresC { get; set; }
    }
}
