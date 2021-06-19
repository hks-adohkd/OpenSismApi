using DBContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class AppTasksController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AppTasksController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: AppTasks
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

                var tableData = (from temp in _context.AppTasks.Where(a => a.IsDeleted == isArchive)
                                 select new
                                 {
                                     DisplayName = temp.DisplayName,
                                     Created = temp.Created,
                                     StartDate = temp.StartDate.ToString("MM/dd/yyyy hh:mm tt"),
                                     EndDate = temp.EndDate.ToString("MM/dd/yyyy hh:mm tt"),
                                     ImageUrl = temp.ImageUrl,
                                     TaskTypeName = temp.TaskType.Name,
                                     Id = temp.Id,
                                     CustmersCount = temp.CustomerTasks.Count(),
                                     Points = temp.Points,
                                     Limit = temp.Limit,
                                 }) ;

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.DisplayName.Contains(searchValue));
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

        // GET: AppTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTask = await _context.AppTasks
                .Include(a => a.TaskType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appTask == null)
            {
                return NotFound();
            }

            return View(appTask);
        }

        // GET: AppTasks/Create
        public IActionResult Create()
        {
            ViewData["TaskTypeId"] = new SelectList(_context.TaskTypes.Where(t => t.Name != "share_games_app"), "Id", "DisplayName");
            return View();
        }

        // POST: AppTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppTask appTask)
        {
            if (ModelState.IsValid)
            {
                if (appTask.file != null)
                {
                    FileInfo fi = new FileInfo(appTask.file.FileName);
                    var newFilename = "P" + appTask.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                    var pathToSave = @"/images/tasks/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        appTask.file.CopyTo(stream);
                    }
                    appTask.ImageUrl = pathToSave;
                }
                else
                {
                    appTask.ImageUrl = "/images/tasks/task_icon.png";
                }
                if (appTask.vedioFile  != null)
                {
                    FileInfo fi = new FileInfo(appTask.vedioFile.FileName);
                    var newFilename = "P" + appTask.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                    var pathToSave = @"/images/tasks/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        appTask.vedioFile.CopyTo(stream);
                    }
                    appTask.Link = pathToSave;
                }
                _context.Add(appTask);
                await _context.SaveChangesAsync();

                if (appTask.IsForAll)
                {
                    foreach (var item in _context.Groups.Where(g => !g.IsDeleted).ToList())
                    {
                        AppTaskGroup appTaskGroup = new AppTaskGroup();
                        appTaskGroup.AppTaskId = appTask.Id;
                        appTaskGroup.GroupId = item.Id;
                        _context.Add(appTaskGroup);
                        await _context.SaveChangesAsync();
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                appTask.TaskType = _context.TaskTypes.Find(appTask.TaskTypeId);
                if (appTask.TaskType.Name == "survey")
                {
                    return RedirectToAction("Index", "Questions", new { id = appTask.Id });
                }
                else if(appTask.TaskType.Name == "quiz")
                {
                    return RedirectToAction("Index", "Quiz", new { id = appTask.Id });
                }
                else if (appTask.TaskType.Name == "sport_match")
                {
                    return RedirectToAction(nameof(Create), "SportMatches", new { appTaskId = appTask.Id});
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            ViewData["TaskTypeId"] = new SelectList(_context.TaskTypes.Where(t => t.Name != "share_games_app"), "Id", "DisplayName", appTask.TaskTypeId);
            return View(appTask);
        }

        // GET: AppTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTask = await _context.AppTasks.FindAsync(id);
            if (appTask == null)
            {
                return NotFound();
            }
            ViewData["TaskTypeId"] = new SelectList(_context.TaskTypes.Where(t => t.Name != "share_games_app"), "Id", "DisplayName", appTask.TaskTypeId);
            return View(appTask);
        }

        // POST: AppTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppTask appTask)
        {
            if (id != appTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (appTask.file != null)
                    {
                        FileInfo fi = new FileInfo(appTask.file.FileName);
                        var newFilename = "P" + appTask.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                        var pathToSave = @"/images/tasks/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            appTask.file.CopyTo(stream);
                        }
                        appTask.ImageUrl = pathToSave;
                    }
                    if (appTask.vedioFile != null)
                    {
                        FileInfo fi = new FileInfo(appTask.vedioFile.FileName);
                        var newFilename = "P" + appTask.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                        var pathToSave = @"/images/tasks/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            appTask.vedioFile.CopyTo(stream);
                        }
                        appTask.Link = pathToSave;
                    }
                    _context.Update(appTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppTaskExists(appTask.Id))
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
            ViewData["TaskTypeId"] = new SelectList(_context.TaskTypes.Where(t => t.Name != "share_games_app"), "Id", "DisplayName", appTask.TaskTypeId);
            return View(appTask);
        }

        // GET: AppTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTask = await _context.AppTasks
                .Include(a => a.TaskType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appTask == null)
            {
                return NotFound();
            }

            return View(appTask);
        }

        // POST: AppTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appTask = await _context.AppTasks.FindAsync(id);
            try
            {
                if (appTask.CustomerTasks.Count() > 0 || appTask.AppTaskGroups.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    appTask.IsDeleted = true;
                    _context.Update(appTask);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (appTask.TaskType.Name == "sport_match" && appTask.SportMatch != null)
                    {
                        _context.SportMatches.Remove(appTask.SportMatch);
                        await _context.SaveChangesAsync();
                    }
                    _context.AppTasks.Remove(appTask);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(appTask);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appTask = await _context.AppTasks
                .Include(a => a.TaskType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appTask == null)
            {
                return NotFound();
            }

            return View(appTask);
        }

        // POST: AppTasks/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var appTask = await _context.AppTasks.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                appTask.IsDeleted = false;
                _context.Update(appTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(appTask);
            }
        }

        private bool AppTaskExists(int id)
        {
            return _context.AppTasks.Any(e => e.Id == id);
        }
    }
}
