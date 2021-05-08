using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using DBContext.Models;
using Microsoft.Extensions.Localization;
using DBContext.ViewModels;
using OpenSismApi.Models;
using X.PagedList;
using OpenSismApi.Helpers;
using AutoMapper;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        //private readonly IMapper _mapper;
        public CitiesController(OpenSismDBContext context ,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
          
        }
        
        // GET: api/Cities
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<CityViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<CityViewModel>>> response = new Response<PagedContent<IPagedList<CityViewModel>>>();
            try
            {
                var items = _context.Cities.Where(a => !a.IsDeleted)
                    .OrderBy(c => c.DisplayName)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                 var cities = Mapper.Map<IPagedList<CityViewModel>>(items);
               // var cities = _mapper.Map<IPagedList<CityViewModel>>(items);
                PagedContent<IPagedList<CityViewModel>> pagedContent = new PagedContent<IPagedList<CityViewModel>>();
                pagedContent.content = cities;
                pagedContent.pagination = new Pagination(cities.TotalItemCount, cities.PageSize, cities.PageCount, cities.PageNumber);
                response = APIContants<PagedContent<IPagedList<CityViewModel>>>.CostumSuccessResult(pagedContent);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<CityViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
              //  Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }


        [HttpGet]
        [Route("Get")]
        public Response<PagedContent<IPagedList<CityViewModel>>> Get([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<CityViewModel>>> response = new Response<PagedContent<IPagedList<CityViewModel>>>();
            try
            {
                var items = _context.Cities.Where(a => !a.IsDeleted)
                    .OrderBy(c => c.DisplayName)
                    .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var cities = Mapper.Map<IPagedList<CityViewModel>>(items);
                // var cities = _mapper.Map<IPagedList<CityViewModel>>(items);
                PagedContent<IPagedList<CityViewModel>> pagedContent = new PagedContent<IPagedList<CityViewModel>>();
                pagedContent.content = cities;
                pagedContent.pagination = new Pagination(cities.TotalItemCount, cities.PageSize, cities.PageCount, cities.PageNumber);
                response = APIContants<PagedContent<IPagedList<CityViewModel>>>.CostumSuccessResult(pagedContent);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<CityViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                //  Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }



    }
}
