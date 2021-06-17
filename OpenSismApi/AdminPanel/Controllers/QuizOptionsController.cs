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
    public class QuizOptionsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public QuizOptionsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

       


        // GET: QuestionOptions
        public IActionResult Index(int id)
        {
            ViewBag.QuizId = id;
            
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

                var tableData = (from temp in _context.QuizOptions
                                 .Include(q => q.Quiz)
                                 .Where(a => a.QuizId == id && !a.IsDeleted)
                                 select new
                                 {
                                     Id = temp.Id,
                                     ItemOrder = temp.ItemOrder,
                                    QuizScriptAr = temp.Quiz.ScriptAr,  
                                     Script = temp.ScriptAr
                                 });

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.QuizScriptAr.Contains(searchValue) ||
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizOption = await _context.QuizOptions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizOption == null)
            {
                return NotFound();
            }
           
            ViewBag.QuizId = quizOption.QuizId;
            return View(quizOption);
        }

        // GET: QuestionOptions/Create
        public IActionResult Create(int quizId)
        {
            ViewBag.QuizId = quizId;
            
            return View();
        }

        // POST: QuestionOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizOption quizOption)
        {
            
            try
            {
                _context.Add(quizOption);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = quizOption.QuizId });
            }
            catch (Exception e)
            {
                ViewBag.QuizId = quizOption.QuizId;
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(quizOption);
            }
        }

        // GET: QuestionOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizOption = await _context.QuizOptions.FindAsync(id);
            if (quizOption == null)
            {
                return NotFound();
            }
           
            ViewData["QuizId"] = quizOption.QuizId;
            return View(quizOption);
        }

        // POST: QuestionOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuizOption quizOptions)
        {
            if (id != quizOptions.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizOptions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizOptionExists(quizOptions.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = quizOptions.QuizId });
            }
            ViewBag.QuestionId = quizOptions.QuizId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(quizOptions);
        }

        // GET: QuestionOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quizOption = await _context.QuizOptions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizOption == null)
            {
                return NotFound();
            }

            ViewBag.QuizId = quizOption.QuizId;
            return View(quizOption);

        }
        // POST: QuestionOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quizOption = await _context.QuizOptions.FindAsync(id);
           
            try
            {
                int quizId = quizOption.QuizId;
                
               
                    _context.QuizOptions.Remove(quizOption);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { id = quizId });
               
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(quizOption);
            }
        }

        private bool QuizOptionExists(int id)
        {
            return _context.QuizOptions.Any(e => e.Id == id);
        }
    }
}
