using DBContext.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class CustomerTasksController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public CustomerTasksController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: CustomerTasks
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

                var tableData = (from temp in _context.CustomerTasks.Where(c => c.CustomerId == id)
                                 select new
                                 {
                                     Id = temp.Id,
                                     DisplayName = temp.AppTask.DisplayName,
                                     DoneDate = temp.DoneDate,
                                     CustomerId = temp.CustomerId,
                                     AppTaskId = temp.AppTaskId,
                                     TaskTypeName = temp.AppTask.TaskType.Name
                                 });

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

        // GET: CustomerTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerTask = await _context.CustomerTasks
                .Include(c => c.AppTask)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerTask == null)
            {
                return NotFound();
            }
            ViewBag.CustomerId = customerTask.CustomerId;
            return View(customerTask);
        }



        private bool CustomerTaskExists(int id)
        {
            return _context.CustomerTasks.Any(e => e.Id == id);
        }
    }
}
