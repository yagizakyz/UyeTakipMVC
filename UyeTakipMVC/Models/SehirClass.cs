using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class SehirClass
    {
        [Key]
        public int sehir_id { get; set; }

        [ForeignKey("Ulke")]
        public int ulke_id { get; set; }

        public string? sehir_ad { get; set; }
        public int plaka_kodu { get; set; }
        public virtual UlkeClass Ulke { get; set; }

        public List<AdresClass> AdresC { get; set; }
        public List<IlceClass> IlceC { get; set; }
    }
}
