using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using System.Collections.Generic;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppTasksController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppTasksController(OpenSismDBContext context, UserManager<ApplicationUser> userManager,
            IStringLocalizer<BaseController> localizer) : base(localizer)
        {
            _context = context;
            _userManager = userManager;
        }
        /// <summary>
        /// returns all task not deleted or finished by date or by user limits , and tasks for user group 
        /// so it returns all task suitable to the customer 
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        // GET: api/AppTasks
        [HttpPost]
        [Route("GetAll")]
        public Response<PagedContent<IPagedList<AppTaskViewModel>>> GetAll([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<AppTaskViewModel>>> response = new Response<PagedContent<IPagedList<AppTaskViewModel>>>();
            try
            {
                // get the customer who request 
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                //NotDeleted
                var data = (from temp in _context.AppTasks.Where(a => !a.IsDeleted)
                    //within limit :  the count of all users who done the task smaller that task limit
                    .Where(a => a.CustomerTasks.Count() < a.Limit)
                    //customer group : the task is from customer group
                    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                    .OrderByDescending(a => a.Modified)
                            select temp);

                if (pagination.TaskTypeId != null)
                {
                    data = data.Where(a => a.TaskTypeId == pagination.TaskTypeId);
                }

                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                //within time : the tasks that acheive the time constrains 
                items = items.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks)
                                        .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var tasks = Mapper.Map<IPagedList<AppTaskViewModel>>(items);
                PagedContent<IPagedList<AppTaskViewModel>> pagedContent = new PagedContent<IPagedList<AppTaskViewModel>>();
                pagedContent.content = tasks;
                pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        // returns the active task of customers that is nor done 
        [HttpPost]
        [Route("GetActiveCustomerTask")]
        public Response<PagedContent<IPagedList<AppTaskViewModel>>> GetActiveCustomerTask([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<AppTaskViewModel>>> response = new Response<PagedContent<IPagedList<AppTaskViewModel>>>();
            try
            {
                // get the customer who request 
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                //NotDeleted
                var data = (from temp in _context.AppTasks.Where(a => !a.IsDeleted)
                    //within limit :  the count of all users who done the task smaller that task limit
                    .Where(a => a.CustomerTasks.Count() < a.Limit)
                    //customer group : the task is from customer group
                    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                    .OrderByDescending(a => a.Modified)
                            select temp);

                if (pagination.TaskTypeId != null)
                {
                    data = data.Where(a => a.TaskTypeId == pagination.TaskTypeId);
                }
                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var tempTasks =  new List<AppTaskViewModel>();
                
                items = items.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks)
                                        .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
               
                var finished = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id && c.IsDone
                && !c.IsDeleted).Select(c => c.AppTask).ToList();
                //get tasks list which is not done 
                
                    foreach (var at in items)
                    {
                        bool isDone = false;
                        foreach (var ct in finished)
                        {
                            if (at.Id == ct.Id)
                            {
                                isDone = true;
                                finished.Remove(ct);
                                break;
                            }
                        }
                        if (!isDone)
                        {

                            tempTasks.Add(Mapper.Map<AppTaskViewModel>(at));
                        }
                    }

                AppTask shareAppTask = _context.AppTasks.Where(a => !a.IsDeleted)
                    .Where(a => a.CustomerTasks.Where(c => c.AppTask.TaskType.Name == "share_games_app").Count() < a.Limit)
                    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                    .Where(a => a.TaskType.Name == "share_games_app").FirstOrDefault();

                if (shareAppTask != null)
                {
                    tempTasks.Add(Mapper.Map<AppTaskViewModel>(shareAppTask));
                }


                var tasks = tempTasks.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                PagedContent<IPagedList<AppTaskViewModel>> pagedContent = new PagedContent<IPagedList<AppTaskViewModel>>();
                pagedContent.content = tasks;
                pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }


        [HttpPost]
        [Route("GetActiveCustomerTask_V2")]
        public Response<PagedContent<IPagedList<AppTaskViewModel>>> GetActiveCustomerTask_V2([FromBody] PaginationViewModel pagination) 
        {
            Response<PagedContent<IPagedList<AppTaskViewModel>>> response = new Response<PagedContent<IPagedList<AppTaskViewModel>>>();
            try
            {
                
                // get the customer who request 
                var username = User.Identity.Name;
               
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();

                //var TaskTemp =  (from temp in _context.AppTasks.Where(a => !a.IsDeleted).Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                //    .OrderByDescending(a => a.Modified)
                //                 select temp);


                var TaskTemp = _context.AppTasks.Where(a => !a.IsDeleted).Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                   .OrderByDescending(a => a.Modified);

                //within limit :  the count of all users who done the task smaller that task limit

                //customer group : the task is from customer group


                //var CustomerTaskTemp = (from tempCustomers in _context.CustomerTasks.Where(a => !a.IsDeleted)
                //                              select tempCustomers);
                var CustomerTaskTemp = _context.CustomerTasks.Where(a => !a.IsDeleted);
                                              

                var data = new List<AppTask>();
                
                foreach (var at in TaskTemp)
                {
                    int counter = 0;

                    foreach (var ct in CustomerTaskTemp)
                    {
                        if ((at.Id == ct.AppTaskId) && (ct.IsDone))
                        {
                            counter++;
                           
                           // break;
                        }
                    }
                    if (at.Limit < counter)
                    {

                        data.Add(at);
                       
                    }
                }
                

                //NotDeleted
                //var data = (from temp in _context.AppTasks.Where(a => !a.IsDeleted)
                //    //within limit :  the count of all users who done the task smaller that task limit
                //    .Where(a => a.CustomerTasks.Count() < a.Limit)
                //    //customer group : the task is from customer group
                //    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                //    .OrderByDescending(a => a.Modified)
                //            select temp);
                

                if (pagination.TaskTypeId != null)
                {
                   // data.Add
                    data = (List<AppTask>)data.Where(a => a.TaskTypeId == pagination.TaskTypeId);
                }
                var items = data.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                var tempTasks = new List<AppTaskViewModel>();

                items = items.Where(a => a.StartDate.Ticks <= DateTime.Now.Ticks && a.EndDate.Ticks >= DateTime.Now.Ticks)
                                        .ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);

                var finished = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id && c.IsDone
                && !c.IsDeleted).Select(c => c.AppTask).ToList();
                //get tasks list which is not done 

                foreach (var at in items)
                {
                    bool isDone = false;
                    foreach (var ct in finished)
                    {
                        if (at.Id == ct.Id)
                        {
                            isDone = true;
                            finished.Remove(ct);
                            break;
                        }
                    }
                    if (!isDone)
                    {

                        tempTasks.Add(Mapper.Map<AppTaskViewModel>(at));
                    }
                }

                AppTask shareAppTask = _context.AppTasks.Where(a => !a.IsDeleted)
                    .Where(a => a.CustomerTasks.Where(c => c.AppTask.TaskType.Name == "share_games_app").Count() < a.Limit)
                    .Where(a => a.AppTaskGroups.Select(a => a.Group).Contains(customer.Group))
                    .Where(a => a.TaskType.Name == "share_games_app").FirstOrDefault();

                if (shareAppTask != null)
                {
                    tempTasks.Add(Mapper.Map<AppTaskViewModel>(shareAppTask));
                }


                var tasks = tempTasks.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                PagedContent<IPagedList<AppTaskViewModel>> pagedContent = new PagedContent<IPagedList<AppTaskViewModel>>();
                pagedContent.content = tasks;
                pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }


        // returns all finished customer Task
        [HttpPost]
        [Route("GetFinishCustomerTask")]
        public Response<PagedContent<IPagedList<AppTaskViewModel>>> GetFinishCustomerTask([FromBody] PaginationViewModel pagination)
        {
            Response<PagedContent<IPagedList<AppTaskViewModel>>> response = new Response<PagedContent<IPagedList<AppTaskViewModel>>>();
            try
            {
                // get the customer who request 
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();  
                var tempTasks = new List<AppTaskViewModel>();
                var  finished = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id && c.IsDone
                && !c.IsDeleted).Select(c => c.AppTask).ToList();
                int i = 0;
                foreach (var at in finished)
                {
                            tempTasks.Add(Mapper.Map<AppTaskViewModel>(at ));
                  //  tempTasks[i].IsDone = true;
                }
                foreach (var at in tempTasks)
                {
                    
                      at.IsDone = true;
                }
              
                var tasks = tempTasks.ToPagedList(pageNumber: pagination.Page, pageSize: pagination.Limit);
                PagedContent<IPagedList<AppTaskViewModel>> pagedContent = new PagedContent<IPagedList<AppTaskViewModel>>();
                pagedContent.content = tasks;
                pagedContent.pagination = new Pagination(tasks.TotalItemCount, tasks.PageSize, tasks.PageCount, tasks.PageNumber);
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSuccessResult(pagedContent, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<PagedContent<IPagedList<AppTaskViewModel>>>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }





        /// <summary>
        /// return Single task by task ID with isReachlimit or not 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // GET: api/AppTasks/5
        [HttpPost]
        [Route("Get")]
        public Response<AppTaskViewModel> Get([FromBody] AppTaskViewModel model)
        {
            Response<AppTaskViewModel> response = new Response<AppTaskViewModel>();
            try
            {
                // get the customer data 
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                // get the unique task by ID 
                var item = _context.AppTasks.Where(a => a.Id == model.Id && !a.IsDeleted).FirstOrDefault();
                
                if (item == null)
                {
                    response = APIContants<AppTaskViewModel>.CostumNotFound(_localizer["NotFound"], null);
                    Serilog.Log.Warning("{@AddressId}, {@RequestId}, {@Response}", model.Id, CustomFilterAttribute.RequestId, response);
                    return response;
                }
                var appTask = Mapper.Map<AppTaskViewModel>(item);
                appTask.IsDone = false;
                //check if the customer started the task before 
                item.CustomerTasks = item.CustomerTasks.Where(c => c.CustomerId == customer.Id)
                    .Where(c => c.AppTask.TaskType.Name != "share_games_app").ToList();
                //check if the customer done the task before 
                if (item.CustomerTasks.Count() > 0)
                {
                    foreach (var ct in item.CustomerTasks)
                    {
                        if (ct.IsDone)
                        {
                            appTask.IsDone = true;
                            break;
                        }
                    }
                }
                // if the Id of  get request is for share games app task , 
                // it has a special treatment because the user can do it many times 
                if(item.TaskType.Name == "share_games_app")
                {
                    int count = _context.CustomerTasks.Where(c => c.CustomerId == customer.Id)
                        .Where(c => c.IsDone && c.AppTaskId == item.Id).Count();
                    if (count < item.Limit)
                        appTask.IsReachLimit = false;
                    else
                        appTask.IsReachLimit = true;
                }
                else
                {
                    int count = _context.CustomerTasks.Where(c => c.IsDone && c.AppTaskId == item.Id).Count();
                    if (count < item.Limit)
                        appTask.IsReachLimit = false;
                    else
                        appTask.IsReachLimit = true;
                }
                response = APIContants<AppTaskViewModel>.CostumSuccessResult(appTask, customer);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<AppTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("AddStart")]
        public async Task<Response<CustomerTaskViewModel>> AddStart([FromBody] CustomerTaskViewModel model)
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
                    customerTask.DoneDate = null;
                    customerTask.StartDate = DateTime.Now;
                    customerTask.Description = model.Description;
                    customerTask.IsDone = false;

                    _context.CustomerTasks.Add(customerTask);
                    await _context.SaveChangesAsync();
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

                customerTask.DoneDate = DateTime.Now;
                customerTask.IsDone = true;
                customerTask.EarnedPoints = customerTask.AppTask.Points;
                customerTask.Description = model.Description;
                _context.CustomerTasks.Update(customerTask);
                await _context.SaveChangesAsync();

                customer.CurrentPoints = customer.CurrentPoints + customerTask.AppTask.Points;
                customer.TotalPoints = customer.TotalPoints + customerTask.AppTask.Points;
                int nextGroup = customer.Group.ItemOrder + 1;
                Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                if(group != null)
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


        [HttpPost]
        [Route("AddEnd_V2")]
        public async Task<Response<CustomerTaskViewModel>> AddEnd_V2([FromBody] CustomerTaskViewModel model)
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

                else if (customerTask.IsDone == true) {
                    response = APIContants<CustomerTaskViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                    return response;
                }



                customer.CurrentPoints = customer.CurrentPoints + customerTask.AppTask.Points;
                customer.TotalPoints = customer.TotalPoints + customerTask.AppTask.Points;
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

        [HttpPost]
        [Route("AddShareAppEnd")]
        public async Task<Response<CustomerTaskViewModel>> AddShareAppEnd([FromBody] CustomerTaskViewModel model)
        {
            Response<CustomerTaskViewModel> response = new Response<CustomerTaskViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                if(customer.InstalledFrom == null || customer.InstalledFrom == "")
                {
                    customer.InstalledFrom = model.ShareCode;
                    _context.Customers.Update(customer);
                    await _context.SaveChangesAsync();

                    Customer otherCustomer = _context.Customers.Where(c => c.ShareCode == model.ShareCode).FirstOrDefault();
                    AppTask appTask = _context.AppTasks.Where(a => a.TaskType.Name == "share_games_app").FirstOrDefault();

                    CustomerTask customerTask = new CustomerTask();
                    customerTask.AppTaskId = appTask.Id;
                    customerTask.CustomerId = otherCustomer.Id;
                    customerTask.DoneDate = DateTime.Now;
                    customerTask.IsDone = true;
                    customerTask.StartDate = DateTime.Now;
                    customerTask.EarnedPoints = appTask.Points;

                    _context.CustomerTasks.Add(customerTask);
                    await _context.SaveChangesAsync();

                    otherCustomer.CurrentPoints = otherCustomer.CurrentPoints + appTask.Points;
                    otherCustomer.TotalPoints = otherCustomer.TotalPoints + appTask.Points;

                    int nextGroup = otherCustomer.Group.ItemOrder + 1;
                    Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                    if (group != null)
                    {
                        if (otherCustomer.TotalPoints >= group.Points)
                        {
                            otherCustomer.GroupId = group.Id;
                            otherCustomer.Group = group;
                            int nextNextGroupOrder = nextGroup + 1;
                            Group nextNextGroup = _context.Groups.Where(g => g.ItemOrder == nextNextGroupOrder).FirstOrDefault();
                            if (nextNextGroup != null)
                            {
                                otherCustomer.NextGroupPoints = nextNextGroup.Points;
                            }
                            else
                            {
                                otherCustomer.NextGroupPoints = 0;
                            }
                        }
                        else
                        {
                            otherCustomer.NextGroupPoints = group.Points;
                        }
                    }
                    else
                    {
                        otherCustomer.NextGroupPoints = 0;
                    }
                    _context.Customers.Update(otherCustomer);
                    await _context.SaveChangesAsync();

                    response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(Mapper.Map<CustomerTaskViewModel>(customerTask), customer);
                    return response;
                }
                else
                {
                    response = APIContants<CustomerTaskViewModel>.CostumSuccessResult(null, customer);
                    return response;
                }
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
