using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class GroupsController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public GroupsController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Groups
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

                var tableData = (from temp in _context.Groups.Include(g => g.Customers).Where(c => c.IsDeleted == isArchive)
                                 select new
                                 {
                                     DisplayName = temp.DisplayName,
                                     DisplayNameAr = temp.DisplayNameAr,
                                     Name = temp.Name,
                                     Created = temp.Created,
                                     ItemOrder = temp.ItemOrder,
                                     Color = temp.Color,
                                     ImageUrl = temp.ImageUrl,
                                     Customers = temp.Customers.Count(),
                                     Id = temp.Id,
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Name.Contains(searchValue) ||
                    m.DisplayName.Contains(searchValue) || m.DisplayNameAr.Contains(searchValue));
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

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group @group)
        {
            if (ModelState.IsValid)
            {
                if (@group.file != null)
                {
                    FileInfo fi = new FileInfo(@group.file.FileName);
                    var newFilename = "P" + @group.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\groups\" + newFilename);
                    var pathToSave = @"/images/groups/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        @group.file.CopyTo(stream);
                    }
                    @group.ImageUrl = pathToSave;
                }
                else
                {
                    @group.ImageUrl = "/images/tasks/group_icon.png";
                }
                _context.Add(@group);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (@group.file != null)
                    {
                        FileInfo fi = new FileInfo(@group.file.FileName);
                        var newFilename = "P" + @group.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\groups\" + newFilename);
                        var pathToSave = @"/images/groups/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            @group.file.CopyTo(stream);
                        }
                        @group.ImageUrl = pathToSave;
                    }
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            try
            {
                if (@group.AppTaskGroups.Count() > 0 || @group.Customers.Count() > 0 || @group.MessageGroups.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    @group.IsDeleted = true;
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.Groups.Remove(@group);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(@group);
            }

        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                @group.IsDeleted = false;
                _context.Update(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(@group);
            }

        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
