using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class MessagesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private IHostingEnvironment env;

        public MessagesController(OpenSismDBContext context, IHostingEnvironment env) : base(context, env)
        {
            _context = context;
            this.env = env;
        }

        // GET: Messages
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

                var tableData = (from temp in _context.Messages.Where(c => c.IsDeleted == isArchive)
                                 .Where(m => !m.IsForCustomer)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Title.Contains(searchValue) ||
                    m.TitleAr.Contains(searchValue) || m.Script.Contains(searchValue) ||
                    m.ScriptAr.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<MessageViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DBContext.Models.Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                if (message.IsForAll)
                {
                    foreach (var item in _context.Groups.Where(g => !g.IsDeleted).ToList())
                    {
                        MessageGroup messageGroup = new MessageGroup();
                        messageGroup.MessageId = message.Id;
                        messageGroup.GroupId = item.Id;
                        _context.Add(messageGroup);
                        await _context.SaveChangesAsync();
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DBContext.Models.Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(message);
        }

        public async Task<IActionResult> Send(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.MessageGroups)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            try
            {
                if (message.MessageGroups.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    foreach (var item in message.MessageGroups.Select(m => m.Group).ToList())
                    {
                        foreach (var c in item.Customers.ToList())
                        {
                            CustomerMessage customerMessage = new CustomerMessage();
                            customerMessage.CustomerId = c.Id;
                            customerMessage.MessageId = message.Id;
                            customerMessage.IsRead = false;
                            customerMessage.SendDate = DateTime.Now;
                            _context.Add(customerMessage);
                            await _context.SaveChangesAsync();
                            try { await SendNotification(message, c.FCMToken); }
                            catch (Exception e)
                            { }
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    HttpContext.Session.SetString("FailedMsg", "You have to choose at least one group !!");
                    return View(message);
                }

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(message);
            }
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            try
            {
                if (message.MessageGroups.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    message.IsDeleted = true;
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(message);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                message.IsDeleted = false;
                _context.Update(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(message);
            }
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
