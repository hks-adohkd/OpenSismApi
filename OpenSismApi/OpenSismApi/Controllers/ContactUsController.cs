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
using System.Threading.Tasks;
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public ContactUsController(OpenSismDBContext context,
             IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }


        //get all customer messages 
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<ContactUsViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<ContactUsViewModel>>> response = new Response<PagedContent<IPagedList<ContactUsViewModel>>>();
            try
            {
                var items = _context.ContactsUs.Where(a => !a.IsDeleted)
                    .Where(a => a.IsFeatured)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var contacts = Mapper.Map<IPagedList<ContactUsViewModel>>(items);
                PagedContent<IPagedList<ContactUsViewModel>> pagedContent = new PagedContent<IPagedList<ContactUsViewModel>>();
                pagedContent.content = contacts;
                pagedContent.pagination = new Pagination(contacts.TotalItemCount, contacts.PageSize, contacts.PageCount, contacts.PageNumber);
                response = APIContants<PagedContent<IPagedList<ContactUsViewModel>>>.CostumSuccessResult(pagedContent);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<ContactUsViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        //add message from customer to database 
        [HttpPost]
        [Route("Add")]
        public async Task<Response<ContactUsViewModel>> Add([FromBody] ContactUsViewModel model)
        {
            Response<ContactUsViewModel> response = new Response<ContactUsViewModel>();
            try
            {
                ContactUs contactUs = Mapper.Map<ContactUs>(model);
                var username = User.Identity.Name;
                if (username != null)
                {
                    var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                    contactUs.CustomerId = customer.Id;
                    contactUs.FirstName = customer.FirstName;
                    contactUs.LastName = customer.LastName;
                    contactUs.PhoneNumber = customer.User.PhoneNumber;

                }
                _context.ContactsUs.Add(contactUs);
                await _context.SaveChangesAsync();
                response = APIContants<ContactUsViewModel>.CostumSuccessResult(
                Mapper.Map<ContactUsViewModel>(contactUs));
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<ContactUsViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
