using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class ParametreAdClass
    {
        [Key]
        public int parametre_ad_id { get; set; }

        public string parametre_ad { get; set; }

        public List<ParametreClass> ParametreC { get; set; }
    }
}
