using AdminPanel.Models;
using DBContext.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaseController : Controller
    {
        private readonly OpenSismDBContext db;
        public static string SuccessMsg = "Operation accomplished successfully";
        public static string FailedMsg = "Something went wrong";
        private IHostingEnvironment env;

        public BaseController(OpenSismDBContext myGamesDBContext, IHostingEnvironment? env)
        {
            db = myGamesDBContext;
            this.env = env;
        }

        public BaseController(OpenSismDBContext myGamesDBContext)
        {
            db = myGamesDBContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Dictionary<string, string> sessionActive = HttpContext.Session.GetComplexData<Dictionary<string, string>>("Active");
            string controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            string actionName = ControllerContext.RouteData.Values["action"].ToString();
            if (sessionActive == null)
            {
                sessionActive = new Dictionary<string, string>();
                sessionActive.Add("Home", "");
                sessionActive.Add("AppTasks", "");
                sessionActive.Add("AppTasks.Header", "");
                sessionActive.Add("Cities", "");
                sessionActive.Add("Cities.Header", "");
                sessionActive.Add("Conditions", "");
                sessionActive.Add("Conditions.Header", "");
                sessionActive.Add("Contacts", "");
                sessionActive.Add("Contacts.Header", "");
                sessionActive.Add("ContactUs", "");
                sessionActive.Add("ContactUs.Header", "");

                sessionActive.Add("ReplayForCustomer", "");
                sessionActive.Add("ReplayForCustomer.Header", "");
                sessionActive.Add("ReplayForCustomer.Index", "");
                sessionActive.Add("ReplayForCustomer.Details", "");

                

                sessionActive.Add("CustomerMessages", "");
                sessionActive.Add("CustomerMessages.Header", "");
                sessionActive.Add("CustomerMessages.Index", "");
                sessionActive.Add("CustomerMessages.Details", "");
                sessionActive.Add("CustomerMessages.Details", "");

                sessionActive.Add("MailReplay", "");
                sessionActive.Add("MailReplay.Header", "");
                sessionActive.Add("MailReplay.Index", "");
                sessionActive.Add("MailReplay.Details", "");

                sessionActive.Add("Contents", "");
                sessionActive.Add("Contents.Header", "");
                sessionActive.Add("Contents.Index", "");
                sessionActive.Add("Contents.Create", "");
                sessionActive.Add("Contents.Delete", "");
                sessionActive.Add("Contents.Details", "");
                sessionActive.Add("Contents.Edit", "");
                sessionActive.Add("Contents.IndexBanner", "");
                sessionActive.Add("Contents.EditBanner", "");

                sessionActive.Add("Contents.IntroIndex", "");
                sessionActive.Add("Contents.IntroCreate", "");
                sessionActive.Add("Contents.IntroDelete", "");
                sessionActive.Add("Contents.IntroDetails", "");
                sessionActive.Add("Contents.IntroEdit", "");
                sessionActive.Add("Contents.AboutUs", "");
                sessionActive.Add("Contents.IntroVidoe", "");
                sessionActive.Add("Customers", "");
                sessionActive.Add("Customers.Header", "");
                sessionActive.Add("DailyBonuses", "");
                sessionActive.Add("DailyBonuses.Header", "");
                sessionActive.Add("Groups", "");
                sessionActive.Add("Groups.Header", "");
                sessionActive.Add("Logs", "");
                sessionActive.Add("Logs.Header", "");
                sessionActive.Add("LuckyWheels", "");
                sessionActive.Add("LuckyWheels.Header", "");
                sessionActive.Add("Messages", "");
                sessionActive.Add("Messages.Header", "");
                sessionActive.Add("MobileAppVersions", "");
                sessionActive.Add("MobileAppVersions.Header", "");
                sessionActive.Add("Prizes", "");
                sessionActive.Add("Prizes.Header", "");
                sessionActive.Add("PrizeTypes", "");
                sessionActive.Add("PrizeTypes.Header", "");
                sessionActive.Add("PrizeStatus", "");
                sessionActive.Add("PrizeStatus.Header", "");
                sessionActive.Add("CustomerPrizes", "");
                sessionActive.Add("CustomerPrizes.Header", "");
                sessionActive.Add("TaskTypes", "");
                sessionActive.Add("TaskTypes.Header", "");
                sessionActive.Add("Users", "");
                sessionActive.Add("Users.Header", "");
                sessionActive.Add("UserLogs", "");
                sessionActive.Add("UserLogs.Header", "");

            }
            SetActive(sessionActive, controllerName, actionName);
            HttpContext.Session.SetComplexData("Active", sessionActive);
            HttpContext.Session.SetString("ControllerName", controllerName);
            HttpContext.Session.SetString("ActionName", actionName);

            int newContacts = db.ContactsUs.Where(c => !c.IsDeleted && !c.IsViewed).ToList().Count();
            HttpContext.Session.SetString("NewContacts", newContacts.ToString());

            int newPrizes = db.CustomerPrizes.Where(c => !c.IsDeleted && c.PrizeStatus.Name == "requested").ToList().Count();
            HttpContext.Session.SetString("NewPrizes", newPrizes.ToString());

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        protected void SetActive(Dictionary<string, string> sessionActive, string controllerName, string actionName)
        {
            if (sessionActive != null)
            {
                if (controllerName == "CustomerTasks" || controllerName == "CustomerGroups")
                {
                    sessionActive["Customers"] = "active";
                }
                else if (controllerName == "AppTaskGroups" || controllerName == "Questions" || controllerName == "QuestionOptions")
                {
                    sessionActive["AppTasks"] = "active";
                }
                else if (controllerName == "MessageGroups")
                {
                    sessionActive["Messages"] = "active";
                }
                else
                {
                    sessionActive[controllerName + ".Header"] = "menu-open";
                    sessionActive[controllerName] = "active";
                    sessionActive[controllerName + "." + actionName] = "active";
                    List<string> keys = new List<string>(sessionActive.Keys.Where(k => !k.Equals(controllerName)
                    && !k.Equals(controllerName + "." + actionName) && !k.Equals(controllerName + ".Header")));
                    foreach (var item in keys)
                    {
                        sessionActive[item] = "";
                    }
                }
            }
        }

        public async Task SendEmail(DBContext.Models.Mail customerMail, string SenderEmail)
        {
            MailAddress to = new MailAddress(customerMail.RecieverEmail);
            MailAddress from = new MailAddress(SenderEmail);
            MailMessage message = new MailMessage(from, to);
            message.Subject = customerMail.Subject;
            message.Body =customerMail.Message;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("smtp_username", "smtp_password"),
                EnableSsl = true
            };




        }

        public async Task SendNotification(DBContext.Models.Message customerMessage, string token)
        {
            var path = env.ContentRootPath;
            path = path + "\\mygamesapp-90f10-firebase-adminsdk-tmuzu-47e23e1e3f.json";
            FirebaseApp app = null;
            try
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                }, "mygamesapp-90f10");
            }
            catch (Exception ex)
            {
                app = FirebaseApp.GetInstance("mygamesapp-90f10");
            }

            var fcm = FirebaseAdmin.Messaging.FirebaseMessaging.GetMessaging(app);
            FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new Notification
                {
                    Title = customerMessage.Title,
                    Body = customerMessage.Script
                },
                Data = new Dictionary<string, string>()
                 {
                     { "AdditionalData1", "data 1" },
                     { "AdditionalData2", "data 2" },
                     { "AdditionalData3", "data 3" },
                 },

                Token = token
            };

            await fcm.SendAsync(message);
        }
    }
}