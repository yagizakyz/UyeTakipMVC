using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class KullaniciRolClass
    {
        [Key]
        public int kr_id { get; set; }

        [ForeignKey("Rol")]
        public int rol_id { get; set; }

        [ForeignKey("Kullanici")]
        public int kullanici_id { get; set; }

        public virtual RolClass Rol { get; set; }
        public virtual KullaniciClass Kullanici { get; set; }
    }
}
