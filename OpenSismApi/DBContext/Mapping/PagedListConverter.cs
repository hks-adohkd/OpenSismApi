using AutoMapper;
using X.PagedList;

namespace DBContext.Mapping
{
    public class PagedListConverter<Te, Tv> : ITypeConverter<IPagedList<Te>, IPagedList<Tv>>
    {
        public IPagedList<Tv> Convert(
            IPagedList<Te> source,
            IPagedList<Tv> destination,
            ResolutionContext context)
        {
            var models = (IPagedList<Te>)source;
            var viewModels = models.Select(Mapper.Map<Te, Tv>);

            return new StaticPagedList<Tv>(
                viewModels,
                models);
        }
    }
}
