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
using System.Threading.Tasks;
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerPrizesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public CustomerPrizesController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<CustomerPrizeViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<CustomerPrizeViewModel>>> response = new Response<PagedContent<IPagedList<CustomerPrizeViewModel>>>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                //NotDeleted
                var data = (from temp in _context.CustomerPrizes.Where(a => !a.IsDeleted)
                                                .Where(m => m.CustomerId == customer.Id)
                                                .OrderByDescending(a => a.Modified)
                            select temp);
                if (pagination.PrizeTypeId != null)
                {
                    data = data.Where(a => a.Prize.PrizeTypeId == pagination.PrizeTypeId);
                }
                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var prizes = Mapper.Map<IPagedList<CustomerPrizeViewModel>>(items);
                PagedContent<IPagedList<CustomerPrizeViewModel>> pagedContent = new PagedContent<IPagedList<CustomerPrizeViewModel>>();
                pagedContent.content = prizes;
                pagedContent.pagination = new Pagination(prizes.TotalItemCount, prizes.PageSize, prizes.PageCount, prizes.PageNumber);
                response = APIContants<PagedContent<IPagedList<CustomerPrizeViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<CustomerPrizeViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Get")]
        public Response<CustomerPrizeViewModel> Get([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                var item = _context.CustomerPrizes.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerPrizeViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var prize = Mapper.Map<CustomerPrizeViewModel>(item);
                response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(prize, item.Customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerPrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Delete")]
        public Response<CustomerPrizeViewModel> Delete([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                var item = _context.CustomerPrizes.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerPrizeViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }

                item.IsDeleted = true;
                _context.Update(item);
                _context.SaveChanges();
                response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(null, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerPrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddLucky")]
        public async Task<Response<CustomerPrizeViewModel>> AddLucky([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                CustomerPrize customerPrize = Mapper.Map<CustomerPrize>(model);
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                TimeSpan ts = DateTime.Now.Subtract(customer.LuckyWheelLastSpinDate.Value);
                int NumberOfDays = (int)ts.TotalDays;
                if (NumberOfDays >= 1)
                {
                    customerPrize.CustomerId = customer.Id;
                    customerPrize.RequestDate = DateTime.Now;
                    customerPrize.EarnDate = DateTime.Now;
                    customerPrize.Description = "Lucky Points";
                    customerPrize.PrizeStatusId = _context.PrizeStatuses.Where(p => p.Name == "accepted").FirstOrDefault().Id;
                    _context.CustomerPrizes.Add(customerPrize);
                    await _context.SaveChangesAsync();
                    customerPrize.Prize = await _context.Prizes.FindAsync(customerPrize.PrizeId);
                    customer.LuckyWheelLastSpinDate = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();


                    response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(
                    Mapper.Map<CustomerPrizeViewModel>(customerPrize), customer);
                    return response;
                }
                else {
                    response = APIContants<CustomerPrizeViewModel>.CostumBonusgWrong(_localizer["NotAllowed"], null, customer);
                    Serilog.Log.Fatal("not Allowed wright Now ", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerPrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddLuckyPremium")]
        public async Task<Response<CustomerPrizeViewModel>> AddLuckyPremium([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                CustomerPrize customerPrize = Mapper.Map<CustomerPrize>(model);
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).Where(a => a.Premium == true).FirstOrDefault();
                if (customer != null)
                {
                    TimeSpan ts = DateTime.Now.Subtract(customer.LuckyWheelPremiumLastSpinDate.Value);
                    int NumberOfDays = (int)ts.TotalDays;
                    if (NumberOfDays >= 1)
                    {
                        customerPrize.CustomerId = customer.Id;
                        customerPrize.RequestDate = DateTime.Now;
                        customerPrize.EarnDate = DateTime.Now;
                        customerPrize.Description = "LuckyPremium Points";
                        customerPrize.PrizeStatusId = _context.PrizeStatuses.Where(p => p.Name == "accepted").FirstOrDefault().Id;
                        _context.CustomerPrizes.Add(customerPrize);
                        await _context.SaveChangesAsync();
                        customerPrize.Prize = await _context.Prizes.FindAsync(customerPrize.PrizeId);
                        customer.LuckyWheelPremiumLastSpinDate = DateTime.Now;
                        _context.Update(customer);
                        await _context.SaveChangesAsync();


                        response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(
                        Mapper.Map<CustomerPrizeViewModel>(customerPrize), customer);
                        return response;
                    }
                    else
                    {
                        response = APIContants<CustomerPrizeViewModel>.CostumBonusgWrong(_localizer["NotAllowed"], null, customer);
                        Serilog.Log.Fatal("not Allowed wright Now ", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                        return response;
                    }
                }
                else
                {
                    response = APIContants<CustomerPrizeViewModel>.CostumBonusgWrong(_localizer["SomethingWentWrong"], null, customer);
                    Serilog.Log.Fatal("not Allowed wright Now ", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerPrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddDailyBonus")]
        public async Task<Response<CustomerPrizeViewModel>> AddDailyBonus([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                CustomerPrize customerPrize = Mapper.Map<CustomerPrize>(model);
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
               // TimeSpan ts = DateTime.Now - customer.DailyBonusLastUseDate.Value;
                TimeSpan ts = DateTime.Now.Subtract(customer.DailyBonusLastUseDate.Value);
              
                int NumberOfDays = (int) ts.TotalDays;
                if (NumberOfDays >= 1)
                {
                    customerPrize.CustomerId = customer.Id;
                    customerPrize.RequestDate = DateTime.Now;
                    customerPrize.EarnDate = DateTime.Now;
                    customerPrize.Description = "DailyBonus Points";
                    customerPrize.PrizeStatusId = _context.PrizeStatuses.Where(p => p.Name == "accepted").FirstOrDefault().Id;
                    _context.CustomerPrizes.Add(customerPrize);
                    await _context.SaveChangesAsync();
                    customerPrize.Prize = await _context.Prizes.FindAsync(customerPrize.PrizeId);
                    customer.DailyBonusLastUseDate = DateTime.Now;
                    
                        customer.DailyBonusLevel = customer.DailyBonusLevel + 1;
                    
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(
                    Mapper.Map<CustomerPrizeViewModel>(customerPrize), customer);
                    return response;
                }
                else {
                    response = APIContants<CustomerPrizeViewModel>.CostumBonusgWrong(_localizer["NotAllowed"], null , customer);
                    Serilog.Log.Fatal("not Allowed wright Now ", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }


            }
            catch (Exception e)
            {
                response = APIContants<CustomerPrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
