using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EasyInvoiceWeb.Models;

namespace EasyInvoiceWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EasyInvoiceWeb.Models.Business>? Business { get; set; }
        public DbSet<EasyInvoiceWeb.Models.Client>? Client { get; set; }
        public DbSet<Invoice>? Invoice { get; set; }
        public DbSet<Payment>? Payment { get; set; }
    }
}