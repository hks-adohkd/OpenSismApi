using OpenSismApi.Helpers;
using OpenSismApi.Models;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace OpenSismApi.AppStart
{
    public class CustomFilterAttribute : ActionFilterAttribute
    {
        public static string Authorization;
        public static string ContentType;
        public static string AcceptLanguage;
        public static string AppVersion;

        public static string Platform;
        public static string OsVersion;
        public static string MobileBrand;
        public static string DeviceId;

        public static string cultureName = "en";
        public static int RequestId = 0;

        private readonly OpenSismDBContext _context;
        public readonly IStringLocalizer<CustomFilterAttribute> _localizer;

        public CustomFilterAttribute(OpenSismDBContext context, IStringLocalizer<CustomFilterAttribute> localizer)
        {
            _localizer = localizer;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                //StringValues authorization;
                //context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorization);
                //if (authorization.ToList() != null && authorization.Count() > 0)
                //{
                //    Authorization = authorization.ElementAt(0).Replace("Bearer ", "");
                //    context.HttpContext.Request.RouteValues.Remove("Token");
                //    context.HttpContext.Request.RouteValues.Add("Token", Authorization);
                //}
                //else
                //{
                //    Serilog.Log.Warning("{@Response}", new Response<string>(APIContants.UNAUTHORIZED_CODE, _localizer["UnauthorizwdMsg"], _localizer["UnauthorizwdMsg"], null));
                //}

                context.HttpContext.Request.Headers.TryGetValue("Accept-Language", out StringValues acceptLanguage);
                if (acceptLanguage.ToList() == null || acceptLanguage.Count() <= 0)
                {
                    Serilog.Log.Warning("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing Accept-Language", "Missing Accept-Language", null));
                }
                else
                {
                    AcceptLanguage = acceptLanguage.ElementAt(0);
                }

                context.HttpContext.Request.Headers.TryGetValue("App-Version", out StringValues appVersion);
                if (appVersion.ToList() == null || appVersion.Count() <= 0)
                {
                    Serilog.Log.Error("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing App-Version", "Missing App-Version", null));
                }
                else
                {
                    AppVersion = appVersion.ElementAt(0);
                }

                context.HttpContext.Request.Headers.TryGetValue("Platform", out StringValues platform);
                if (platform.ToList() == null || platform.Count() <= 0)
                {
                    Serilog.Log.Error("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing Platform", "Missing Platform", null));
                }
                else
                {
                    Platform = platform.ElementAt(0);
                }

                context.HttpContext.Request.Headers.TryGetValue("Os-Version", out StringValues osVersion);
                if (osVersion.ToList() == null || osVersion.Count() <= 0)
                {
                    Serilog.Log.Warning("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing Os-Version", "Missing Os-Version", null));
                }
                else
                {
                    OsVersion = osVersion.ElementAt(0);
                }

                context.HttpContext.Request.Headers.TryGetValue("Mobile-Brand", out StringValues mobileBrand);
                if (mobileBrand.ToList() == null || mobileBrand.Count() <= 0)
                {
                    Serilog.Log.Warning("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing Mobile-Brand", "Missing Mobile-Brand", null));
                }
                else
                {
                    MobileBrand = mobileBrand.ElementAt(0);
                }

                StringValues deviceId;
                context.HttpContext.Request.Headers.TryGetValue("Device-Id", out deviceId);
                if (deviceId.ToList() == null || deviceId.Count() <= 0)
                {
                    Serilog.Log.Warning("{@Response}", new Response<string>(APIContants.MISSING_HEADER_CODE, "Missing Device-Id", "Missing Device-Id", null));
                }
                else
                {
                    DeviceId = deviceId.ElementAt(0);
                }

                //check version
                //var requestedVersion = _context.MobileAppVersions.Where(v => v.VersionNumber == AppVersion && v.PlatformType == Platform).FirstOrDefault();
                //var currentVersion = _context.MobileAppVersions.Where(m => m.IsCurrent && !m.IsDeleted && m.PlatformType == Platform).FirstOrDefault();

                //if (requestedVersion == null || !requestedVersion.IsSupported || requestedVersion.IsDeleted)
                //{
                //    Serilog.Log.Error("{@Response}", new Response<MobileAppVersion>(APIContants.VERSION_NOT_SUPPORTED_CODE, _localizer["NotSupportedVersion"], requestedVersion, null));
                //    var outer = AutoMapper.Mapper.Map<MobileAppVersionViewModel>(currentVersion);
                //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //    context.HttpContext.Response.ContentType = "application/json";
                //    HttpResponseWritingExtensions.WriteAsync(context.HttpContext.Response, JsonConvert.SerializeObject(APIContants<MobileAppVersionViewModel>
                //        .CostumSometingWrong(_localizer["NotSupportedVersion"], outer)));
                //    return;
                //}

                StringValues values;
                if (context.HttpContext.Request.Headers.TryGetValue("Accept-Language", out values))
                {
                    cultureName = values.ToList() != null && values.Count() > 0 ? values.FirstOrDefault() : "en";
                }

                //cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                context.HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName)));

                OpenSismDBContext._culture = cultureName;
                if (!context.HttpContext.Request.RouteValues.ContainsKey("CurrentCulture"))
                {
                    context.HttpContext.Request.RouteValues.Add("CurrentCulture", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
                }
                else
                {
                    context.HttpContext.Request.RouteValues.Remove("CurrentCulture");
                    context.HttpContext.Request.RouteValues.Add("CurrentCulture", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
                }
                object controller, action;
                UserLog userLog = new UserLog();
                userLog.AcceptLanguage = AcceptLanguage;
                context.HttpContext.Request.RouteValues.TryGetValue("action", out action);
                context.HttpContext.Request.RouteValues.TryGetValue("controller", out controller);
                userLog.Action = controller.ToString() + "/" + action.ToString();
                userLog.AppVersion = AppVersion;
                //string PhoneNumber = context.HttpContext.User.Identity.Name;
                //Customer customer = _context.Customers.Where(c => c.User.UserName == PhoneNumber).FirstOrDefault();
                //if (customer != null)
                //{
                //    userLog.CustomerId = customer.Id;
                //}
                userLog.DeviceId = DeviceId;
                userLog.MobileBrand = MobileBrand;
                userLog.OsVersion = OsVersion;
                userLog.Platform = Platform;
                userLog.Status = "success";
               // userLog.Token = Authorization;
                _context.UserLogs.Add(userLog);
                _context.SaveChanges();
               // RequestId = userLog.Id;

                base.OnActionExecuting(context);
            }

            catch (Exception e)
            {
                Serilog.Log.Fatal(e, "{@Response}", new Response<Exception>(APIContants.SOMETHING_WENT_WROEG_CODE, _localizer["SomethingWentWrong"], e, null));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.HttpContext.Response.ContentType = JsonConvert.SerializeObject(APIContants<string>.CostumSometingWrong(_localizer["SomethingWentWrong"], null));
            }
            base.OnActionExecuting(context);
        }
    }
}
