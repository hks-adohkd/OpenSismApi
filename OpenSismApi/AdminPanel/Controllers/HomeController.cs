using AdminPanel.Models;
using DBContext.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace AdminPanel.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OpenSismDBContext _context;

        public HomeController(OpenSismDBContext myGamesDBContext, ILogger<HomeController> logger) : base(myGamesDBContext)
        {
            _logger = logger;
            _context = myGamesDBContext;
        }

        public IActionResult Index()
        {
            var customers = _context.Customers.Where(a => !a.IsDeleted).ToList();
            ViewBag.newCustomers = customers.Where(a => a.Created.Ticks <= DateTime.Now.Ticks && a.Created.Ticks >= DateTime.Now.AddDays(-7).Ticks).Count();
            PrizeStatus prizeStatus = _context.PrizeStatuses.Where(p => p.Name == "requested").FirstOrDefault();
            ViewBag.pendingPrizes = _context.CustomerPrizes.Include(p => p.PrizeStatus).Where(c => c.PrizeStatusId == prizeStatus.Id).Count();
            var tasks = _context.AppTasks.ToList();
            ViewBag.activeTasks = tasks.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks).Count();
            ViewBag.contactUs = _context.ContactsUs.Where(c => !c.IsViewed).Count();

            ViewBag.AllCustomers = _context.Customers.Count();
            ViewBag.AllDoneTasks = _context.CustomerTasks.Where(c => c.IsDone).Count();
            ViewBag.MaxTaskDone = _context.AppTasks.OrderByDescending(a => a.CustomerTasks.Where(c => c.IsDone).Count()).FirstOrDefault().DisplayName;
            ViewBag.MaxRequestedPrize = _context.Prizes.OrderByDescending(a => a.CustomerPrizes.Count()).FirstOrDefault().DisplayName;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
