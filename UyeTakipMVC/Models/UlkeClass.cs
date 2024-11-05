using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UyeTakipMVC.Models
{
    public class UlkeClass
    {
        [Key]
        public int ulke_id { get; set; }

        public string? ulke_ad { get; set; }

        public List<SehirClass>? SehirC { get; set; }
        public List<AdresClass>? AdresC { get; set; }
    }
}
