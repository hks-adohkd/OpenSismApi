using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class MessageGroupsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public MessageGroupsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: MessageGroups
        public IActionResult Index(int id)
        {
            ViewBag.MessageId = id;
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

                var tableData = (from temp in _context.MessageGroups.Where(m => m.MessageId == id)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Group.DisplayName.Contains(searchValue) ||
                    m.Group.DisplayNameAr.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<MessageGroupViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: MessageGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageGroup = await _context.MessageGroups
                .Include(m => m.Group)
                .Include(m => m.Message)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageGroup == null)
            {
                return NotFound();
            }
            ViewBag.MessageId = messageGroup.MessageId;
            return View(messageGroup);
        }

        // GET: MessageGroups/Create
        public IActionResult Create(int MessageId)
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName");
            ViewBag.MessageId = MessageId;
            return View();
        }

        // POST: MessageGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageGroup messageGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(messageGroup);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = messageGroup.MessageId });
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", messageGroup.GroupId);
            ViewBag.MessageId = messageGroup.MessageId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(messageGroup);
        }

        // GET: MessageGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageGroup = await _context.MessageGroups.FindAsync(id);
            if (messageGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", messageGroup.GroupId);
            ViewBag.MessageId = messageGroup.MessageId;
            return View(messageGroup);
        }

        // POST: MessageGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MessageGroup messageGroup)
        {
            if (id != messageGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageGroupExists(messageGroup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = messageGroup.MessageId });
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", messageGroup.GroupId);
            ViewBag.MessageId = messageGroup.MessageId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(messageGroup);
        }

        // GET: MessageGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageGroup = await _context.MessageGroups
                .Include(m => m.Group)
                .Include(m => m.Message)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageGroup == null)
            {
                return NotFound();
            }
            ViewBag.MessageId = messageGroup.MessageId;
            return View(messageGroup);
        }

        // POST: MessageGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageGroup = await _context.MessageGroups.FindAsync(id);
            try
            {
                int messageId = messageGroup.MessageId;
                _context.MessageGroups.Remove(messageGroup);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = messageId });
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(messageGroup);
            }
        }

        private bool MessageGroupExists(int id)
        {
            return _context.MessageGroups.Any(e => e.Id == id);
        }
    }
}
