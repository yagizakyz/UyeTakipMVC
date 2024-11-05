namespace UyeTakipMVC.Models
{
    public class MyTuple
    {
        //  MyTuple sınıfı ile aynı anda iki tabloya veri ekleyebiliyoruz.
        //  Burada kullanıcı ve kullanıcıRol tablolarına veri göndermek için bu sınıfı oluşturduk.
        public string kullanici_ad { get; set; }
        public string sifre { get; set; }
        public int rol_id { get; set; }
    }
}
