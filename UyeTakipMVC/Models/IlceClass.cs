using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class IlceClass
    {
        [Key]
        public int ilce_id { get; set; }

        [ForeignKey("Sehir")]
        public int sehir_id { get; set; }
        public string? ilce_ad { get; set; }

        public virtual SehirClass Sehir { get; set; }

        public List<BeldeClass> BeldeC { get; set; }
        public List<AdresClass> AdresC { get; set; }
    }
}
