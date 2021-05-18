using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
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
    public class PrizesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PrizesController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Prizes
        public IActionResult Index(int? luckyWheelId, bool? isArchive, int? dailyBonusId)
        {
            if (luckyWheelId != null && luckyWheelId != 0)
            {
                ViewBag.LuckyWheelId = luckyWheelId;
                LuckyWheel luckyWheel = _context.LuckyWheels.Find(luckyWheelId);
                if (luckyWheel.Prizes.Count() < luckyWheel.PartsCount)
                {
                    ViewBag.Closed = false;
                }
                else
                {
                    ViewBag.Closed = true;
                }
            }
            if (dailyBonusId != null && dailyBonusId != 0)
            {
                ViewBag.DailyBonusId = dailyBonusId;
                DailyBonus dailyBonus = _context.DailyBonuses.Find(dailyBonusId);
                if (dailyBonus.Prizes.Count() < dailyBonus.PartsCount)
                {
                    ViewBag.Closed = false;
                }
                else
                {
                    ViewBag.Closed = true;
                }
            }
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
        public IActionResult IndexPost(bool isArchive, int? luckyWheelId, int? dailyBonusId)
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

                if (luckyWheelId != null && luckyWheelId != 0)
                {
                    var tableData = (from temp in _context.Prizes.Where(p => p.IsDeleted == isArchive && p.LuckyWheelId == luckyWheelId)
                                     select new
                                     {
                                         DisplayName = temp.DisplayName,
                                         DisplayNameAr = temp.DisplayNameAr,
                                         Created = temp.Created,
                                         ImageUrl = temp.ImageUrl,
                                         ItemOrder = temp.ItemOrder,
                                         Id = temp.Id,
                                         CustmersCount = temp.CustomerPrizes.Count(),
                                     });
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                    {
                        tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        tableData = tableData.Where(m => m.DisplayNameAr.Contains(searchValue) || m.DisplayName.Contains(searchValue));
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
                else if (dailyBonusId != null && dailyBonusId != 0)
                {
                    var tableData = (from temp in _context.Prizes.Where(p => p.IsDeleted == isArchive && p.DailyBonusId == dailyBonusId)
                                     select new
                                     {
                                         DisplayName = temp.DisplayName,
                                         DisplayNameAr = temp.DisplayNameAr,
                                         Created = temp.Created,
                                         ImageUrl = temp.ImageUrl,
                                         Id = temp.Id,
                                         ItemOrder = temp.ItemOrder,
                                         CustmersCount = temp.CustomerPrizes.Count(),
                                     });
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                    {
                        tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        tableData = tableData.Where(m => m.DisplayNameAr.Contains(searchValue) || m.DisplayName.Contains(searchValue));
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
                else
                {
                    var tableData = (from temp in _context.Prizes.Where(p => p.IsDeleted == isArchive && p.LuckyWheelId == null
                                     && p.DailyBonusId == null)
                                     select new
                                     {
                                         DisplayName = temp.DisplayName,
                                         DisplayNameAr = temp.DisplayNameAr,
                                         Created = temp.Created,
                                         ImageUrl = temp.ImageUrl,
                                         ItemOrder = temp.ItemOrder,
                                         Id = temp.Id,
                                         CustmersCount = temp.CustomerPrizes.Count(),
                                     });
                    if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
                    {
                        tableData = tableData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        tableData = tableData.Where(m => m.DisplayNameAr.Contains(searchValue) || m.DisplayName.Contains(searchValue));
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
            }
            catch (Exception e)
            {
                throw;
            }
        }

        // GET: Prizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prizes
                .Include(p => p.PrizeType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prize == null)
            {
                return NotFound();
            }

            return View(prize);
        }

        // GET: Prizes/Create
        public IActionResult Create(int? luckyWheelId, int? dailyBonusId)
        {
            ViewData["PrizeTypeId"] = new SelectList(_context.PrizeTypes, "Id", "DisplayName");
            if (luckyWheelId != null && luckyWheelId != 0)
            {
                ViewBag.LuckyWheelId = luckyWheelId;
                return View();
            }
            if (dailyBonusId != null && dailyBonusId != 0)
            {
                ViewBag.DailyBonusId = dailyBonusId;
                return View();
            }
            return View();
        }

        // POST: Prizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prize prize)
        {
            if (ModelState.IsValid)
            {
                if (prize.file != null)
                {
                    FileInfo fi = new FileInfo(prize.file.FileName);
                    var newFilename = "P" + prize.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\prizes\" + newFilename);
                    var pathToSave = @"/images/prizes/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        prize.file.CopyTo(stream);
                    }
                    prize.ImageUrl = pathToSave;
                }
                else
                {
                    prize.ImageUrl = "/images/prizes/prize_icon.png";
                }
                if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
                {
                    //prize.PrizeTypeId = _context.PrizeTypes.Where(p => p.Name == "lucky_wheel").FirstOrDefault().Id;
                    prize.Name = "lucky_wheel";
                    prize.Points = 0;
                    _context.Add(prize);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { luckyWheelId = prize.LuckyWheelId });
                }
                if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
                {
                    prize.Name = "daily_bonus";
                    prize.Points = 0;
                    _context.Add(prize);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    return RedirectToAction(nameof(Index), new { dailyBonusId = prize.DailyBonusId });
                }
                _context.Add(prize);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            var message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
         //   return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);

            HttpContext.Session.SetString("FailedMsg", message);
            ViewData["PrizeTypeId"] = new SelectList(_context.PrizeTypes, "Id", "DisplayName", prize.PrizeTypeId);
            return View(prize);
        }

        // GET: Prizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prizes.FindAsync(id);
            if (prize == null)
            {
                return NotFound();
            }
            ViewData["PrizeTypeId"] = new SelectList(_context.PrizeTypes, "Id", "DisplayName", prize.PrizeTypeId);
            if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
            {
                ViewBag.LuckyWheelId = prize.LuckyWheelId;
            }
            if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
            {
                ViewBag.DailyBonusId = prize.DailyBonusId;
            }
            return View(prize);
        }

        // POST: Prizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prize prize)
        {
            if (id != prize.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (prize.file != null)
                    {
                        FileInfo fi = new FileInfo(prize.file.FileName);
                        var newFilename = "P" + prize.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\prizes\" + newFilename);
                        var pathToSave = @"/images/prizes/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            prize.file.CopyTo(stream);
                        }
                        prize.ImageUrl = pathToSave;
                    }
                    if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
                    {
                        //prize.PrizeTypeId = _context.PrizeTypes.Where(p => p.Name == "lucky_wheel").FirstOrDefault().Id;
                        prize.Name = "lucky_wheel";
                        prize.Points = 0;
                        _context.Update(prize);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                        return RedirectToAction(nameof(Index), new { luckyWheelId = prize.LuckyWheelId });
                    }
                    if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
                    {
                        prize.Name = "daily_bonus";
                        prize.Points = 0;
                        _context.Update(prize);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                        return RedirectToAction(nameof(Index), new { dailyBonusId = prize.DailyBonusId });
                    }
                    _context.Update(prize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrizeExists(prize.Id))
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
            ViewData["PrizeTypeId"] = new SelectList(_context.PrizeTypes.Where(p => p.Name != "points"), "Id", "DisplayName", prize.PrizeTypeId);
            if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
            {
                ViewBag.LuckyWheelId = prize.LuckyWheelId;
            }
            if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
            {
                ViewBag.DailyBonusId = prize.DailyBonusId;
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(prize);
        }

        // GET: Prizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prizes
                .Include(p => p.PrizeType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prize == null)
            {
                return NotFound();
            }

            return View(prize);
        }

        // POST: Prizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prize = await _context.Prizes.FindAsync(id);
            try
            {
                if (prize.CustomerPrizes.Count() > 0)
                {
                    HttpContext.Session.SetString("SuccessMsg", "Item Archived");
                    prize.IsDeleted = true;
                    _context.Update(prize);
                    await _context.SaveChangesAsync();
                    if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
                    {
                        return RedirectToAction(nameof(Index), new { luckyWheelId = prize.LuckyWheelId });
                    }
                    if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
                    {
                        return RedirectToAction(nameof(Index), new { dailyBonusId = prize.DailyBonusId });
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.Prizes.Remove(prize);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                    if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
                    {
                        return RedirectToAction(nameof(Index), new { luckyWheelId = prize.LuckyWheelId });
                    }
                    if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
                    {
                        return RedirectToAction(nameof(Index), new { dailyBonusId = prize.DailyBonusId });
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(prize);
            }
        }

        public async Task<IActionResult> Recover(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prize = await _context.Prizes
                .Include(p => p.PrizeType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prize == null)
            {
                return NotFound();
            }

            return View(prize);
        }

        // POST: Prizes/Delete/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverConfirmed(int id)
        {
            var prize = await _context.Prizes.FindAsync(id);
            try
            {
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                prize.IsDeleted = false;
                _context.Update(prize);
                await _context.SaveChangesAsync();
                if (prize.LuckyWheelId != null && prize.LuckyWheelId != 0)
                {
                    return RedirectToAction(nameof(Index), new { luckyWheelId = prize.LuckyWheelId });
                }
                if (prize.DailyBonusId != null && prize.DailyBonusId != 0)
                {
                    return RedirectToAction(nameof(Index), new { dailyBonusId = prize.DailyBonusId });
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(prize);
            }
        }

        private bool PrizeExists(int id)
        {
            return _context.Prizes.Any(e => e.Id == id);
        }
    }
}
