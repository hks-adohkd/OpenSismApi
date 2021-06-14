using DBContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class SportMatchesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SportMatchesController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: AppTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportMatch = await _context.SportMatches
                .Include(a => a.AppTask)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportMatch == null)
            {
                return NotFound();
            }
            ViewBag.AppTaskId = sportMatch.AppTaskId;
            return View(sportMatch);
        }

        // GET: AppTasks/Create
        public IActionResult Create(int appTaskId)
        {
            ViewBag.AppTaskId = appTaskId;
            return View();
        }

        // POST: AppTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SportMatch sportMatch)
        {
            if (ModelState.IsValid)
            {
                if (sportMatch.FirstFile != null)
                {
                    FileInfo fi = new FileInfo(sportMatch.FirstFile.FileName);
                    var newFilename = "P" + sportMatch.Id + "_1" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                    var pathToSave = @"/images/tasks/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        sportMatch.FirstFile.CopyTo(stream);
                    }
                    sportMatch.FirstTeamFlag = pathToSave;
                }
                else
                {
                    sportMatch.FirstTeamFlag = "/images/tasks/task_icon.png";
                }
                if (sportMatch.SecondFile  != null)
                {
                    FileInfo fi = new FileInfo(sportMatch.SecondFile.FileName);
                    var newFilename = "P" + sportMatch.Id + "_2" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                    var pathToSave = @"/images/tasks/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        sportMatch.SecondFile.CopyTo(stream);
                    }
                    sportMatch.SecondTeamFlag = pathToSave;
                }
                else
                {
                    sportMatch.SecondTeamFlag = "/images/tasks/task_icon.png";
                }
                _context.Add(sportMatch);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), "AppTasks");
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            ViewBag.AppTaskId = sportMatch.AppTaskId;
            return View(sportMatch);
        }

        // GET: AppTasks/Edit/5
        public async Task<IActionResult> Edit(int? appTaskId)
        {
            if (appTaskId == null)
            {
                return NotFound();
            }

            var sportMatch = await _context.SportMatches.Where(sm => sm.AppTaskId == appTaskId).FirstOrDefaultAsync();
            if (sportMatch == null)
            {
                SportMatch match = new SportMatch();
                match.AppTaskId = (int)appTaskId;
                _context.Add(match);
                await _context.SaveChangesAsync();
                return View(match);
            }
            ViewData["AppTaskId"] = sportMatch.AppTaskId;
            return View(sportMatch);
        }

        // POST: AppTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SportMatch sportMatch)
        {
            if (id != sportMatch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (sportMatch.FirstFile != null)
                    {
                        FileInfo fi = new FileInfo(sportMatch.FirstFile.FileName);
                        var newFilename = "P" + sportMatch.Id + "_1" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                        var pathToSave = @"/images/tasks/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            sportMatch.FirstFile.CopyTo(stream);
                        }
                        sportMatch.FirstTeamFlag = pathToSave;
                    }
                    if (sportMatch.SecondFile != null)
                    {
                        FileInfo fi = new FileInfo(sportMatch.SecondFile.FileName);
                        var newFilename = "P" + sportMatch.Id + "_2" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\tasks\" + newFilename);
                        var pathToSave = @"/images/tasks/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            sportMatch.SecondFile.CopyTo(stream);
                        }
                        sportMatch.SecondTeamFlag = pathToSave;
                    }
                    _context.SportMatches.Update(sportMatch);
                    var res = await _context.SaveChangesAsync();

                    List<CustomerTask> customerTasks = _context.CustomerTasks.Where(ct => ct.AppTaskId == sportMatch.AppTaskId).ToList();
                    int i = 0;
                    foreach (var item in customerTasks)
                    {

                        
                        Customer customer = await _context.Customers.FindAsync(item.CustomerId);
                        if(customer != null)
                        {
                            CustomerPrediction customerPrediction = _context.CustomerPredictions.Where(cp => cp.CustomerId == customer.Id && cp.SportMatchId == sportMatch.Id).FirstOrDefault();
                            if(customerPrediction != null)
                            {
                                int gainedPoints = 0;
                                if(customerPrediction.FirstTeamScore == sportMatch.FirstTeamScore && customerPrediction.SecondTeamScore == sportMatch.SecondTeamScore)
                                {
                                    gainedPoints = item.AppTask.Points;
                                }
                                else if((customerPrediction.FirstTeamScore > customerPrediction.SecondTeamScore && sportMatch.FirstTeamScore > sportMatch.SecondTeamScore) ||
                                    (customerPrediction.FirstTeamScore < customerPrediction.SecondTeamScore && sportMatch.FirstTeamScore < sportMatch.SecondTeamScore) ||
                                    (customerPrediction.FirstTeamScore == customerPrediction.SecondTeamScore && sportMatch.FirstTeamScore == sportMatch.SecondTeamScore))
                                {
                                    gainedPoints = item.AppTask.Points / 2;
                                }
                                customer.CurrentPoints = customer.CurrentPoints + gainedPoints;

                                customer.TotalPoints = customer.TotalPoints + gainedPoints;
                                customerTasks[i].IsDone = true;
                                item.EarnedPoints = gainedPoints;
                                _context.CustomerTasks.Update(item);
                                _context.SaveChanges();

                                int nextGroup = customer.Group.ItemOrder + 1;
                                Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                                if (group != null)
                                {
                                    if (customer.TotalPoints >= group.Points)
                                    {
                                        customer.GroupId = group.Id;
                                        customer.Group = group;
                                        int nextNextGroupOrder = nextGroup + 1;
                                        Group nextNextGroup = _context.Groups.Where(g => g.ItemOrder == nextNextGroupOrder).FirstOrDefault();
                                        if (nextNextGroup != null)
                                        {
                                            customer.NextGroupPoints = nextNextGroup.Points;
                                        }
                                        else
                                        {
                                            customer.NextGroupPoints = 0;
                                        }
                                    }
                                    else
                                    {
                                        customer.NextGroupPoints = group.Points;
                                    }
                                }
                                else
                                {
                                    customer.NextGroupPoints = 0;
                                }
                                _context.Customers.Update(customer);
                                await _context.SaveChangesAsync();
                            }
                        }
                        i++;
                    }
                }
                catch (Exception e)
                {
                    HttpContext.Session.SetString("FailedMsg", FailedMsg);
                    ViewBag.AppTaskId = sportMatch.AppTaskId;
                    return View(sportMatch);
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index), "AppTasks");
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            ViewBag.AppTaskId = sportMatch.AppTaskId;
            return View(sportMatch);
        }

        private bool AppTaskExists(int id)
        {
            return _context.AppTasks.Any(e => e.Id == id);
        }
    }
}
