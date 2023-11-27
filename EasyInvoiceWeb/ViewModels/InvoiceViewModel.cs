using EasyInvoiceWeb.Models;
using static EasyInvoiceWeb.Utilities.Enums;

namespace EasyInvoiceWeb.ViewModels
{
    public class InvoiceViewModel
    {
        public long ClientId { get; set; }
        public long Id { get; set; }
        public string Client { get; set; }
        public string ClientEmail { get; set; }
        public long BusinessId { get; set; }
        public string Business { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DueDate { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    public class CreateInvoiceViewModel
    {
        public long ClientId { get; set; }
        public long BusinessId { get; set; }
        public DateTime DueDate { get; set; }
        public double TotalAmount { get; set; }
    }

    public class InvoiceDetail
    {
        public InvoiceViewModel Invoice { get; set; }
        public IEnumerable<Payment> Payments { get; set; }
    }
}
