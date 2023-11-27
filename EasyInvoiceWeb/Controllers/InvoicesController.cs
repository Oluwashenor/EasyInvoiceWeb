using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyInvoiceWeb.Data;
using EasyInvoiceWeb.Models;
using EasyInvoiceWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EasyInvoiceWeb.Controllers
{
    [Authorize]
    public class InvoicesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InvoicesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserId(User);
            var businesses = await _context.Business.Where(x => x.UserId == user).Select(x=>x.Id).ToListAsync();
            var invoices = _context.Invoice.Where(x=> businesses.Contains(x.BusinessId)).Include(i => i.Business).Include(i => i.Client).Select(x=> new InvoiceViewModel()
            {
                 Business = x.Business.Name,
                 BusinessId = x.BusinessId,
                 Client = x.Client.ClientName,
                 ClientEmail = x.Client.Email,
                 ClientId = x.ClientId,
                 DueDate = x.DueDate,
                 Id = x.Id,
                 InvoiceNumber = x.InvoiceNumber,
                 Status = x.Status,
                 TotalAmount = x.TotalAmount,
                 IssuedDate = x.IssueDate
            }).ToList();
            return View(invoices);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .Include(i => i.Business)
                .Include(i => i.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DetailsById(string? id)
        {

            var invoice = await _context.Invoice
                .Include(i => i.Business)
                .Include(i => i.Client)
                .FirstOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null)
            {
                return NotFound();
            }
            var payments = await _context.Payment.Where(x => x.InvoiceId == invoice.Id).ToListAsync();
            var invoiceData = new InvoiceViewModel()
            {
                Business = invoice.Business.Name,
                TotalAmount = invoice.TotalAmount,
                InvoiceNumber = invoice.InvoiceNumber,
                Status = invoice.Status,
                BusinessId = invoice.Business.Id,
                Client = invoice.Client.ClientName,
                ClientId = invoice.ClientId,
                DueDate = invoice.DueDate,
                Id = invoice.Id,
                ClientEmail = invoice.Client.Email
            };
            var viewModel = new InvoiceDetail()
            {
                Invoice = invoiceData,
                Payments = payments
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> InvoiceStatus(string invoiceNumber)
        {
            var invoice = await _context.Invoice.FirstOrDefaultAsync(x=>x.InvoiceNumber ==  invoiceNumber);
            var status = invoice == null ? false : invoice.Status == Utilities.Enums.InvoiceStatus.Paid;
            return Ok(status);
        }

        // GET: Invoices/Create
        //public IActionResult Create()
        //{
        //    ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id");
        //    ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id");
        //    return View();
        //}

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientDetailViewModel model)
        {
            ModelState.Clear();
            if (TryValidateModel(model.CreateInvoice, nameof(CreateInvoiceViewModel)))
            {
                if(model.CreateInvoice.TotalAmount <= 0)
                {
                    DisplayError("Amount cannot be 0 or less");
                    return RedirectToAction(nameof(Details), "Clients", new { id = model.CreateInvoice.ClientId });
                }
                if(model.CreateInvoice.DueDate < DateTime.Now)
                {
                    DisplayError("Due Date has to be in the future");
                    return RedirectToAction(nameof(Details), "Clients", new { id = model.CreateInvoice.ClientId });
                }
                var invoice = new Invoice()
                {
                    DueDate = model.CreateInvoice.DueDate,
                    InvoiceNumber = Guid.NewGuid().ToString(),
                    BusinessId = model.CreateInvoice.BusinessId,
                    ClientId = model.CreateInvoice.ClientId,
                    Status = Utilities.Enums.InvoiceStatus.Unpaid,
                    TotalAmount = model.CreateInvoice.TotalAmount,
                    Description = model.CreateInvoice.Description,
                };
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Clients", new { id = model.CreateInvoice.ClientId });
            }
            //ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", invoice.BusinessId);
            //ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id", invoice.ClientId);
            return RedirectToAction(nameof(Details), "Clients", new { id = model.CreateInvoice.ClientId });
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", invoice.BusinessId);
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id", invoice.ClientId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ClientId,BusinessId,InvoiceNumber,IssueDate,DueDate,TotalAmount,Status,Id,Tracking,Created,Modified,IsActive")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", invoice.BusinessId);
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id", invoice.ClientId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .Include(i => i.Business)
                .Include(i => i.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Invoice == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Invoice'  is null.");
            }
            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoice.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(long id)
        {
            return (_context.Invoice?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
