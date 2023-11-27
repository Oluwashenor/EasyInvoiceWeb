using EasyInvoiceWeb.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace EasyInvoiceWeb.Controllers
{
    public class BaseController : Controller
    {
        public void DisplaySuccessMessage(string message)
        {
            TempData[Toastr.Success] = message;
        }

        public void DisplayError(string message)
        {
            TempData[Toastr.Error] = message;
        }

        public void DisplayInfo(string message)
        {
            TempData[Toastr.Info] = message;
        }
        public void DisplayWarning(string message)
        {
            TempData[Toastr.Warning] = message;
        }
    }
}
