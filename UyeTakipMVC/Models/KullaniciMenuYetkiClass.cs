using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class KullaniciMenuYetkiClass
    {
        [Key]
        public int kmy_id { get; set; }

        [ForeignKey("Kullanici")]
        public int kullanici_id { get; set; }

        [ForeignKey("Menu")]
        public int menu_id { get; set; }

        public bool ekleme { get; set; }
        public bool okuma { get; set; }
        public bool guncelleme { get; set; }
        public bool silme { get; set; }

        public virtual MenuClass? Menu { get; set; }
        public virtual KullaniciClass? Kullanici { get; set; }
    }
}
