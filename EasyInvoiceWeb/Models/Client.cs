namespace EasyInvoiceWeb.Models
{
    public class Client : BaseModel
    {
        public long BusinessId { get; set; }
        public Business Business { get; set; }
        public string ClientName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
