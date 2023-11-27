namespace EasyInvoiceWeb.Models
{
    public class Payment :BaseModel
    {
        public long InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public DateTime PaymentDate { get; set; }
        public double AmountPaid { get; set; }
        public string Reference { get; set; }
        public long ClientId { get; set; }
    }
}
