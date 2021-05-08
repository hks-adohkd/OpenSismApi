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
    public class CustomerMessagesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public CustomerMessagesController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/CustomerMessages
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<CustomerMessageViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<CustomerMessageViewModel>>> response = new Response<PagedContent<IPagedList<CustomerMessageViewModel>>>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var items = _context.CustomerMessages.Where(a => !a.IsDeleted)
                    .Where(m => m.CustomerId == customer.Id)
                    .OrderByDescending(c => c.SendDate)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var messages = Mapper.Map<IPagedList<CustomerMessageViewModel>>(items);
                PagedContent<IPagedList<CustomerMessageViewModel>> pagedContent = new PagedContent<IPagedList<CustomerMessageViewModel>>();
                pagedContent.content = messages;
                pagedContent.pagination = new Pagination(messages.TotalItemCount, messages.PageSize, messages.PageCount, messages.PageNumber);
                response = APIContants<PagedContent<IPagedList<CustomerMessageViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<CustomerMessageViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Get")]
        public async Task<Response<CustomerMessageViewModel>> GetAsync([FromBody] CustomerMessageViewModel model)
        {
            Response<CustomerMessageViewModel> response = new Response<CustomerMessageViewModel>();
            try
            {
                var item = _context.CustomerMessages.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerMessageViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                item.IsRead = true;
                _context.Update(item);
                await _context.SaveChangesAsync();
                var message = Mapper.Map<CustomerMessageViewModel>(item);
                response = APIContants<CustomerMessageViewModel>.CostumSuccessResult(message, item.Customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerMessageViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<Response<CustomerMessageViewModel>> Delete([FromBody] CustomerMessageViewModel model)
        {
            Response<CustomerMessageViewModel> response = new Response<CustomerMessageViewModel>();
            try
            {
                var item = _context.CustomerMessages.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerMessageViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                item.IsDeleted = true;
                _context.Update(item);
                await _context.SaveChangesAsync();
                response = APIContants<CustomerMessageViewModel>.CostumSuccessResult(null, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerMessageViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
