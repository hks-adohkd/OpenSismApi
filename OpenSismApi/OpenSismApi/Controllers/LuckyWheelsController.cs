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
    public class LuckyWheelsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public LuckyWheelsController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/LuckyWheels
        [HttpPost]
        [Route("Get")]
        public Response<LuckyWheelViewModel> Get([FromBody] LuckyWheelViewModel model)
        {
            Response<LuckyWheelViewModel> response = new Response<LuckyWheelViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var item = _context.LuckyWheels.Where(a => a.GroupId == customer.GroupId)
                    .Where(a => !a.IsDeleted)
                    .FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<LuckyWheelViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
               
                item.Prizes = _context.Prizes.Where(p => p.LuckyWheelId == item.Id).OrderBy(p => p.ItemOrder).ToList();
                var luckyWheel = Mapper.Map<LuckyWheelViewModel>(item);
                luckyWheel.IsDoneToday = false;
                if (((DateTime)customer.LuckyWheelLastSpinDate).Date == DateTime.Today)
                {
                    luckyWheel.IsDoneToday = true;
                }
                response = APIContants<LuckyWheelViewModel>.CostumSuccessResult(luckyWheel, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<LuckyWheelViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
