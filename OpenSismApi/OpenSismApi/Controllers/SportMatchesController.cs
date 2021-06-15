using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SportMatchesController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public SportMatchesController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }


        //[HttpPost]
        //[Route("Get")]
        //public Response<CustomerPredictionViewModel> Get([FromBody] SportMatchViewModel model)
        //{
        //    Response<CustomerPredictionViewModel> response = new Response<CustomerPredictionViewModel>();
        //    try
        //    {
        //        var username = User.Identity.Name;
        //        var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
        //        var item = _context.SportMatches.Include(s => s.AppTask).Include(s => s.AppTask.CustomerTasks)
        //            .Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted && !a.AppTask.IsDeleted).FirstOrDefault();

        //        var prediction = _context.CustomerPredictions.Where(c => c.CustomerId == customer.Id).FirstOrDefault();
        //        CustomerPredictionViewModel predictionModel = Mapper.Map<CustomerPredictionViewModel>(prediction);
        //        if (item == null)
        //        {
        //            response = APIContants<CustomerPredictionViewModel>.CostumNotFound(_localizer["NotFound"], null);
        //            Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
        //            return response;
        //        }
        //        var sportMatch = Mapper.Map<SportMatchViewModel>(item);
        //       // sportMatch.CustomerPrediction = Mapper.Map<CustomerPredictionViewModel>(prediction);
        //        sportMatch.AppTask.IsDone = false;
        //        item.AppTask.CustomerTasks = item.AppTask.CustomerTasks.Where(c => c.CustomerId == customer.Id)
        //            .Where(c => c.AppTask.TaskType.Name != "share_games_app").ToList();
        //        if (item.AppTask.CustomerTasks.Count() > 0)
        //        {
        //            foreach (var ct in item.AppTask.CustomerTasks)
        //            {
        //                if (ct.IsDone)
        //                {
        //                    sportMatch.AppTask.IsDone = true;
        //                    break;
        //                }
        //            }
        //        }
        //        int count = _context.CustomerTasks.Where(c => c.IsDone && c.AppTaskId == item.Id).Count();
        //        if (count < item.AppTask.Limit)
        //            sportMatch.AppTask.IsReachLimit = false;
        //        else
        //            sportMatch.AppTask.IsReachLimit = true;
        //        predictionModel.SportMatch = Mapper.Map<SportMatchViewModel>(sportMatch);
        //        response = APIContants<CustomerPredictionViewModel>.CostumSuccessResult(predictionModel, customer);
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        response = APIContants<CustomerPredictionViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
        //        Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
        //        return response;
        //    }
        //}

        //GET: api/Questions
       [HttpPost]
       [Route("Get")]
        public Response<SportMatchViewModel> Get([FromBody] SportMatchViewModel model)
        {
            Response<SportMatchViewModel> response = new Response<SportMatchViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var item = _context.SportMatches.Include(s => s.AppTask).Include(s => s.AppTask.CustomerTasks)
                    .Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted && !a.AppTask.IsDeleted).FirstOrDefault();

                var prediction = item.CustomerPredictions.Where(c => c.CustomerId == customer.Id  ).FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<SportMatchViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var sportMatch = Mapper.Map<SportMatchViewModel>(item);
                sportMatch.CustomerPrediction = Mapper.Map<CustomerPredictionViewModel>(prediction);
                sportMatch.AppTask.IsDone = false;
                item.AppTask.CustomerTasks = item.AppTask.CustomerTasks.Where(c => c.CustomerId == customer.Id)
                    .Where(c => c.AppTask.TaskType.Name != "share_games_app").ToList();
                if (item.AppTask.CustomerTasks.Count() > 0)
                {
                    foreach (var ct in item.AppTask.CustomerTasks)
                    {
                        if (ct.IsDone)
                        {
                            sportMatch.AppTask.IsDone = true;
                            break;
                        }
                    }
                }
                int count = _context.CustomerTasks.Where(c => c.IsDone && c.AppTaskId == item.Id).Count();
                if (count < item.AppTask.Limit)
                    sportMatch.AppTask.IsReachLimit = false;
                else
                    sportMatch.AppTask.IsReachLimit = true;
                response = APIContants<SportMatchViewModel>.CostumSuccessResult(sportMatch, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<SportMatchViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddEnd")]
        public async Task<Response<CustomerTaskViewModel>> AddEnd([FromBody] CustomerPredictionViewModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
               
                SportMatch sportMatch = _context.SportMatches.Find(model.SportMatchId);
                SportMatchViewModel match = Mapper.Map<SportMatchViewModel>(sportMatch);
                if (sportMatch == null)
                {
                    response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["NotFound"], null);
                   // Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
                CustomerTask customerTask = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id
                && c.AppTaskId == sportMatch.AppTaskId).FirstOrDefault();
                // CustomerTask customerTask = null;

                if (customerTask == null)
                {
                    customerTask = new CustomerTask();
                    customerTask.CustomerId = customer.Id;
                    customerTask.AppTaskId = sportMatch.AppTaskId;
                    customerTask.DoneDate = DateTime.Now;
                    //customerTask.IsDone = true;
                    customerTask.StartDate = DateTime.Now;
                    customerTask.EarnedPoints = 0;

                    _context.CustomerTasks.Add(customerTask);
                    await _context.SaveChangesAsync();
                    // return null;
                }
                else if (customerTask.IsDone == true) {
                    response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                    return response;
                }
                else
                {
                    customerTask.DoneDate = DateTime.Now;
                    customerTask.IsDone = true;

                    _context.CustomerTasks.Update(customerTask);
                    await _context.SaveChangesAsync();
                }
                
                CustomerPrediction customerPrediction = new CustomerPrediction();
                customerPrediction.CustomerId = customer.Id;
                customerPrediction.FirstTeamScore = model.FirstTeamScore;
                customerPrediction.SecondTeamScore = model.SecondTeamScore;
                customerPrediction.SportMatchId = sportMatch.Id;
                _context.CustomerPredictions.Add(customerPrediction);
                await _context.SaveChangesAsync();
                CustomerTaskViewModel Task = Mapper.Map<CustomerTaskViewModel>(customerTask);
                Task.SportMatch = Mapper.Map<SportMatchViewModel>(match);
                response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(Task, customer);
                return response;
            } 
            catch (Exception e)
            {
                response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
