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
    public class AppTaskGroupsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public AppTaskGroupsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: AppTaskGroups
        public IActionResult Index(int id)
        {
            ViewBag.AppTaskId = id;
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

                var tableData = (from temp in _context.AppTaskGroups
                                 .Include(a => a.Group).Where(a => a.AppTaskId == id)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.AppTask.DisplayName.Contains(searchValue) ||
                    m.Group.DisplayName.Contains(searchValue) || m.AppTask.DisplayNameAr.Contains(searchValue) ||
                    m.Group.DisplayNameAr.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<AppTaskGroupViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: AppTaskGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTaskGroup = await _context.AppTaskGroups
                .Include(a => a.AppTask)
                .Include(a => a.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appTaskGroup == null)
            {
                return NotFound();
            }
            ViewBag.AppTaskId = appTaskGroup.AppTaskId;
            return View(appTaskGroup);
        }

        // GET: AppTaskGroups/Create
        public IActionResult Create(int appTaskId)
        {
            ViewBag.AppTaskId = appTaskId;
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName");
            return View();
        }

        // POST: AppTaskGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppTaskGroup appTaskGroup)
        {
            try
            {
                _context.Add(appTaskGroup);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = appTaskGroup.AppTaskId });
            }
            catch (Exception e)
            {
                ViewBag.AppTaskId = appTaskGroup.AppTaskId;
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", appTaskGroup.GroupId);
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(appTaskGroup);
            }
        }

        // GET: AppTaskGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTaskGroup = await _context.AppTaskGroups.FindAsync(id);
            if (appTaskGroup == null)
            {
                return NotFound();
            }
            ViewBag.AppTaskId = appTaskGroup.AppTaskId;
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", appTaskGroup.GroupId);
            return View(appTaskGroup);
        }

        // POST: AppTaskGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppTaskGroup appTaskGroup)
        {
            if (id != appTaskGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appTaskGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppTaskGroupExists(appTaskGroup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = appTaskGroup.AppTaskId });
            }
            ViewBag.AppTaskId = appTaskGroup.AppTaskId;
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "DisplayName", appTaskGroup.GroupId);
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(appTaskGroup);
        }

        // GET: AppTaskGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTaskGroup = await _context.AppTaskGroups
                .Include(a => a.AppTask)
                .Include(a => a.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appTaskGroup == null)
            {
                return NotFound();
            }
            ViewBag.AppTaskId = appTaskGroup.AppTaskId;
            return View(appTaskGroup);
        }

        // POST: AppTaskGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appTaskGroup = await _context.AppTaskGroups.FindAsync(id);
            try
            {
                int appTaskId = appTaskGroup.AppTaskId;
                _context.AppTaskGroups.Remove(appTaskGroup);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = appTaskId });
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(appTaskGroup);
            }
        }

        private bool AppTaskGroupExists(int id)
        {
            return _context.AppTaskGroups.Any(e => e.Id == id);
        }
    }
}
