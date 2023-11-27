using EasyInvoiceWeb.Models;

namespace EasyInvoiceWeb.ViewModels
{
    public class BusinessViewModel
    {
        public IEnumerable<Business> Businesses { get; set; }
        public CreateBusiness CreateBusiness { get; set; }
    }

    public class BusinessDetailViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public Business Business { get; set; }
        public CreateClientViewModel CreateClient { get; set; }
    }

    public class CreateBusiness
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Bank { get; set; }
    }


}
