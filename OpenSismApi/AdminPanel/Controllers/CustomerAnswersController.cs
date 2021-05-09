using DBContext.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class CustomerAnswersController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public CustomerAnswersController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: CustomerAnswers
        public IActionResult Index(int CustomerId, int appTaskId)
        {
            ViewBag.CustomerId = CustomerId;
            ViewBag.AppTaskId = appTaskId;
            return View();
        }

        [HttpPost]
        public IActionResult IndexPost(int CustomerId, int appTaskId)
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

                var tableData = (from temp in _context.CustomerAnswers
                                 .Where(c => c.CustomerId == CustomerId && c.Question.AppTaskId == appTaskId)
                                 select new
                                 {
                                     Id = temp.Id,
                                     Script = temp.Question.Script
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.Script.Contains(searchValue));
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

        // GET: CustomerGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerAnswer = await _context.CustomerAnswers
                .Include(c => c.Customer)
                .Include(c => c.Question).Include(c => c.QuestionOption)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerAnswer == null)
            {
                return NotFound();
            }
            ViewBag.CustomerId = customerAnswer.CustomerId;
            ViewBag.AppTaskId = customerAnswer.Question.AppTaskId;
            return View(customerAnswer);
        }


        private bool CustomerAnswerExists(int id)
        {
            return _context.CustomerAnswers.Any(e => e.Id == id);
        }
    }
}
