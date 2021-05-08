using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class PrizesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrizesController(OpenSismDBContext context, UserManager<ApplicationUser> userManager,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/AppTasks
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<PrizeViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<PrizeViewModel>>> response = new Response<PagedContent<IPagedList<PrizeViewModel>>>();
            try
            {
                //NotDeleted
                var data = (from temp in _context.Prizes.Where(a => !a.IsDeleted)
                            .Where(a => a.Name != "lucky_wheel" && a.Name != "daily_bonus")
                    .OrderByDescending(a => a.Modified)
                            select temp);
                if (pagination.PrizeTypeId != null && pagination.PrizeTypeId != 0)
                {
                    data = data.Where(a => a.PrizeTypeId == pagination.PrizeTypeId);
                }
                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var prizes = Mapper.Map<IPagedList<PrizeViewModel>>(items);
                PagedContent<IPagedList<PrizeViewModel>> pagedContent = new PagedContent<IPagedList<PrizeViewModel>>();
                pagedContent.content = prizes;
                pagedContent.pagination = new Pagination(prizes.TotalItemCount, prizes.PageSize, prizes.PageCount, prizes.PageNumber);
                response = APIContants<PagedContent<IPagedList<PrizeViewModel>>>.CostumSuccessResult(pagedContent, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<PrizeViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        // GET: api/AppTasks/5
        [HttpPost]
        [Route("Get")]
        public Response<PrizeViewModel> Get([FromBody] PrizeViewModel model)
        {
            Response<PrizeViewModel> response = new Response<PrizeViewModel>();
            try
            {
                var item = _context.Prizes.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<PrizeViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var prize = Mapper.Map<PrizeViewModel>(item);
                response = APIContants<PrizeViewModel>.CostumSuccessResult(prize, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PrizeViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("RequestPrize")]
        public async Task<Response<CustomerPrizeViewModel>> RequestPrize([FromBody] CustomerPrizeViewModel model)
        {
            Response<CustomerPrizeViewModel> response = new Response<CustomerPrizeViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                Prize pr = _context.Prizes.Find(model.PrizeId);
                if(pr != null)
                {
                    if (customer.CurrentPoints >= pr.Points)
                    {
                        CustomerPrize customerPrize = new CustomerPrize();
                        customerPrize.CustomerId = customer.Id;
                        customerPrize.PrizeId = model.PrizeId;
                        customerPrize.RequestDate = DateTime.Now;
                        customerPrize.PrizeStatusId = _context.PrizeStatuses.Where(p => p.Name == "requested").FirstOrDefault().Id;
                        _context.CustomerPrizes.Add(customerPrize);
                        await _context.SaveChangesAsync();
                        customer.CurrentPoints = customer.CurrentPoints - customerPrize.Prize.Points;
                        _context.Customers.Update(customer);
                        await _context.SaveChangesAsync();
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
                        response = APIContants<CustomerPrizeViewModel>.CostumSuccessResult(Mapper.Map<CustomerPrizeViewModel>(customerPrize), customer);
                    }
                    else
                    {
                        response = APIContants<CustomerPrizeViewModel>.CostumNoPoints(_localizer["NoPoints"], null);
                    }
                }
                else
                {
                    response = APIContants<CustomerPrizeViewModel>.CostumNotFound(_localizer["NotFound"], null);
                }
                return response;
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
