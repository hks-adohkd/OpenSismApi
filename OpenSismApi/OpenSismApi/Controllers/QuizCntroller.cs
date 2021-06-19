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
using X.PagedList;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : BaseController
    {
        private readonly OpenSismDBContext _context;

        public QuizController(OpenSismDBContext context,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
        }

        // GET: api/Questions
        [HttpPost]
        [Route("Get")]
        public Response<QuizViewModel> Get([FromBody] QuizViewModel model)
        {
            Response<QuizViewModel> response = new Response<QuizViewModel>();
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
                    response = APIContants<QuizViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }

                var item = _context.Quizs.Include(q => q.QuizOptions)
                    .Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted)
                    
                    .FirstOrDefault();
                if (item == null)
                {
                    response = APIContants<QuizViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                QuizViewModel quiz = Mapper.Map<QuizViewModel>(item);
               // int totalQuestionsCount = _context.Quizs.Where(a => a.AppTaskId == model.AppTaskId && !a.IsDeleted).Count();
               // quiz.IsLast = item.ItemOrder == totalQuestionsCount;
               // quiz.TotalQuestionsCount = totalQuestionsCount;

                response = APIContants<QuizViewModel>.CostumSuccessResult(quiz, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<QuizViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("GetAll")]
        public ResponseQuiz<PagedContent<IPagedList<QuizViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            ResponseQuiz<PagedContent<IPagedList<QuizViewModel>>> response = new ResponseQuiz<PagedContent<IPagedList<QuizViewModel>>>();
            try
            {
                // get the customer who request 
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                
                bool IsDone = false;
                foreach (var ct in customer.CustomerTasks)
                {
                    if (ct.AppTaskId == pagination.StatusId && ct.IsDone)
                    {
                        IsDone = true;
                        break;
                    }
                }
                if (IsDone)
                {
                   
                    response = APIContants<PagedContent<IPagedList<QuizViewModel>>>.CostumSometingQuizWrong(_localizer["NotFound"], null );
                    return response;
                }

                //var item = _context.Quizs.Include(q => q.QuizOptions)
                //   .Where(a => a.AppTaskId == pagination.StatusId && !a.IsDeleted);
                
                
                //NotDeleted
                var data = (from temp in _context.Quizs.Include(q => q.QuizOptions)
                            .Where(a => !a.IsDeleted)
                    //within limit :  the count of all users who done the task smaller that task limit
                    .Where(a => a.AppTaskId == pagination.StatusId).OrderBy(b => b.ItemOrder)
                            select temp).ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                List<QuizViewModel> QuizModels = new List<QuizViewModel>();

                foreach (var item in data)
                {
                    QuizViewModel QuizModel = Mapper.Map<QuizViewModel>(item);
                    var optionsModel = _context.QuizOptions.Where(a => !a.IsDeleted).Where(b => b.QuizId == item.Id).OrderBy(a => a.ItemOrder);
                    var options = Mapper.Map <List < QuizOptionViewModel> >(optionsModel);
                    QuizModel.QuizOptions = options;
                    QuizModels.Add(QuizModel);
                }

                int TotalQuestion = QuizModels.Count;
                var indexes = _context.QuizIndexs.Where(a => a.AppTaskId == pagination.StatusId).Where(b => b.CustomerId == customer.Id).FirstOrDefault();

                var Finalitems = QuizModels.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
              //  var tasks = Mapper.Map<IPagedList<QuizViewModel>>(Finalitems);
                PagedContent<IPagedList<QuizViewModel>> pagedContent = new PagedContent<IPagedList<QuizViewModel>>();
                pagedContent.content = Finalitems;
                pagedContent.pagination = new Pagination(Finalitems.TotalItemCount, Finalitems.PageSize, Finalitems.PageCount, Finalitems.PageNumber);
                response = APIContants<PagedContent<IPagedList<QuizViewModel>>>.CostumSuccessQuizResult(pagedContent, customer , indexes , TotalQuestion);
                return response;





               
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<QuizViewModel>>>.CostumSometingQuizWrong(_localizer["SomethingWentWrong"], null );
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        //[HttpPost]
        //[Route("GetPage")]
        //public Response<PagedContent<IPagedList<QuizPageViewModel>>> GetPage([FromBody] PaginationViewModel pagination)
        //{
        //    Response<PagedContent<IPagedList<QuizPageViewModel>>> response = new Response<PagedContent<IPagedList<QuizPageViewModel>>>();
        //    try
        //    {
        //        // get the customer who request 
        //        var username = User.Identity.Name;
        //        var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

        //        bool IsDone = false;
        //        foreach (var ct in customer.CustomerTasks)
        //        {
        //            if (ct.AppTaskId == pagination.StatusId && ct.IsDone)
        //            {
        //                IsDone = true;
        //                break;
        //            }
        //        }
        //        if (IsDone)
        //        {

        //            response = APIContants<PagedContent<IPagedList<QuizPageViewModel>>>.CostumSometingWrong(_localizer["NotFound"], null);
        //            return response;
        //        }


        //        //var item = _context.Quizs.Include(q => q.QuizOptions)
        //        //   .Where(a => a.AppTaskId == pagination.StatusId && !a.IsDeleted);
        //        QuizPageViewModel quizPageViewModel = new QuizPageViewModel();

        //        //NotDeleted
        //        var data = (from temp in _context.Quizs.Include(q => q.QuizOptions)
        //                    .Where(a => !a.IsDeleted)
        //            //within limit :  the count of all users who done the task smaller that task limit
        //            .Where(a => a.AppTaskId == pagination.StatusId).OrderBy(b => b.ItemOrder)
        //                    select temp).ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

        //        List<QuizViewModel> QuizModels = new List<QuizViewModel>();
        //        int i = 0;
        //        foreach (var item in data)
        //        {
        //            QuizViewModel QuizModel = Mapper.Map<QuizViewModel>(item);
        //            var optionsModel = _context.QuizOptions.Where(a => !a.IsDeleted).Where(b => b.QuizId == item.Id).OrderBy(a => a.ItemOrder);
        //            var options = Mapper.Map<List<QuizOptionViewModel>>(optionsModel);
        //            QuizModel.QuizOptions = options;
        //            QuizModels.Add(QuizModel);
        //            i++;
        //        }

        //        quizPageViewModel.Quizes = Mapper.Map<List<QuizViewModel>>(QuizModels);
        //        var index = _context.QuizIndexs.Where(a => a.AppTaskId == pagination.StatusId).Where(b => b.CustomerId == customer.Id).FirstOrDefault();
        //        quizPageViewModel.Indexes = Mapper.Map<QuizIndexViewModel>(index);

        //        quizPageViewModel.TotalQuestionsCount =i;
                
        //       // var Finalitems = QuizModels.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
        //        var Finalitems = quizPageViewModel.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
        //        //  var tasks = Mapper.Map<IPagedList<QuizViewModel>>(Finalitems);
        //        PagedContent<IPagedList<QuizViewModel>> pagedContent = new PagedContent<IPagedList<QuizViewModel>>();
        //        pagedContent.content = Finalitems;
        //        pagedContent.pagination = new Pagination(Finalitems.TotalItemCount, Finalitems.PageSize, Finalitems.PageCount, Finalitems.PageNumber);
        //      //  response = APIContants<PagedContent<IPagedList<QuizViewModel>>>.CostumSuccessResult(pagedContent, customer);
        //        response = APIContants<PagedContent<IPagedList<QuizPageViewModel>>>.CostumSuccessResult(pagedContent, customer);
        //        return response;





        //        //foreach (var at in itemsModel)
        //        //{
        //        //    var option = _context.QuizOptions.Where(a => !a.IsDeleted).Where(b => b.QuizId == at.Id).ToList();
        //        //    at. = Mapper.Map<QuizOptionViewModel>(option);
        //        //}


        //        //var dataModel = Mapper.Map<IPagedList<QuizViewModel>>(data);
        //        //var items = dataModel.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

        //        //var options = new List<QuizOption>();
        //        //foreach (var at in items)
        //        //{
        //        //    var option = _context.QuizOptions.Where(a => !a.IsDeleted).Where(b => b.QuizId == at.Id).ToList();
        //        //   at.QuestionOptions = Mapper.Map<QuizOptionViewModel>(option);
        //        //}



        //        //if (pagination.TaskTypeId != null)
        //        //{
        //        //    data = data.Where(a => a.TaskTypeId == pagination.TaskTypeId);
        //        //}

        //        //var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

        //        ////within time : the tasks that acheive the time constrains 
        //        //items = items.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks)
        //        //                        .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

        //        //var tasks = Mapper.Map<IPagedList<AppTaskViewModel>>(items);
        //        //PagedContent<IPagedList<AppTaskViewModel>> pagedContent = new PagedContent<IPagedList<AppTaskViewModel>>();
        //        //pagedContent.content = tasks;
        //        //pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
        //        //response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
        //        //return response;
        //    }
        //    catch (Exception e)
        //    {
        //        response = APIContants<PagedContent<IPagedList<QuizViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
        //        Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
        //        return response;
        //    }
        //}



        //[HttpPost]
        //[Route("Add")]
        //public async Task<Response<QuestionViewModel>> Add([FromBody] CustomerAnswerViewModel model)
        //{
        //    Response<QuestionViewModel> response = new Response<QuestionViewModel>();
        //    try
        //    {
        //        CustomerAnswer customerAnswer = Mapper.Map<CustomerAnswer>(model);
        //        var username = User.Identity.Name;
        //        var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
        //        customerAnswer.CustomerId = customer.Id;

        //        CustomerAnswer oldAnswer = _context.CustomerAnswers.Where(c => c.CustomerId == customer.Id
        //                                && c.QuestionId == model.QuestionId).FirstOrDefault();
        //        var item = _context.Questions.Include(q => q.QuestionOptions).Include(q => q.CustomerAnswers)
        //            .Include(q => q.AppTask)
        //            .Where(a => a.Id == model.QuestionId && !a.IsDeleted)
        //            .FirstOrDefault();

        //        if (oldAnswer == null)
        //        {
        //            if (!item.Type)
        //            {
        //                customerAnswer.QuestionOptionId = null;
        //                customerAnswer.QuestionOption = null;
        //                customerAnswer.IsRightAnswer = true;
        //            }
        //            else
        //            {
        //                QuestionOption questionOption = _context.QuestionOptions.Find(customerAnswer.QuestionOptionId);
        //                if(questionOption != null)
        //                {
        //                    if(questionOption.IsRightOption)
        //                        customerAnswer.IsRightAnswer = true;
        //                    else
        //                        customerAnswer.IsRightAnswer = false;
        //                }
        //                else
        //                {
        //                    customerAnswer.IsRightAnswer = false;
        //                }
        //            }
        //            _context.CustomerAnswers.Add(customerAnswer);
        //        }
        //        else
        //        {
        //            if (!item.Type)
        //            {
        //                oldAnswer.QuestionOptionId = null;
        //                oldAnswer.QuestionOption = null;
        //                oldAnswer.Answer = customerAnswer.Answer;
        //            }
        //            else
        //            {
        //                oldAnswer.QuestionOptionId = customerAnswer.QuestionOptionId;
        //                QuestionOption questionOption = _context.QuestionOptions.Find(customerAnswer.QuestionOptionId);
        //                if (questionOption != null)
        //                {
        //                    if (questionOption.IsRightOption)
        //                        customerAnswer.IsRightAnswer = true;
        //                    else
        //                        customerAnswer.IsRightAnswer = false;
        //                }
        //                else
        //                {
        //                    customerAnswer.IsRightAnswer = false;
        //                }
        //            }
        //            _context.CustomerAnswers.Update(oldAnswer);
        //        }
        //        await _context.SaveChangesAsync();

        //        if (item == null)
        //        {
        //            response = APIContants<QuestionViewModel>.CostumNotFound(_localizer["NotFound"], null);
        //            Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
        //            return response;
        //        }

        //        int totalQuestionsCount = _context.Questions.Where(a => a.AppTaskId == item.AppTaskId && !a.IsDeleted).Count();
        //        bool IsAllTrue = true;
        //        if (item.ItemOrder == totalQuestionsCount)
        //        {
        //            List<CustomerAnswer> customerAnswers = _context.CustomerAnswers.Where(c => c.CustomerId == customer.Id
        //                                && c.Question.AppTaskId == item.AppTaskId).ToList();
        //            foreach (var ca in customerAnswers)
        //            {
        //                if (!ca.IsRightAnswer)
        //                {
        //                    IsAllTrue = false;
        //                    break;
        //                }
        //            }
        //            CustomerTask customerTask = new CustomerTask();
        //            if (IsAllTrue)
        //            {
        //                customerTask.EarnedPoints = item.AppTask.Points;

        //                customer.CurrentPoints = customer.CurrentPoints + item.AppTask.Points;
        //                customer.TotalPoints = customer.TotalPoints + item.AppTask.Points;
        //                int nextGroup = customer.Group.ItemOrder + 1;
        //                Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
        //                if (group != null)
        //                {
        //                    if (customer.TotalPoints >= group.Points)
        //                    {
        //                        customer.GroupId = group.Id;
        //                        customer.Group = group;
        //                        int nextNextGroupOrder = nextGroup + 1;
        //                        Group nextNextGroup = _context.Groups.Where(g => g.ItemOrder == nextNextGroupOrder).FirstOrDefault();
        //                        if (group != null)
        //                        {
        //                            customer.NextGroupPoints = nextNextGroup.Points;
        //                        }
        //                        else
        //                        {
        //                            customer.NextGroupPoints = 0;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        customer.NextGroupPoints = group.Points;
        //                    }
        //                }
        //                else
        //                {
        //                    customer.NextGroupPoints = 0;
        //                }
        //                _context.Customers.Update(customer);
        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                customerTask.EarnedPoints = 0;
        //            }

        //            customerTask.AppTaskId = item.AppTaskId;
        //            customerTask.CustomerId = customer.Id;
        //            customerTask.DoneDate = DateTime.Now;
        //            customerTask.IsDone = true;
        //            _context.CustomerTasks.Add(customerTask);
        //            await _context.SaveChangesAsync();
        //        }

        //        QuestionViewModel question = Mapper.Map<QuestionViewModel>(item);
        //        question.IsLast = item.ItemOrder == totalQuestionsCount;
        //        question.TotalQuestionsCount = totalQuestionsCount;
        //        question.IsAllRight = IsAllTrue;
        //        response = APIContants<QuestionViewModel>.CostumSuccessResult(question, customer);
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        response = APIContants<QuestionViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
        //        Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
        //        return response;
        //    }
        //}


        [HttpPost]
        [Route("Pause")]
        public async Task<Response<CustomerTaskViewModel>> Pause([FromBody] QuizPauseModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                var appTask = _context.AppTasks.Where(c => !c.IsDeleted).Where(a => a.Id == model.AppTaskId).FirstOrDefault();
                int TotalPoints = 0;
                if (model.Points > appTask.Points)
                {
                    TotalPoints = 0;
                }
                else {
                    TotalPoints = model.Points;
                }
                customer.CurrentPoints = customer.CurrentPoints + TotalPoints;
                customer.TotalPoints = customer.TotalPoints + TotalPoints;
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
                        if (nextNextGroup != null)
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

                


                QuizIndex quizIndex = _context.QuizIndexs.Where(c => c.CustomerId == customer.Id
                && c.AppTaskId == model.AppTaskId).FirstOrDefault();
                if (quizIndex == null)
                {
                    quizIndex = new QuizIndex();
                    quizIndex.AppTaskId = model.AppTaskId;
                    quizIndex.CustomerId = customer.Id;
                    quizIndex.Index = model.Index;
                    quizIndex.Created = DateTime.Now;
                    quizIndex.Modified = DateTime.Now;
                   
                    _context.QuizIndexs.Add(quizIndex);
                    await _context.SaveChangesAsync();
                }


                response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(null, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddEnd")]
        public async Task<Response<CustomerTaskViewModel>> AddEnd([FromBody] CustomerTaskViewModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                CustomerTask customerTask = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id
                && c.AppTaskId == model.AppTaskId).FirstOrDefault();



                if (customerTask == null)
                {
                    customerTask = new CustomerTask();
                    customerTask.CustomerId = customer.Id;
                    customerTask.AppTaskId = model.AppTaskId;
                    customerTask.DoneDate = DateTime.Now; ;
                    customerTask.StartDate = null;
                    customerTask.Description = model.Description;
                    customerTask.IsDone = true;
                    customerTask.EarnedPoints = customerTask.AppTask.Points;
                    _context.CustomerTasks.Add(customerTask);
                    await _context.SaveChangesAsync();
                }

                else if (customerTask.IsDone == true)
                {
                    response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                    return response;
                }

                response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(Mapper.Map<CustomerTaskViewModel>(customerTask), customer);
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
