using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class KullaniciClass
    {
        [Key]
        public int kullanici_id { get; set; }
        public string? kullanici_ad { get; set; }
        public string? sifre { get; set; }

        public List<ParametreClass> ParametreC { get; set; }
        public List<KullaniciRolClass> KullaniciRolC { get; set; }
    }
}
