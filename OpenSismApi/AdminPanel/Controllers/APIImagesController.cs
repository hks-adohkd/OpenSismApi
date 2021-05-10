using AdminPanel.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace AdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIImagesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public APIImagesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("SaveMedia")]
        [AllowAnonymous]
        public Response<CustomerViewModel> SaveMedia()
        {
            Response<CustomerViewModel> response;
            try
            {
                var file = Request.Form.Files[0];
                if (file != null)
                {
                    Customer customer = new Customer();
                    FileInfo fi = new FileInfo(file.FileName);
                    var newFilename = "P" + customer.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\customers\" + newFilename);
                    var pathToSave = @"/images/customers/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    customer.ImageUrl = pathToSave;
                    response = new Response<CustomerViewModel>(1, "success", Mapper.Map<CustomerViewModel>(customer));
                    return response;
                }
                else
                {
                    response = new Response<CustomerViewModel>(-3, "failed", null);
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = new Response<CustomerViewModel>(-3, ex.Message, null);
                return response;
            }
        }
    }
}
