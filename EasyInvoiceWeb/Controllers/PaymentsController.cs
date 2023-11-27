using Microsoft.AspNetCore.Mvc;

namespace EasyInvoiceWeb.Controllers
{
    public class PaymentsController : Controller
    {

        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> VerifyPayment(string reference, string invoice)
        {
            bool response = await PVerifyPayment(reference, invoice);
            return response ? Ok(response) : NotFound();
        }

        private async Task<bool> PVerifyPayment(string reference, string invoice)
        {
            bool response = await _paymentService.VerifyPaystackPayment(reference, invoice);
            return !response ? response : response;
        }
    }
}
