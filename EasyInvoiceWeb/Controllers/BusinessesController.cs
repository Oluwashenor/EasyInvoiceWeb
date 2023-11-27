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
    public class BusinessesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BusinessesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Businesses
        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserId(User);
            var viewModel = new BusinessViewModel()
            {
                Businesses = await _context.Business.Where(x=>x.UserId == user).ToListAsync(),
                CreateBusiness = new()
            };
              return _context.Business != null ? 
                          View(viewModel) :
                          Problem("Entity set 'ApplicationDbContext.Business'  is null.");
        }

        // GET: Businesses/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var viewModel = new BusinessDetailViewModel();
            if (id == null || _context.Business == null)
            {
                return NotFound();
            }
            var clients = await _context.Client.Include(c => c.Business).Where(x=>x.BusinessId == id).ToListAsync();
            var business = await _context.Business
                .FirstOrDefaultAsync(m => m.Id == id);
            if (business == null)
            {
                return NotFound();
            }
            viewModel.Business = business;
            viewModel.Clients = clients;    
            return View(viewModel);
        }

        // GET: Businesses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Businesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreateBusiness")]BusinessViewModel business)
        {
            string loggedInUser = _userManager.GetUserId(User);
            ModelState.Clear();
            if (TryValidateModel(business.CreateBusiness, nameof(CreateBusiness)))
            {
                var nameExist = await _context.Business.AnyAsync(x => x.UserId == loggedInUser && x.Name.ToLower().Trim() == business.CreateBusiness.Name.ToLower().Trim());
                if (nameExist)
                {
                    DisplayError("Name Already Exist");
                    return RedirectToAction(nameof(Index));
                }
                var createBusiness = new Business()
                {
                     AccountNumber = business.CreateBusiness.AccountNumber,
                     Address = business.CreateBusiness.Address,
                     Bank = business.CreateBusiness.Bank,
                     Email = business.CreateBusiness.Email,
                     ContactNumber = business.CreateBusiness.ContactNumber,
                     Name = business.CreateBusiness.Name,
                     UserId = loggedInUser
                };
                _context.Add(createBusiness);
                await _context.SaveChangesAsync();
                DisplaySuccessMessage("Successful Operation");
                return RedirectToAction(nameof(Index));
            }
            DisplayError("Unable to Create Business: Something went wrong");
            return RedirectToAction(nameof(Index));
        }

        // GET: Businesses/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Business == null)
            {
                return NotFound();
            }

            var business = await _context.Business.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }
            return View(business);
        }

        // POST: Businesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Email,Name,ContactNumber,Address,UserId,AccountNumber,Bank,Id,Tracking,Created,Modified,IsActive")] Business business)
        {
            if (id != business.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessExists(business.Id))
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
            return View(business);
        }

        // GET: Businesses/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Business == null)
            {
                return NotFound();
            }

            var business = await _context.Business
                .FirstOrDefaultAsync(m => m.Id == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: Businesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Business == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Business'  is null.");
            }
            var business = await _context.Business.FindAsync(id);
            if (business != null)
            {
                _context.Business.Remove(business);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessExists(long id)
        {
          return (_context.Business?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
