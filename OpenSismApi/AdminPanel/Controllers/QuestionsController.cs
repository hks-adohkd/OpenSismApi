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
    public class QuestionsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public QuestionsController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        // GET: Questions
        public IActionResult Index(int id, int? IsQuiz)
        {
            ViewBag.AppTaskId = id;
            if(IsQuiz != null && IsQuiz == 1)
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

                var tableData = (from temp in _context.Questions.Where(a => a.AppTaskId == id && !a.IsDeleted)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.AppTask.DisplayName.Contains(searchValue) ||
                    m.Script.Contains(searchValue) || m.ScriptAr.Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = Mapper.Map<List<QuestionViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.AppTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewBag.AppTaskId = question.AppTaskId;
            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create(int appTaskId, int? IsQuiz)
        {
            ViewBag.AppTaskId = appTaskId;
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question, int? IsQuiz)
        {
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
                question.Type = true;
            }
            try
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = question.AppTaskId, IsQuiz = IsQuiz });
            }
            catch (Exception e)
            {
                ViewBag.AppTaskId = question.AppTaskId;
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(question);
            }
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewData["AppTaskId"] = question.AppTaskId;
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question, int? IsQuiz)
        {
            if (id != question.Id)
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
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = question.AppTaskId, IsQuiz = IsQuiz });
            }
            ViewBag.AppTaskId = question.AppTaskId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id, int? IsQuiz)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.AppTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            ViewBag.AppTaskId = question.AppTaskId;
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? IsQuiz)
        {
            var question = await _context.Questions.FindAsync(id);
            if (IsQuiz != null && IsQuiz == 1)
            {
                ViewBag.IsQuiz = IsQuiz;
            }
            try
            {
                int appTaskId = question.AppTaskId;
                if (question.CustomerAnswers.Count() > 0 || question.QuestionOptions.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    question.IsDeleted = true;
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = appTaskId, IsQuiz = IsQuiz });
                }
                else
                {
                    _context.Questions.Remove(question);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { id = appTaskId, IsQuiz = IsQuiz });
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(question);
            }
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
