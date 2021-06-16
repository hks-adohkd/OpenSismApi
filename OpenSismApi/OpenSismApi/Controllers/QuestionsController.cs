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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public QuestionsController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/Questions
        [HttpPost]
        [Route("Get")]
        public Response<QuestionViewModel> Get([FromBody] QuestionViewModel model)
        {
            Response<QuestionViewModel> response = new Response<QuestionViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Include(c => c.CustomerTasks)
                    .Where(c => c.User.UserName == username).FirstOrDefault();

                bool IsDone = false;
                foreach (var ct in customer.CustomerTasks)
                {
                    if (ct.AppTaskId == model.AppTaskId && ct.IsDone)
                    {
                        IsDone = true;
                        break;
                    }
                }
                if (IsDone)
                {
                    response = APIContants<QuestionViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }

                var item = _context.Questions.Include(q => q.QuestionOptions).Include(q => q.CustomerAnswers)
                    .Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted)
                    .Where(q => q.ItemOrder == model.ItemOrder)
                    .FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<QuestionViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                QuestionViewModel question = Mapper.Map<QuestionViewModel>(item);
                int totalQuestionsCount = _context.Questions.Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted).Count();
                question.IsLast = item.ItemOrder == totalQuestionsCount;
                question.TotalQuestionsCount = totalQuestionsCount;

                response = APIContants<QuestionViewModel>.CostumSuccessResult(question, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<QuestionViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Response<QuestionViewModel>> Add([FromBody] CustomerAnswerViewModel model)
        {
            Response<QuestionViewModel> response = new Response<QuestionViewModel>();
            try
            {
                CustomerAnswer customerAnswer = Mapper.Map<CustomerAnswer>(model);
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                customerAnswer.CustomerId = customer.Id;

                CustomerAnswer oldAnswer = _context.CustomerAnswers.Where(c => c.CustomerId == customer.Id
                                        && c.QuestionId == model.QuestionId).FirstOrDefault();
                var item = _context.Questions.Include(q => q.QuestionOptions).Include(q => q.CustomerAnswers)
                    .Include(q => q.AppTask)
                    .Where(a => a.Id == model.QuestionId && !a.IsDeleted)
                    .FirstOrDefault();

                if (oldAnswer == null)
                {
                    if (!item.Type)
                    {
                        customerAnswer.QuestionOptionId = null;
                        customerAnswer.QuestionOption = null;
                        customerAnswer.IsRightAnswer = true;
                    }
                    else
                    {
                        QuestionOption questionOption = _context.QuestionOptions.Find(customerAnswer.QuestionOptionId);
                        if(questionOption != null)
                        {
                            if(questionOption.IsRightOption)
                                customerAnswer.IsRightAnswer = true;
                            else
                                customerAnswer.IsRightAnswer = false;
                        }
                        else
                        {
                            customerAnswer.IsRightAnswer = false;
                        }
                    }
                    _context.CustomerAnswers.Add(customerAnswer);
                }
                else
                {
                    if (!item.Type)
                    {
                        oldAnswer.QuestionOptionId = null;
                        oldAnswer.QuestionOption = null;
                        oldAnswer.Answer = customerAnswer.Answer;
                    }
                    else
                    {
                        oldAnswer.QuestionOptionId = customerAnswer.QuestionOptionId;
                        QuestionOption questionOption = _context.QuestionOptions.Find(customerAnswer.QuestionOptionId);
                        if (questionOption != null)
                        {
                            if (questionOption.IsRightOption)
                                customerAnswer.IsRightAnswer = true;
                            else
                                customerAnswer.IsRightAnswer = false;
                        }
                        else
                        {
                            customerAnswer.IsRightAnswer = false;
                        }
                    }
                    _context.CustomerAnswers.Update(oldAnswer);
                }
                await _context.SaveChangesAsync();

                if (item == null)
                {
                    response = APIContants<QuestionViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }

                int totalQuestionsCount = _context.Questions.Where(a => a.AppTaskId == item.AppTaskId && !a.IsDeleted).Count();
                bool IsAllTrue = true;
                if (item.ItemOrder == totalQuestionsCount)
                {
                    List<CustomerAnswer> customerAnswers = _context.CustomerAnswers.Where(c => c.CustomerId == customer.Id
                                        && c.Question.AppTaskId == item.AppTaskId).ToList();
                    foreach (var ca in customerAnswers)
                    {
                        if (!ca.IsRightAnswer)
                        {
                            IsAllTrue = false;
                            break;
                        }
                    }
                    CustomerTask customerTask = new CustomerTask();
                    if (IsAllTrue)
                    {
                        customerTask.EarnedPoints = item.AppTask.Points;

                        customer.CurrentPoints = customer.CurrentPoints + item.AppTask.Points;
                        customer.TotalPoints = customer.TotalPoints + item.AppTask.Points;
                        int nextGroup = customer.Group.ItemOrder + 1;
                        Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                        if (group != null)
                        {
                            if (customer.TotalPoints >= group.Points)
                            {
                                customer.GroupId = group.Id;
                                customer.Group = group;
                                int nextNextGroupOrder = nextGroup + 1;
                                Group nextNextGroup = _context.Groups.Where(g => g.ItemOrder == nextNextGroupOrder).FirstOrDefault();
                                if (group != null)
                                {
                                    customer.NextGroupPoints = nextNextGroup.Points;
                                }
                                else
                                {
                                    customer.NextGroupPoints = 0;
                                }
                            }
                            else
                            {
                                customer.NextGroupPoints = group.Points;
                            }
                        }
                        else
                        {
                            customer.NextGroupPoints = 0;
                        }
                        _context.Customers.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        customerTask.EarnedPoints = 0;
                    }

                    customerTask.AppTaskId = item.AppTaskId;
                    customerTask.CustomerId = customer.Id;
                    customerTask.DoneDate = DateTime.Now;
                    customerTask.IsDone = true;
                    _context.CustomerTasks.Add(customerTask);
                    await _context.SaveChangesAsync();
                }

                QuestionViewModel question = Mapper.Map<QuestionViewModel>(item);
                question.IsLast = item.ItemOrder == totalQuestionsCount;
                question.TotalQuestionsCount = totalQuestionsCount;
                question.IsAllRight = IsAllTrue;
                response = APIContants<QuestionViewModel>.CostumSuccessResult(question, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<QuestionViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }
    }
}
