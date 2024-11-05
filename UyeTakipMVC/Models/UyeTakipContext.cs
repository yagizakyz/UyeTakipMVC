using Microsoft.EntityFrameworkCore;
using UyeTakipMVC.Controlles;

namespace UyeTakipMVC.Models
{
    public class UyeTakipContext : DbContext
    {
        static string connectionString = "";
        public void ServerInf(string server, string user, string password, string db)
        {
            connectionString = @"server=" + server + ";port=5432; user ID=" + user + "; password=" + password + "; Database=" + db + "";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  @"server=localhost;port=5432; user ID=postgres; password=Npgsql61; Database=uye_takip"
            optionsBuilder.UseNpgsql(connectionString);
        }

        public DbSet<UyeClass> tbl_uye { get; set; }
        public DbSet<AdresClass> tbl_adres { get; set; }
        public DbSet<UlkeClass> tbl_ulke { get; set; }
        public DbSet<SehirClass> tbl_sehir { get; set; }
        public DbSet<IlceClass> tbl_ilce { get; set; }
        public DbSet<BeldeClass> tbl_belde { get; set; }
        public DbSet<KullaniciClass> tbl_kullanici { get; set; }
        public DbSet<ParametreClass> tbl_parametre { get; set; }
        public DbSet<ParametreAdClass> tbl_parametre_ad { get; set; }
        public DbSet<MenuClass> tbl_menu { get; set; }
        public DbSet<KullaniciMenuYetkiClass> tbl_kullanici_menu_yetki { get; set; }
        public DbSet<RolClass> tbl_rol { get; set; }
        public DbSet<KullaniciRolClass> tbl_kullanici_rol { get; set; }
        public DbSet<RolMenuYetkiClass> tbl_rol_menu_yetki { get; set; }
    }
}
