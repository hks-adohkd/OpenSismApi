using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public ContentsController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/Contents
        [HttpPost]
        [Route("AboutUs")]
        public Response<ContentViewModel> AboutUs([FromBody] ContentViewModel model)
        {
            Response<ContentViewModel> response = new Response<ContentViewModel>();
            try
            {
                var item = _context.Contents.Where(a => a.Name == "about_us").FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<ContentViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                response = APIContants<ContentViewModel>.CostumSuccessResult(Mapper.Map<ContentViewModel>(item));
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<ContentViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("GetIntro")]
        public Response<IntroViewModel> GetIntro([FromBody] PaginationViewModel pagination)
        {
            Response<IntroViewModel> response = new Response<IntroViewModel>();
            try
            {
                IntroViewModel intro = new IntroViewModel();
                var items = _context.Contents.Where(a => !a.IsDeleted && a.Name == "intro")
                    .OrderBy(p => p.ItemOrder).ToList();
                
                intro.IntroImages = new List<ContentViewModel>();
                foreach (var item in items)
                {
                    intro.IntroImages.Add(Mapper.Map<ContentViewModel>(item));
                }
                var video = _context.Contents.Where(a => !a.IsDeleted && a.Name == "intro_video").FirstOrDefault();
                intro.IntroVideo = Mapper.Map<ContentViewModel>(video);

                response = APIContants<IntroViewModel>.CostumSuccessResult(intro);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<IntroViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("GetIntroVideo")]
        public Response<ContentViewModel> GetIntroVideo([FromBody] PaginationViewModel pagination)
        {
            Response<ContentViewModel> response = new Response<ContentViewModel>();
            try
            {
                var item = _context.Contents.Where(a => !a.IsDeleted && a.Name == "intro_video").FirstOrDefault();
                var video = Mapper.Map<ContentViewModel>(item);
               
                response = APIContants<ContentViewModel>.CostumSuccessResult(video);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<ContentViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
