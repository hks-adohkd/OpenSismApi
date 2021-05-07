using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using X.PagedList;


//this is our informations of whats up instgrame .....
namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public ContactsController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/Contacts
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<ContactViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<ContactViewModel>>> response = new Response<PagedContent<IPagedList<ContactViewModel>>>();
            try
            {
                var items = _context.Contacts.Where(a => !a.IsDeleted)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var contacts = Mapper.Map<IPagedList<ContactViewModel>>(items);
                PagedContent<IPagedList<ContactViewModel>> pagedContent = new PagedContent<IPagedList<ContactViewModel>>();
                pagedContent.content = contacts;
                pagedContent.pagination = new Pagination(contacts.TotalItemCount, contacts.PageSize, contacts.PageCount, contacts.PageNumber);
                response = APIContants<PagedContent<IPagedList<ContactViewModel>>>.CostumSuccessResult(pagedContent);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<ContactViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
