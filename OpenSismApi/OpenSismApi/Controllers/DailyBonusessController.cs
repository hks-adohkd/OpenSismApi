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
using System.Linq;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyBonusessController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public DailyBonusessController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/LuckyWheels
        [HttpPost]
        [Route("Get")]
        public Response<DailyBonusViewModel> Get([FromBody] DailyBonusViewModel model)
        {
            Response<DailyBonusViewModel> response = new Response<DailyBonusViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var item = _context.DailyBonuses.Where(a => !a.IsDeleted)
                    .FirstOrDefault();
                
                if (item == null)
                {
                    response = APIContants<DailyBonusViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }

                item.Prizes = _context.Prizes.Where(p => p.DailyBonusId == item.Id).OrderBy(p => p.ItemOrder).ToList();
                var dailyBonus = Mapper.Map<DailyBonusViewModel>(item);
                dailyBonus.IsDoneToday = false;
                if (((DateTime)customer.DailyBonusLastUseDate).Date == DateTime.Today)
                {
                    dailyBonus.IsDoneToday = true;
                }
                dailyBonus.IsDoneYesterday = false;
                if (!dailyBonus.IsDoneToday)
                {
                    if (((DateTime)customer.DailyBonusLastUseDate).Date == DateTime.Today.AddDays(-1))
                    {
                        dailyBonus.IsDoneYesterday = true;
                    }
                    if (!dailyBonus.IsDoneYesterday)
                    {
                        customer.DailyBonusLevel = 1;
                        _context.Update(customer);
                        _context.SaveChanges();
                    }
                }
                response = APIContants<DailyBonusViewModel>.CostumSuccessResult(dailyBonus, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<DailyBonusViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
