using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UyeTakipMVC.Models
{
    public class AdresClass
    {
        [Key]
        public int adres_id { get; set; }
        
        [ForeignKey("Uye")]
        public string? uye_tckn { get; set; }
        
        public string? adres_tur { get; set; }

        [ForeignKey("Ulke")]
        public int ulke_id { get; set; }

        [ForeignKey("Sehir")]
        public int sehir_id { get; set; }

        [ForeignKey("Ilce")]
        public int ilce_id { get; set; }

        [ForeignKey("Belde")]
        public int belde_id { get; set; }
        public string? mahalle { get; set; }
        public string? bulvar { get; set; }
        public string? cadde { get; set; }
        public string? sokak { get; set; }
        public string? apartman { get; set; }
        public int dis_kapi_no { get; set; }
        public int ic_kapi_no { get; set; }
        public int posta_kodu { get; set; }


        public virtual UyeClass Uye { get; set; }
        public virtual UlkeClass Ulke { get; set; }
        public virtual SehirClass Sehir { get; set; }
        public virtual IlceClass Ilce { get; set; }
        public virtual BeldeClass Belde{ get; set; }
        public virtual UyeTakipContext UyeTakipC { get; set; }
    }
}
