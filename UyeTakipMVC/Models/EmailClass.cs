namespace UyeTakipMVC.Models
{
    public class EmailClass
    {
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        public string? FromEmail { get; set;}
        public string? FromPassword { get; set; }

        public int k_id { get; set; }
    }
}
