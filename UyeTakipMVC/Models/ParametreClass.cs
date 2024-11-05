using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class ParametreClass
    {
        [Key]
        public int parametre_id { get; set; }

        [ForeignKey("Kullanici")]
        public int kullanici_id { get; set; }

        [ForeignKey("ParametreId")]
        public int parametre_ad_id { get; set; }

        public string? parametre_icerigi { get; set; }

        public virtual KullaniciClass Kullanici { get; set; }
        public virtual ParametreAdClass ParametreId { get; set; }
    }
}
