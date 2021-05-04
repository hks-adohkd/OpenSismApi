//using API.AppStart;
using OpenSismApi.AppStart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace OpenSismApi.Controllers
{
    [ApiController]

    [ServiceFilter(typeof(CustomFilterAttribute))]
    public class BaseController : ControllerBase
    {

        public readonly IStringLocalizer<BaseController> _localizer;

        public BaseController(IStringLocalizer<BaseController> localizer)
        {
            _localizer = localizer;
        }
    }
}
