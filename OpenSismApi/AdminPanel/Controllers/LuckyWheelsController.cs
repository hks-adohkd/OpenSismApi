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
    public class LuckyWheelsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public LuckyWheelsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: LuckyWheels
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

                var tableData = (from temp in _context.LuckyWheels.Include(l => l.Group)
                                 .Where(c => c.IsDeleted == isArchive)
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
                    data = Mapper.Map<List<LuckyWheelViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: LuckyWheels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luckyWheel = await _context.LuckyWheels
                .Include(l => l.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (luckyWheel == null)
            {
                return NotFound();
            }

            return View(luckyWheel);
        }

        // GET: LuckyWheels/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName");
            return View();
        }

        // POST: LuckyWheels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LuckyWheel luckyWheel)
        {
            try
            {
                _context.Add(luckyWheel);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", luckyWheel.GroupId);
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(luckyWheel);
            }
        }

        // GET: LuckyWheels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luckyWheel = await _context.LuckyWheels.FindAsync(id);
            if (luckyWheel == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", luckyWheel.GroupId);
            return View(luckyWheel);
        }

        // POST: LuckyWheels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LuckyWheel luckyWheel)
        {
            if (id != luckyWheel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luckyWheel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuckyWheelExists(luckyWheel.Id))
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
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", luckyWheel.GroupId);
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(luckyWheel);
        }

        // GET: LuckyWheels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luckyWheel = await _context.LuckyWheels
                .Include(l => l.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (luckyWheel == null)
            {
                return NotFound();
            }

            return View(luckyWheel);
        }

        // POST: LuckyWheels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luckyWheel = await _context.LuckyWheels.FindAsync(id);
            try
            {
                if (luckyWheel.Prizes.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    luckyWheel.IsDeleted = true;
                    _context.Update(luckyWheel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.LuckyWheels.Remove(luckyWheel);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(luckyWheel);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luckyWheel = await _context.LuckyWheels
                .Include(l => l.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (luckyWheel == null)
            {
                return NotFound();
            }

            return View(luckyWheel);
        }

        // POST: LuckyWheels/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var luckyWheel = await _context.LuckyWheels.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                luckyWheel.IsDeleted = false;
                _context.Update(luckyWheel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(luckyWheel);
            }
        }

        private bool LuckyWheelExists(int id)
        {
            return _context.LuckyWheels.Any(e => e.Id == id);
        }
    }
}
