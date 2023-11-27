using EasyInvoiceWeb.Models;

namespace EasyInvoiceWeb.ViewModels
{
    public class ClientViewModel
    {
        public long BusinessId { get; set; }
        public long Id { get; set; }
        public string Business { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class CreateClientViewModel
    {
        public long BusinessId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }


    public class ClientDetailViewModel
    {
        public ClientViewModel Client { get; set; }
        public CreateInvoiceViewModel CreateInvoice { get; set; }
        public IEnumerable<InvoiceViewModel> Invoices { get; set; }
    }
}
