using DBContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class CustomerPrizesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        private IHostingEnvironment env;

        public CustomerPrizesController(OpenSismDBContext context, IHostingEnvironment env) : base(context, env)
        {
            _context = context;
            this.env = env;
        }

        // GET: CustomerPrizes
        public IActionResult Index(int id)
        {
            ViewBag.CustomerId = id;
            return View();
        }

        [HttpPost]
        public IActionResult IndexPost(int id)
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

                var tableData = (from temp in _context.CustomerPrizes.Where(c => c.CustomerId == id)
                                 select new
                                 {
                                     Id = temp.Id,
                                     Prize = temp.Prize.DisplayName,
                                     Name = temp.Customer.FirstName + " " + temp.Customer.LastName,
                                     RequestDate = temp.RequestDate.ToString("MM/dd/yyyy hh:mm tt"),
                                     PrizeStatus = temp.PrizeStatus.DisplayName,
                                     CustomerName = temp.Customer.FirstName + " " + temp.Customer.LastName
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Prize.Contains(searchValue));
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

        // GET: CustomerPrizes
        public IActionResult PendingIndex()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PendingIndexPost()
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

                PrizeStatus prizeStatus = _context.PrizeStatuses.Where(p => p.Name == "requested").FirstOrDefault();
                var tableData = (from temp in _context.CustomerPrizes.Include(p => p.PrizeStatus)
                                 .Where(c => c.PrizeStatusId == prizeStatus.Id)
                                 select new
                                 {
                                     Id = temp.Id,
                                     Prize = temp.Prize.DisplayName,
                                     Email = temp.Customer.User.Email,
                                     RequestDate = temp.RequestDate,
                                     PrizeStatus = temp.PrizeStatus.DisplayName,
                                     CustomerName = temp.Customer.FirstName + " " + temp.Customer.LastName
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Prize.Contains(searchValue) ||
                    m.Prize.Contains(searchValue));
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

        // GET: CustomerPrizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPrize = await _context.CustomerPrizes
                .Include(c => c.Customer)
                .Include(c => c.Prize)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerPrize == null)
            {
                return NotFound();
            }
            ViewBag.CustomerId = customerPrize.CustomerId;
            return View(customerPrize);
        }

        public async Task<IActionResult> Accept(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPrize = await _context.CustomerPrizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerPrize == null)
            {
                return NotFound();
            }
            ViewBag.Script = "Congratulations, You Prize " + customerPrize.Prize.DisplayName + " is Accepted";
            ViewBag.ScriptAr = "مبروك، لقد تم قبول جائزتك " + customerPrize.Prize.DisplayNameAr;
            return View(customerPrize);
        }

        [HttpPost, ActionName("Accept")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptConfirmed(int id, string Script, string ScriptAr)
        {
            var customerPrize = await _context.CustomerPrizes.FindAsync(id);
            ViewBag.Script = "Congratulations, You Prize " + customerPrize.Prize.DisplayName + " is Accepted";
            ViewBag.ScriptAr = "مبروك، لقد تم قبول جائزتك " + customerPrize.Prize.DisplayNameAr;
            try
            {
                PrizeStatus prizeStatus = _context.PrizeStatuses.Where(p => p.Name == "accepted").FirstOrDefault();
                customerPrize.PrizeStatusId = prizeStatus.Id;
                customerPrize.EarnDate = DateTime.Now;
                _context.Update(customerPrize);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                Message message = new Message();
                message.IsForAll = false;
                message.Script = Script;
                message.ScriptAr = ScriptAr;
                message.Title = "Prize Accepted";
                message.TitleAr = "جائزة مقبولة";
                message.IsForCustomer = true;
                _context.Add(message);
                await _context.SaveChangesAsync();
                CustomerMessage customerMessage = new CustomerMessage();
                customerMessage.CustomerId = customerPrize.CustomerId;
                customerMessage.IsRead = false;
                customerMessage.MessageId = message.Id;
                customerMessage.SendDate = DateTime.Now;
                _context.Add(customerMessage);
                await _context.SaveChangesAsync();
                try
                {
                  //  await SendNotification(message, customerPrize.Customer.FCMToken);
                }
                catch (Exception e)
                {
                    string m = e.Message;
                }
                return RedirectToAction("PendingIndex");

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customerPrize);
            }
        }

        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPrize = await _context.CustomerPrizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerPrize == null)
            {
                return NotFound();
            }
            ViewBag.Script = "Sorry, You Prize " + customerPrize.Prize.DisplayName + " is Rejected";
            ViewBag.ScriptAr = "للأسف، لقد تم رفض جائزتك " + customerPrize.Prize.DisplayNameAr;
            return View(customerPrize);
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id, string Script, string ScriptAr)
        {
            var customerPrize = await _context.CustomerPrizes.FindAsync(id);
            ViewBag.Script = "Sorry, You Prize " + customerPrize.Prize.DisplayName + " is Rejected";
            ViewBag.ScriptAr = "للأسف، لقد تم رفض جائزتك " + customerPrize.Prize.DisplayNameAr;
            try
            {
                PrizeStatus prizeStatus = _context.PrizeStatuses.Where(p => p.Name == "rejected").FirstOrDefault();
                customerPrize.PrizeStatusId = prizeStatus.Id;
                customerPrize.EarnDate = DateTime.Now;
                _context.Update(customerPrize);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                Message message = new Message();
                message.IsForAll = false;
                message.Script = Script;
                message.ScriptAr = ScriptAr;
                message.Title = "Prize Rejected";
                message.TitleAr = "جائزة مرفوضة";
                message.IsForCustomer = true;
                _context.Add(message);
                await _context.SaveChangesAsync();
                CustomerMessage customerMessage = new CustomerMessage();
                customerMessage.CustomerId = customerPrize.CustomerId;
                customerMessage.IsRead = false;
                customerMessage.MessageId = message.Id;
                customerMessage.SendDate = DateTime.Now;
                _context.Add(customerMessage);
                await _context.SaveChangesAsync();
                try
                {
                //    await SendNotification(message, customerPrize.Customer.FCMToken);
                }
                catch (Exception e) { }
                Customer customer = await _context.Customers.FindAsync(customerPrize.CustomerId);
                customer.CurrentPoints = customer.CurrentPoints + customerPrize.Prize.Points;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("PendingIndex");

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(customerPrize);
            }
        }

        private bool CustomerPrizeExists(int id)
        {
            return _context.CustomerPrizes.Any(e => e.Id == id);
        }
    }
}
