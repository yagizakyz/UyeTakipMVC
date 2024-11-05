using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class MenuClass
    {
        [Key]
        public int menu_id { get; set; }

        public string? menu_ad { get; set; }
        public string? aciklama { get; set; }
        public string? menu_kod { get; set; }
        public string? controller_name { get; set; }

        public List<KullaniciMenuYetkiClass>? KullaniciYetkiMenuC { get; set; }
        public List<RolMenuYetkiClass>? RolYetkiMenuC { get; set; }
    }
}
