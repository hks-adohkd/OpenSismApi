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
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerTasksController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public CustomerTasksController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/CustomerTasks
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<CustomerTaskViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<CustomerTaskViewModel>>> response = new Response<PagedContent<IPagedList<CustomerTaskViewModel>>>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var items = _context.CustomerTasks.Where(a => !a.IsDeleted)
                    .Where(m => m.CustomerId == customer.Id)
                    .OrderByDescending(c => c.DoneDate)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var tasks = Mapper.Map<IPagedList<CustomerTaskViewModel>>(items);
                foreach (var item in tasks)
                {
                    if(item.AppTask.TaskType.Name == "sport_match")
                    {
                        SportMatch sportMatch = _context.SportMatches.Where(s => s.AppTaskId == item.AppTaskId).FirstOrDefault();
                        item.SportMatch = Mapper.Map<SportMatchViewModel>(sportMatch);
                        item.SportMatch.AppTask = null;
                    }
                }
                PagedContent<IPagedList<CustomerTaskViewModel>> pagedContent = new PagedContent<IPagedList<CustomerTaskViewModel>>();
                pagedContent.content = tasks;
                pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
                response = APIContants<PagedContent<IPagedList<CustomerTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<CustomerTaskViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Get")]
        public Response<CustomerTaskViewModel> Get([FromBody] CustomerTaskViewModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var item = _context.CustomerTasks.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerTaskViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var task = Mapper.Map<CustomerTaskViewModel>(item);
                response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(task, item.Customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Delete")]
        public Response<CustomerTaskViewModel> Delete([FromBody] CustomerTaskViewModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var item = _context.CustomerTasks.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<CustomerTaskViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                item.IsDeleted = true;
                _context.Update(item);
                _context.SaveChanges();
                response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(null, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}

