using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
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
    public class PrizeTypesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public PrizeTypesController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: PrizeTypes
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

                var tableData = (from temp in _context.PrizeTypes.Where(c => c.IsDeleted == isArchive)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.DisplayName.Contains(searchValue) ||
                    m.DisplayNameAr.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<PrizeTypeViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: PrizeTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prizeType = await _context.PrizeTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prizeType == null)
            {
                return NotFound();
            }

            return View(prizeType);
        }

        // GET: PrizeTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrizeTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrizeType prizeType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prizeType);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(prizeType);
        }

        // GET: PrizeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prizeType = await _context.PrizeTypes.FindAsync(id);
            if (prizeType == null)
            {
                return NotFound();
            }
            return View(prizeType);
        }

        // POST: PrizeTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrizeType prizeType)
        {
            if (id != prizeType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prizeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrizeTypeExists(prizeType.Id))
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
            return View(prizeType);
        }

        // GET: PrizeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prizeType = await _context.PrizeTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prizeType == null)
            {
                return NotFound();
            }

            return View(prizeType);
        }

        // POST: PrizeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prizeType = await _context.PrizeTypes.FindAsync(id);
            try
            {
                if (prizeType.Prizes.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    prizeType.IsDeleted = true;
                    _context.Update(prizeType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    _context.PrizeTypes.Remove(prizeType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(prizeType);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prizeType = await _context.PrizeTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prizeType == null)
            {
                return NotFound();
            }

            return View(prizeType);
        }

        // POST: PrizeTypes/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var prizeType = await _context.PrizeTypes.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                prizeType.IsDeleted = false;
                _context.Update(prizeType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(prizeType);
            }
        }
        private bool PrizeTypeExists(int id)
        {
            return _context.PrizeTypes.Any(e => e.Id == id);
        }
    }
}
