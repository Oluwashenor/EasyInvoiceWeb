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
using Microsoft.AspNetCore.Identity;

namespace EasyInvoiceWeb.Controllers
{
    public class ClientsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Client.Include(c => c.Business);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.Business)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }
            var invoices = await _context.Invoice
                .Where(m => m.ClientId == id).ToListAsync();
            var viewModel = new ClientDetailViewModel()
            {
                Client = new ClientViewModel()
                {
                    Address = client.Address,
                    Business = client.Business.Name,
                    BusinessId = client.Business.Id,
                    ContactNumber = client.ContactNumber,
                    Email = client.Email,
                    Id = client.Id,
                    Name = client.ClientName
                },
                CreateInvoice = new()
                {
                    DueDate = DateTime.Now
                },
                Invoices = invoices.Select(x=> new InvoiceViewModel()
                {
                     Id = x.Id,
                     Business = client.Business.Name,
                     BusinessId= client.Business.Id,
                     TotalAmount = x.TotalAmount,
                     DueDate = x.DueDate,
                     InvoiceNumber = x.InvoiceNumber,
                     Status = x.Status
                })
            };

            return View(viewModel);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessDetailViewModel model)
        {
            string loggedInUser = _userManager.GetUserId(User);
            ModelState.Clear();
            if (TryValidateModel(model.CreateClient, nameof(CreateClientViewModel)))
            {
                var existingClients = await _context.Client.Where(x => x.BusinessId == model.CreateClient.BusinessId).ToListAsync();
                var emailExist = existingClients.Any(x=> x.Email.ToLower() == model.CreateClient.Email.ToLower());
                if(emailExist)
                {
                    DisplayError("Email Already Exist");
                    return RedirectToAction(nameof(Details), "Businesses", new { id = model.CreateClient.BusinessId });
                };
                var contactPhoneExist = existingClients.Any(x => x.ContactNumber== model.CreateClient.ContactNumber);
                if (contactPhoneExist)
                {
                    DisplayError("Phone Already Exist");
                    return RedirectToAction(nameof(Details), "Businesses", new { id = model.CreateClient.BusinessId });
                };
                var client = new Client()
                {
                    Address = model.CreateClient.Address,
                    BusinessId = model.CreateClient.BusinessId,
                    ClientName = model.CreateClient.Name,
                    ContactNumber = model.CreateClient.ContactNumber,
                    Email = model.CreateClient.Email,
                };
                _context.Add(client);
                await _context.SaveChangesAsync();
                DisplaySuccessMessage("Successful Operation");
                return RedirectToAction(nameof(Details), "Businesses", new { id = model.CreateClient.BusinessId });
            }
        //    ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", client.BusinessId);
            return RedirectToAction( nameof(Details), "Businesses", new {id = model.CreateClient.BusinessId});
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", client.BusinessId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("BusinessId,ClientName,ContactPerson,ContactNumber,Email,Address,Id,Tracking,Created,Modified,IsActive")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            ViewData["BusinessId"] = new SelectList(_context.Business, "Id", "Id", client.BusinessId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Client == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.Business)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Client == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Client'  is null.");
            }
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(long id)
        {
          return (_context.Client?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
