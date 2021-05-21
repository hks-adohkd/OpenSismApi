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
    public class CustomerMessagesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private IHostingEnvironment env;

        public CustomerMessagesController(OpenSismDBContext context, IHostingEnvironment env) : base(context, env)
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

                //var customerMessageData =  _context.CustomerMessages.Where(c => c.IsDeleted == isArchive);


                //var customerData = _context.Customers.Where() 
                var tableData = (from temp in _context.CustomerMessages.Where(c => c.IsDeleted == isArchive)
                                 .Where (a=> a.Message.IsForCustomer == true)
                                 select new
                                 {
                                     Id = temp.Id,
                                     Script = temp.Message.Script,
                                     ScriptAr = temp.Message.ScriptAr,
                                     Title = temp.Message.Title,
                                     TitleAr = temp.Message.TitleAr,
                                     Name = temp.Customer.FirstName + "  " + temp.Customer.LastName,
                                     Created = temp.Created,
                                     PhoneNumber = temp.Customer.User.PhoneNumber,
                                    // Message = temp.Message,
                                     MessageId = temp.MessageId,
                                     CustomerId = temp.CustomerId

                                 });
                
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                else
                {
                 //   tableData = tableData.OrderBy("Created" + " " + "DESC");
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Title.Contains(searchValue) || m.TitleAr.Contains(searchValue) ||
                    m.Name.Contains(searchValue) || m.PhoneNumber.Contains(searchValue));
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

            var customerMessage = await _context.CustomerMessages
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (customerMessage == null)
            {
                return NotFound();
            }
            ViewBag.TitleEn = customerMessage.Message.Title;
            ViewBag.TitleAr = customerMessage.Message.TitleAr;
            ViewBag.Script = customerMessage.Message.Script;
            ViewBag.ScriptAr = customerMessage.Message.ScriptAr;
            ViewBag.Name = customerMessage.Customer.FirstName + "  " + customerMessage.Customer.LastName;
            ViewBag.PhoneNumber = customerMessage.Customer.User.PhoneNumber;

            return View(customerMessage);
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
