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
    public class DailyBonusesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public DailyBonusesController(OpenSismDBContext context) : base(context)
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

                var tableData = (from temp in _context.DailyBonuses
                                 .Where(c => c.IsDeleted == isArchive)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<DailyBonusViewModel>>(data)
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

            var dailyBonus = await _context.DailyBonuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyBonus == null)
            {
                return NotFound();
            }

            return View(dailyBonus);
        }

        // GET: LuckyWheels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LuckyWheels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyBonus dailyBonus)
        {
            try
            {
                _context.Add(dailyBonus);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(dailyBonus);
            }
        }

        // GET: LuckyWheels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyBonus = await _context.DailyBonuses.FindAsync(id);
            if (dailyBonus == null)
            {
                return NotFound();
            }
            return View(dailyBonus);
        }

        // POST: LuckyWheels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DailyBonus dailyBonus)
        {
            if (id != dailyBonus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyBonus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyBonusExists(dailyBonus.Id))
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
            return View(dailyBonus);
        }

        // GET: LuckyWheels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyBonus = await _context.DailyBonuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyBonus == null)
            {
                return NotFound();
            }

            return View(dailyBonus);
        }

        // POST: LuckyWheels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyBonus = await _context.DailyBonuses.FindAsync(id);
            try
            {
                if (dailyBonus.Prizes.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    dailyBonus.IsDeleted = true;
                    _context.Update(dailyBonus);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.DailyBonuses.Remove(dailyBonus);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(dailyBonus);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyBonus = await _context.DailyBonuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyBonus == null)
            {
                return NotFound();
            }

            return View(dailyBonus);
        }

        // POST: LuckyWheels/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var dailyBonus = await _context.DailyBonuses.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                dailyBonus.IsDeleted = false;
                _context.Update(dailyBonus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(dailyBonus);
            }
        }

        private bool DailyBonusExists(int id)
        {
            return _context.DailyBonuses.Any(e => e.Id == id);
        }
    }
}
