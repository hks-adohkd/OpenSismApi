using DBContext.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace AdminPanel.Controllers
{
    public class UserLogsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public UserLogsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: UserLogs
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IndexPost()
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

                var tableData = (from temp in _context.UserLogs
                                 select new UserLog
                                 {
                                     Id = temp.Id,
                                     Platform = temp.Platform,
                                     Action = temp.Action,
                                     Status = temp.Status
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Platform.Contains(searchValue) ||
                    m.Status.Contains(searchValue));
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

        // GET: UserLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLog = await _context.UserLogs
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLog == null)
            {
                return NotFound();
            }

            return View(userLog);
        }

        // GET: UserLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLog = await _context.UserLogs
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLog == null)
            {
                return NotFound();
            }

            return View(userLog);
        }

        // POST: UserLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userLog = await _context.UserLogs.FindAsync(id);
            try
            {
                _context.UserLogs.Remove(userLog);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(userLog);
            }
        }

        public IActionResult DeleteAll()
        {
            return View();
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllConfirmed(int id)
        {
            var logs = await _context.UserLogs.ToListAsync();
            try
            {
                _context.UserLogs.RemoveRange(logs);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View();
            }
        }

        private bool UserLogExists(int id)
        {
            return _context.UserLogs.Any(e => e.Id == id);
        }
    }
}
