using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class RolClass
    {
        [Key]
        public int rol_id { get; set; }
        public string rol_ad { get; set; }

        public List<KullaniciRolClass> KullaniciRolC { get; set; }
        public List<RolMenuYetkiClass> RolMenuYetkiC { get; set; }
    }
}
