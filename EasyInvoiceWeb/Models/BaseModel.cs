namespace EasyInvoiceWeb.Models
{
    public class BaseModel
    {
        public long Id { get; set; }
        public string Tracking { get; set; } = Guid.NewGuid().ToString();
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
