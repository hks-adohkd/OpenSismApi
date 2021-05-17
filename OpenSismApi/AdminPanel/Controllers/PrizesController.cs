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

                /*
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
                }*/
                
                {
                    var tableData = (from temp in _context.Prizes.Where(p => p.IsDeleted == isArchive )
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

       

    }


}
