using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class BeldeClass
    {
        [Key]
        public int belde_id { get; set; }

        [ForeignKey("Ilce")]
        public int ilce_id { get; set; }
        public string? belde_ad { get; set; }

        public virtual IlceClass Ilce { get; set; }
        public List<AdresClass> AdresC { get; set; }
    }
}
