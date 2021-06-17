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
    public class QuizController : BaseController
    {
        private readonly OpenSismDBContext _context;
            
        public QuizController(OpenSismDBContext context) : base(context)
        {
            _context = context;
        }

        


        // GET: Questions
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

                var tableData = (from temp in _context.Quizs.Where(a => a.AppTaskId == id && !a.IsDeleted)
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
                    data = Mapper.Map<List<QuizViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                //HttpContext.Session.SetString("FailedMsg", e.Message);
                throw;
            }
        }

        public IActionResult result(int id)    
        {

            ViewBag.AppTaskId = id;

            return View();
        }
            
        [HttpPost]  
        public IActionResult resultPost(int id)
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

                var tableData = (from temp in _context.Quizs.Where(a =>  !a.IsDeleted)
                                 select temp);

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                {
                    tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tableData = tableData.Where(m => m.AppTask.DisplayName.Contains(searchValue) ||
                    m.Script.Contains(searchValue) || m.ScriptAr.Contains(searchValue) || m.AppTaskId.ToString().Contains(searchValue));
                }
                recordsTotal = tableData.Count();
                var data = tableData.Skip(skip).Take(pageSize).ToList();
                var res = Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,

                    data = Mapper.Map<List<QuizViewModel>>(data)
                });
                return res;
            }
            catch (Exception e)
            {
                //HttpContext.Session.SetString("FailedMsg", e.Message);
                throw;
            }
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizs
                .Include(q => q.AppTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }
            
            ViewBag.AppTaskId = quiz.AppTaskId;
            return View(quiz);
        }

        // GET: Questions/Create
        public IActionResult Create(int appTaskId)
        {
            ViewBag.AppTaskId = appTaskId;
            
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz) 
        {
            
            quiz.Type = true;
            try
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = quiz.AppTaskId });
            }
            catch (Exception e)
            {

                ViewBag.AppTaskId = quiz.AppTaskId;
                HttpContext.Session.SetString("FailedMsg", e.ToString());
                return View(quiz);
            }
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizs.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            
            ViewData["AppTaskId"] = quiz.AppTaskId;
            return View(quiz);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), new { id = quiz.AppTaskId });
            }
            ViewBag.AppTaskId = quiz.AppTaskId;
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(quiz);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizs
                .Include(q => q.AppTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }
           
            ViewBag.AppTaskId = quiz.AppTaskId;
            return View(quiz);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizs.FindAsync(id);
           
            try
            {
                int appTaskId = quiz.AppTaskId;
                
                
                    _context.Quizs.Remove(quiz);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { id = appTaskId });
                
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(quiz);
            }
        }

        private bool QuizExists(int id)
        {
            return _context.Quizs.Any(e => e.Id == id);
        }
    }
}
