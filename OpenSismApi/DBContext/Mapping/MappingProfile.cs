using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using X.PagedList;
//using X.PagedList;

namespace DBContext.Mapping
{
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public MappingProfile()
        {
            CreateMap<ApplicationUserViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, ApplicationUserViewModel>();

            CreateMap<CityViewModel, City>();
            CreateMap<City, CityViewModel>();
            CreateMap<IPagedList<City>, IPagedList<CityViewModel>>().ConvertUsing<PagedListConverter<City, CityViewModel>>();
            CreateMap<IPagedList<CityViewModel>, IPagedList<City>>().ConvertUsing<PagedListConverter<CityViewModel, City>>();
            CreateMissingTypeMaps = true;
        }
    }
}
