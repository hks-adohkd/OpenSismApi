using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomersController(OpenSismDBContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customers
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IndexPost()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var tableData = (from temp in _context.Customers
                                 select new
                                 {
                                     Id = temp.Id,
                                     Name = temp.FirstName + " " + temp.LastName,
                                     ImageUrl = temp.ImageUrl,
                                     LockoutEnabled = temp.User.LockoutEnabled,
                                     TotalPoints = temp.TotalPoints,
                                     CustomerPrizes = temp.CustomerPrizes.Count(),
                                     CustomerTasks = temp.CustomerTasks.Count(),
                                     Group = temp.Group.DisplayName,
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Name.Contains(searchValue) ||
                    m.Group.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .Include(c => c.User)
                .Include(c => c.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .Include(c => c.User)
                .Include(c => c.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.Include(c => c.User).Where(c => c.Id == id).FirstOrDefaultAsync();
            try
            {
                var user = customer.User;
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddYears(3);
                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customer);
            }
        }

        public async Task<IActionResult> UnLock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("UnLock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLockConfirmed(int id)
        {
            var customer = await _context.Customers.Include(c => c.User).Where(c => c.Id == id).FirstOrDefaultAsync();
            try
            {
                var user = customer.User;
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customer);
            }
        }
        
        public async Task<IActionResult> ResetPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordConfirmed(int id, string Password)
        {
            var customer = await _context.Customers.Include(c => c.User).Where(c => c.Id == id).FirstOrDefaultAsync();
            try
            {
                var user = customer.User;
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, Password);
                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    HttpContext.Session.SetString("FailedMsg", FailedMsg);
                    return View(customer);
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customer);
            }
        }

        public IActionResult Message(int customerId)
        {
            ViewBag.customerId = customerId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Message(CustomerMessage customerMessage)
        {
            try
            {
                Message message = customerMessage.Message;
                message.IsForCustomer = true;
                message.IsForAll = false;
                _context.Add(message);
                await _context.SaveChangesAsync();

                Customer c = _context.Customers.Find(customerMessage.CustomerId);

                customerMessage.MessageId = message.Id;
                customerMessage.IsRead = false;
                customerMessage.SendDate = DateTime.Now;
                _context.Add(customerMessage);
                await _context.SaveChangesAsync();

                try { await SendNotification(message, c.FCMToken); }
                catch (Exception e)
                { }

                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customerMessage);
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
