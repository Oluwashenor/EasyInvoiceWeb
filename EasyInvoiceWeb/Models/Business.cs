namespace EasyInvoiceWeb.Models
{
    public class Business : BaseModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
    }
}
