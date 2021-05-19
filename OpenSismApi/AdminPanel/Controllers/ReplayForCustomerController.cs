using DBContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class ReplayForCustomerController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private IHostingEnvironment env;

        public ReplayForCustomerController(OpenSismDBContext context, IHostingEnvironment env) : base(context, env)
        {
            _context = context;
            this.env = env;
        }

        // GET: ContactUs
        public IActionResult Index(bool? isArchive)
        {
            if (isArchive != null)
            {
                ViewBag.isArchive = isArchive;
            }
            else
            {
                ViewBag.isArchive = false;
            }
            return View();
        }

        [HttpPost]
        public IActionResult IndexPost(bool isArchive)
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

                var tableData = (from temp in _context.ContactsUs.Where(c => c.IsDeleted == isArchive && c.Reply != null)
                                 select new
                                 {
                                     Id = temp.Id,
                                     Name = temp.FirstName + " " + temp.LastName,
                                     Subject = temp.Subject,
                                     IsFeatured = temp.IsFeatured,
                                     IsViewed = temp.IsViewed,
                                     Created = temp.Created,
                                     Message = temp.Message,
                                     CustomerId = temp.CustomerId
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                else
                {
                    tableData = tableData.OrderBy("Created" + " " + "DESC");
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Subject.Contains(searchValue) ||
                    m.Name.Contains(searchValue));
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

        // GET: ContactUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactsUs
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactUs == null)
            {
                return NotFound();
            }
            try
            {
                contactUs.IsViewed = true;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
            return View(contactUs);
        }

        public async Task<IActionResult> Reply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactsUs.Include(c => c.Customer)
                .Include(c => c.Customer.User).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (contactUs == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = contactUs.CustomerId;
            try
            {
                contactUs.IsViewed = true;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
            return View(contactUs);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, ContactUs contactUs)
        {
            if (id != contactUs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactUs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactUsExists(contactUs.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var contact = await _context.ContactsUs.Include(c => c.Customer).Where(c => c.Id == contactUs.Id).FirstOrDefaultAsync();
                if (contact.CustomerId != null)
                {
                    Message message = new Message();
                    message.IsForAll = false;
                    message.Script = contact.Reply;
                    message.ScriptAr = contact.Reply;
                    message.Title = contact.Subject;
                    message.TitleAr = contact.Subject;
                    message.IsForCustomer = true;
                    _context.Add(message);
                    await _context.SaveChangesAsync();
                    CustomerMessage customerMessage = new CustomerMessage();
                    customerMessage.CustomerId = (int)contact.CustomerId;
                    customerMessage.IsRead = false;
                    customerMessage.MessageId = message.Id;
                    customerMessage.SendDate = DateTime.Now;
                    _context.Add(customerMessage);
                    await _context.SaveChangesAsync();
                   // await SendNotification(message, contact.Customer.FCMToken);
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = contactUs.CustomerId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(contactUs);
        }

        // GET: ContactUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactsUs.FindAsync(id);
            if (contactUs == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = contactUs.CustomerId;
            try
            {
                contactUs.IsViewed = true;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
            return View(contactUs);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactUs contactUs)
        {
            if (id != contactUs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactUs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactUsExists(contactUs.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = contactUs.CustomerId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(contactUs);
        }

        // GET: ContactUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactsUs
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactUs == null)
            {
                return NotFound();
            }
            try
            {
                contactUs.IsViewed = true;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
            return View(contactUs);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactUs = await _context.ContactsUs.FindAsync(id);
            try
            {
                contactUs.IsDeleted = true;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(contactUs);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactUs = await _context.ContactsUs
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactUs == null)
            {
                return NotFound();
            }
            return View(contactUs);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var contactUs = await _context.ContactsUs.FindAsync(id);
            try
            {
                contactUs.IsDeleted = false;
                _context.Update(contactUs);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(contactUs);
            }
        }

        private bool ContactUsExists(int id)
        {
            return _context.ContactsUs.Any(e => e.Id == id);
        }
    }
}
