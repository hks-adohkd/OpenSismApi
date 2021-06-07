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
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        public CustomerController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            this.userManager = userManager;
            _context = context;
        }


        [HttpPost]
        [Route("GetTime")]
        public async Task<IActionResult> GetTimeAsync()
        {
            // Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            var username = User.Identity.Name;
            var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

            if (customer != null)
            {
                String dateStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff");
                DateTime tempDate = DateTime.ParseExact(dateStr, "MM/dd/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
              //  DateTime tempDate = DateTime.Parse("yyyy-MM-dd hh:mm:ss");

                var response = new TimeResponse(tempDate);
                return Ok(response);

            }
            else
            {
                return BadRequest(new { message = "Customer Not Found" });
            }

            //   var response = _userService.Authenticate(model);





        }

        [HttpPost]
        [Route("GetCustomer")]
        public ResponseNew GetCustomer()
        {
            ResponseNew response = new ResponseNew();
            try
            {

               // CustomerViewModel customerViewModel = new CustomerViewModel();
               
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

               
              

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

                response = APIContants.CostumSuccessResultNew( customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants.CostumSometingWrongNew(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
