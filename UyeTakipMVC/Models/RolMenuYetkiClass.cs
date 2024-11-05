using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class RolMenuYetkiClass
    {
        [Key]
        public int rmy_id { get; set; }

        [ForeignKey("Rol")]
        public int rol_id { get; set; }

        [ForeignKey("Menu")]
        public int menu_id { get; set; }

        public bool ekleme { get; set; }
        public bool okuma { get; set; }
        public bool guncelleme { get; set; }
        public bool silme { get; set; }

        public virtual MenuClass Menu { get; set; }
        public virtual RolClass Rol { get; set; }
    }
}
