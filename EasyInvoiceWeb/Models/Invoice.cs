using static EasyInvoiceWeb.Utilities.Enums;

namespace EasyInvoiceWeb.Models
{
    public class Invoice : BaseModel
    {
        public long ClientId { get; set; }
        public Client Client { get; set; }
        public long BusinessId { get; set; }
        public Business Business { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; } // e.g., Paid, Unpaid
    }
}
