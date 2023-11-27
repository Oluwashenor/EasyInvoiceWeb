using EasyInvoiceWeb.Data;
using EasyInvoiceWeb.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace EasyInvoiceWeb
{
    public class PaymentService : IPaymentService
    {


        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> VerifyPaystackPayment(string reference, string invoice)
        {
            //var secretKey = _paymentOptions.Value.SecretKey ?? Environment.GetEnvironmentVariable("SecretKey");
            var secretKey = "sk_test_f124ccbc4ed787463d5b79b46effa2367af0a761";
            PaystackPaymentResponse.Root response = VerifyPayment(reference, secretKey);
            if (!response.Status) return false;
            Payment paymentModel = new()
            {
                AmountPaid = response.Data.Amount / 100,
                Reference = response.Data.Reference,
                PaymentDate = response.Data.Transaction_date
            };
            return await CreatePayment(paymentModel, invoice);
        }

        public PaystackPaymentResponse.Root VerifyPayment(string reference, string secretKey)
        {
            string url = "https://api.paystack.co/transaction/verify/";
            string fullUrl = url + reference;
            Dictionary<string, string> headers = CreateDictionary("Authorization", $"Bearer {secretKey}");
            string result = GetAsync(fullUrl, headers);
            PaystackPaymentResponse.Root paystackPaymentResponse = JsonConvert.DeserializeObject<PaystackPaymentResponse.Root>(result);
            return paystackPaymentResponse;
        }

        public static Dictionary<T, T> CreateDictionary<T>(T key, T value)
        {
            Dictionary<T, T> dictionary = new()
            {
                { key, value }
            };
            return dictionary;
        }

        public async Task<bool> CreatePayment(Payment payment, string invoice)
        {

           
            var invoiceFromDb = await _context.Invoice.FirstOrDefaultAsync(x => x.InvoiceNumber == invoice);
            payment.ClientId = invoiceFromDb.ClientId;
            payment.InvoiceId = invoiceFromDb.Id;
            await _context.AddAsync(payment);
            if (payment.AmountPaid < invoiceFromDb.TotalAmount)
            {
                invoiceFromDb.Status = Utilities.Enums.InvoiceStatus.Part;
            }
            else
            {
                invoiceFromDb.Status = Utilities.Enums.InvoiceStatus.Paid;
            }
            _context.Update(invoiceFromDb);
            return await _context.SaveChangesAsync() > 0;
        }

        public static string GetAsync(string url, Dictionary<string, string> headers = null)
        {
            string response;
            using (HttpClient client = new())
            {
                using HttpRequestMessage httpRequestMessage = new();
                httpRequestMessage.RequestUri = new Uri(url);
                httpRequestMessage.Method = HttpMethod.Get;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        httpRequestMessage.Headers.Add(header.Key, header.Value);
                    }
                }
                Task<HttpResponseMessage> httpResponse = client.SendAsync(httpRequestMessage);
                using HttpResponseMessage httpResponseMessage = httpResponse.Result;
                //Status Code
                HttpStatusCode statusCode = httpResponseMessage.StatusCode;
                //Response 
                HttpContent responseContent = httpResponseMessage.Content;
                Task<string> responseData = responseContent.ReadAsStringAsync();
                response = responseData.Result;
            }
            return response;
        }
    }

    public interface IPaymentService
    {
        Task<bool> VerifyPaystackPayment(string reference, string invoice);
    }
}
