using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public HomeController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetHome")]
        public Response<HomePageViewModel> GetHome()
        {
            Response<HomePageViewModel> response = new Response<HomePageViewModel>();
            try
            {
                
                HomePageViewModel homePageViewModel = new HomePageViewModel();
                var pics = _context.Contents.Where(p => p.Name == "slider" && !p.IsDeleted).OrderBy(p => p.ItemOrder).ToList();
                homePageViewModel.Slides = Mapper.Map<List<ContentViewModel>>(pics);
                var banners = _context.Contents.Where(p => p.Name == "banner" && !p.IsDeleted).OrderBy(p => p.ItemOrder).ToList();
                homePageViewModel.Banner = Mapper.Map<List<ContentViewModel>>(banners);
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                ////NotDeleted
                //var items = _context.AppTasks.Where(a => !a.IsDeleted)
                //    //within limit
                //    .Where(a => a.CustomerTasks.Count() < a.Limit)
                //    //customer group
                //    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                //    .Where(a => a.TaskType.Name != "share_games_app")
                //    .OrderByDescending(a => a.Modified)
                //    .ToList();
                ////within time
                //items = items.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks)
                //                        .ToList();
                //var finished = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id && c.IsDone
                //&& !c.IsDeleted).Select(c => c.AppTask).ToList();
                //homePageViewModel.Tasks = new List<AppTaskViewModel>();
                //homePageViewModel.FinishedTasks = new List<AppTaskViewModel>();
                //foreach (var at in items)
                //{
                //    bool isDone = false;
                //    foreach (var ct in finished)
                //    {
                //        if(at.Id == ct.Id)
                //        {
                //            isDone = true;
                //            homePageViewModel.FinishedTasks.Add(Mapper.Map<AppTaskViewModel>(at));
                //            finished.Remove(ct);
                //            break;
                //        }
                //    }
                //    if (!isDone)
                //    {
                //        homePageViewModel.Tasks.Add(Mapper.Map<AppTaskViewModel>(at));
                //    }
                //}
                //AppTask shareAppTask = _context.AppTasks.Where(a => !a.IsDeleted)
                //    .Where(a => a.CustomerTasks.Where(c => c.AppTask.TaskType.Name == "share_games_app").Count() < a.Limit)
                //    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                //    .Where(a => a.TaskType.Name == "share_games_app").FirstOrDefault();

                //if(shareAppTask != null)
                //{
                //    homePageViewModel.Tasks.Add(Mapper.Map<AppTaskViewModel>(shareAppTask));
                //}
                homePageViewModel.LuckyWheelValid = true;
                if (((DateTime)customer.LuckyWheelLastSpinDate).Date == DateTime.Today)
                {
                    homePageViewModel.LuckyWheelValid = false;
                }

                homePageViewModel.DailyBonusValid = true;
                if (((DateTime)customer.DailyBonusLastUseDate).Date == DateTime.Today)
                {
                    homePageViewModel.DailyBonusValid = false;
                }

                //var pendings = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id && !c.IsDone
                //&& !c.IsDeleted).Select(c => c.AppTask).ToList();
                //homePageViewModel.PendingTasks = Mapper.Map<List<AppTaskViewModel>>(pendings);

                int nextGroup = customer.Group.ItemOrder + 1;
                Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                if (group != null)
                {
                    customer.NextGroupPoints = group.Points;
                }
                else
                {
                    customer.NextGroupPoints = 0;
                }
                //homePageViewModel.NewMessages = _context.CustomerMessages.Where(c => c.CustomerId == customer.Id
                //&& !c.IsRead && !c.IsDeleted).Count();

                response = APIContants<HomePageViewModel>.CostumSuccessResult(homePageViewModel,customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<HomePageViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
