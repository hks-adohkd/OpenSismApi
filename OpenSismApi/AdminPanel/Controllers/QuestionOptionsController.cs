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
    public class QuestionOptionsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public QuestionOptionsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: QuestionOptions
        public IActionResult Index(int id, int? IsQuiz)
        {
            ViewBag.QuestionId = id;
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
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

                var tableData = (from temp in _context.QuestionOptions
                                 .Include(q => q.Question)
                                 .Where(a => a.QuestionId == id && !a.IsDeleted)
                                 select new
                                 {
                                     Id = temp.Id,
                                     ItemOrder = temp.ItemOrder,
                                     QuestionScript = temp.Question.Script,
                                     Script = temp.Script
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.QuestionScript.Contains(searchValue) ||
                    m.Script.Contains(searchValue));
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

        // GET: QuestionOptions/Details/5
        public async Task<IActionResult> Details(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionOption = await _context.QuestionOptions
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionOption == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewBag.QuestionId = questionOption.QuestionId;
            return View(questionOption);
        }

        // GET: QuestionOptions/Create
        public IActionResult Create(int questionId, int? IsQuiz)
        {
            ViewBag.QuestionId = questionId;
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            return View();
        }

        // POST: QuestionOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionOption questionOption, int? IsQuiz)
        {
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            try
            {
                _context.Add(questionOption);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = questionOption.QuestionId, IsQuiz = IsQuiz });
            }
            catch (Exception e)
            {
                ViewBag.QuestionId = questionOption.QuestionId;
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(questionOption);
            }
        }

        // GET: QuestionOptions/Edit/5
        public async Task<IActionResult> Edit(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionOption = await _context.QuestionOptions.FindAsync(id);
            if (questionOption == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewData["QuestionId"] = questionOption.QuestionId;
            return View(questionOption);
        }

        // POST: QuestionOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuestionOption questionOption, int? IsQuiz)
        {
            if (id != questionOption.Id)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionOptionExists(questionOption.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = questionOption.QuestionId, IsQuiz = IsQuiz });
            }
            ViewBag.QuestionId = questionOption.QuestionId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(questionOption);
        }

        // GET: QuestionOptions/Delete/5
        public async Task<IActionResult> Delete(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionOption = await _context.QuestionOptions
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionOption == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewBag.QuestionId = questionOption.QuestionId;
            return View(questionOption);
        }

        // POST: QuestionOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? IsQuiz)
        {
            var questionOption = await _context.QuestionOptions.FindAsync(id);
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            try
            {
                int questionId = questionOption.QuestionId;
                if (questionOption.CustomerAnswers.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    questionOption.IsDeleted = true;
                    _context.Update(questionOption);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = questionId, IsQuiz = IsQuiz });
                }
                else
                {
                    _context.QuestionOptions.Remove(questionOption);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { id = questionId, IsQuiz = IsQuiz });
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(questionOption);
            }
        }

        private bool QuestionOptionExists(int id)
        {
            return _context.QuestionOptions.Any(e => e.Id == id);
        }
    }
}
