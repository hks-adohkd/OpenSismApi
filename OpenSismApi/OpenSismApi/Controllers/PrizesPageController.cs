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
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrizesPageController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public PrizesPageController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetPrizePage")]

        public Response<PrizePageViewModel> GetPrizePage()
        {
            Response<PrizePageViewModel> response = new Response<PrizePageViewModel>();
            try
            {
                

               


                PrizePageViewModel prizePageViewModel = new PrizePageViewModel();
                var data = (from temp in _context.Prizes.Where(a => !a.IsDeleted)
                            .Where(a => a.Name != "lucky_wheel" && a.Name != "daily_bonus")
                    .OrderByDescending(a => a.Modified)
                            select temp);
               
                prizePageViewModel.Prizes = Mapper.Map<List<PrizeViewModel>>(data);

                response = APIContants<PrizePageViewModel>.CostumSuccessResult(prizePageViewModel, null);
                return response;



            }
            catch (Exception e)
            {
                response = APIContants<PrizePageViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            } 
        }

        }
    }

