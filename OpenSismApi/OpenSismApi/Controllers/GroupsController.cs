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
    public class GroupsController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupsController(OpenSismDBContext context, UserManager<ApplicationUser> userManager,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/AppTasks
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<GroupViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<GroupViewModel>>> response = new Response<PagedContent<IPagedList<GroupViewModel>>>();
            try
            {
                //NotDeleted
                var data = (from temp in _context.Groups.Where(a => !a.IsDeleted)
                    .OrderByDescending(a => a.ItemOrder)
                            select temp);
              
                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var grooups = Mapper.Map<IPagedList<GroupViewModel>>(items);
                PagedContent<IPagedList<GroupViewModel>> pagedContent = new PagedContent<IPagedList<GroupViewModel>>();
                pagedContent.content = grooups;
                pagedContent.pagination = new Pagination(grooups.TotalItemCount, grooups.PageSize, grooups.PageCount, grooups.PageNumber);
                response = APIContants<PagedContent<IPagedList<GroupViewModel>>>.CostumSuccessResult(pagedContent, null);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<GroupViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        // GET: api/AppTasks/5
        [HttpPost]
        [Route("Get")]
        public Response<GroupViewModel> Get([FromBody] GroupViewModel model)
        {
            Response<GroupViewModel> response = new Response<GroupViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var item = _context.Groups.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<GroupViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var group = Mapper.Map<GroupViewModel>(item);
                response = APIContants<GroupViewModel>.CostumSuccessResult(group, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<GroupViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

    }
}
