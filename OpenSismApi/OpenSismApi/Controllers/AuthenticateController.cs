using OpenSismApi.AppStart;
using OpenSismApi.Helpers;
using OpenSismApi.Models;
using OpenSismApi.TwilioServices;
using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace OpenSismApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly OpenSismDBContext _context;
        private readonly IVerification _verificationService;
        private readonly IHostingEnvironment _hostingEnvironment;
       
        public AuthenticateController(OpenSismDBContext context, UserManager<ApplicationUser> userManager,
            IConfiguration configuration, 
            IStringLocalizer<BaseController> localizer,
            IHostingEnvironment hostingEnvironment ) : base(localizer)
            
        {
            this.userManager = userManager;
            _configuration = configuration;
            _context = context;
           // _verificationService = verificationService;
            _hostingEnvironment = hostingEnvironment;
            
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginModel model)
        {
            // Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                // user is valid do whatever you want
                //var authclaims = new list<claim>
                //        {
                //            new claim(claimtypes.name, user.username),
                //            new claim(jwtregisteredclaimnames.jti, guid.newguid().tostring()),
                //        };

                //var authsigningkey = new symmetricsecuritykey(encoding.utf8.getbytes(_configuration["jwt:secret"]));

                //var token = new jwtsecuritytoken(
                //    issuer: _configuration["jwt:validissuer"],
                //    audience: _configuration["jwt:validaudience"],
                //    expires: datetime.now.adddays(30),
                //    claims: authclaims,
                //    signingcredentials: new signingcredentials(authsigningkey, securityalgorithms.hmacsha256)
                //    );
               
               // var customer = _context.Customers.Where(c => c.userid == user.id).firstordefault();
                //customer.token = new jwtsecuritytokenhandler().writetoken(token);
                //customer.tokenexpiration = token.validto;
                //_context.update(customer);
                //await _context.savechangesasync();

                var refreshToken = generateRefreshToken();

                setTokenCookie(refreshToken.Token);
                var response = new AuthenticateResponse("", refreshToken.Token);
                return Ok(response);

            }
            else {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

         //   var response = _userService.Authenticate(model);

            
           

           
        }

        //[AllowAnonymous]
        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refreshToken"];
        //    // Response<CustomerViewModel> response = new Response<CustomerViewModel>();
        //   var user = await userManager.FindByNameAsync(model.Username);
        //  //  var customer = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        //    if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        // user is valid do whatever you want
        //        //var authclaims = new list<claim>
        //        //        {
        //        //            new claim(claimtypes.name, user.username),
        //        //            new claim(jwtregisteredclaimnames.jti, guid.newguid().tostring()),
        //        //        };

        //        //var authsigningkey = new symmetricsecuritykey(encoding.utf8.getbytes(_configuration["jwt:secret"]));

        //        //var token = new jwtsecuritytoken(
        //        //    issuer: _configuration["jwt:validissuer"],
        //        //    audience: _configuration["jwt:validaudience"],
        //        //    expires: datetime.now.adddays(30),
        //        //    claims: authclaims,
        //        //    signingcredentials: new signingcredentials(authsigningkey, securityalgorithms.hmacsha256)
        //        //    );
        //      //  var customer = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        //       // var customer = _context.Customers.Where(c => c.UserId == user.Id).FirstOrDefault();
        //        //customer.token = new jwtsecuritytokenhandler().writetoken(token);
        //        //customer.tokenexpiration = token.validto;
        //        //_context.update(customer);
        //        //await _context.savechangesasync();

        //        var _refreshToken = generateRefreshToken();

        //        setTokenCookie(_refreshToken.Token);
        //      //  var response = new AuthenticateResponse("", refreshToken.Token);
        //        return Ok(response);

        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Username or password is incorrect" });
        //    }

        //    //   var response = _userService.Authenticate(model);





        //}

        private RefreshToken generateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                   
                };
            }
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        //[AllowAnonymous]
        //[HttpPost("refresh-token")]
        //public IActionResult RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refreshToken"];
        //    var response = _userService.RefreshToken(refreshToken, ipAddress());

        //    if (response == null)
        //        return Unauthorized(new { message = "Invalid token" });

        //    setTokenCookie(response.RefreshToken);

        //    return Ok(response);
        //}




        [HttpPost]
        [Route("Login")]
        public async Task<Response<CustomerViewModel>> Login([FromBody] LoginModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Username);
                   
                    if (user == null)
                    {
                        return APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["WrongPhone"], null);
                    }
                    else
                    {
                        if (!await userManager.CheckPasswordAsync(user, model.Password))
                        {
                            return APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["WrongPassword"], null);
                        }
                        else
                        {
                            if (user.LockoutEnd > DateTime.UtcNow)
                            {
                                return APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["LockedAccount"], null);
                            }
                            var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                            var token = new JwtSecurityToken(
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudience"],
                                expires: DateTime.Now.AddDays(30),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                                );

                            var customer = _context.Customers.Where(c => c.UserId == user.Id).FirstOrDefault();
                            customer.Token = new JwtSecurityTokenHandler().WriteToken(token);
                            customer.TokenExpiration = token.ValidTo;
                            
                            customer.FCMToken = model.FCMToken;
                            //customer.LuckyWheelLastSpinDate = DateTime.Now.AddDays(-10);
                            //customer.DailyBonusLastUseDate = DateTime.Now.AddDays(-2);
                            //customer.DailyBonusLevel = 15;
                            _context.Update(customer);
                            await _context.SaveChangesAsync();
                            int nextGroup = customer.Group.ItemOrder + 1;
                            Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                            CustomerViewModel customerViewModel = Mapper.Map<CustomerViewModel>(customer);
                            if (group != null)
                            {
                                customerViewModel.NextGroupPoints = group.Points;
                            }
                            else
                            {
                                customerViewModel.NextGroupPoints = 0;
                            }
                            response = APIContants<CustomerViewModel>.CostumSuccessResult(customerViewModel , customer) ;
                            return response;
                        }
                    }
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<Response<CustomerViewModel>> Register([FromBody] RegisterModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var userExists = await userManager.FindByNameAsync(model.Phone);
                    if (userExists != null)
                        return APIContants<CustomerViewModel>.CostumUserExist(_localizer["UserExist"], null);

                    ApplicationUser user = new ApplicationUser()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Phone,
                        PhoneNumber = model.Phone,
                        Email = model.Email
                    };
                    /////////////////////////////////////////////////////////////////////////
                    user.PhoneNumberConfirmed = true; // added to test 
                    
                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };
                    
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(30),
                    //  expires: DateTime.Now.AddMinutes(2),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        ); ;


                    
                    ///////////////////////////////////////////////////////////////////////////////////
                    var result = await userManager.CreateAsync(user, model.Password);
                   
                    if (!result.Succeeded)
                    {
                        return null;
                       
                        response = APIContants<CustomerViewModel>.CostumSometingWrong(result.Errors.ElementAt(0).Description, null);
                        Serilog.Log.Fatal(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                        return response;
                    }
                    else
                    {
                        
                        Customer customer = new Customer();
                        customer.UserId = user.Id;
                        customer.GroupId = _context.Groups.Where(g => g.Name == "general").FirstOrDefault().Id;
                       // customer.CityId = model.CityId;
                        customer.CurrentPoints = 0;
                        customer.FirstName = model.FirstName;
                        customer.LastName = model.LastName;
                        customer.LockoutEnabled = false;
                        customer.LuckyWheelLastSpinDate = DateTime.Now.AddDays(-1);
                        customer.DailyBonusLastUseDate = DateTime.Now.AddDays(-1);
                        customer.LuckyWheelPremiumLastSpinDate = DateTime.Now.AddDays(-1);
                        customer.DailyBonusLevel = 1;
                        customer.TermsAndConditions = model.TermsAndConditions;
                        // customer.Token = "";
                        customer.Token = new JwtSecurityTokenHandler().WriteToken(token);
                        customer.FCMToken = model.FCMToken;
                        customer.TotalPoints = 0;
                        // customer.TokenExpiration = DateTime.Now;
                        customer.TokenExpiration = token.ValidTo;
                        customer.ImageUrl = "";
                        customer.Premium = false;
                        customer.Gender = model.Gender;
                        customer.ShareCode = RandomString(10);
                        _context.Add(customer);
                        
                        await _context.SaveChangesAsync();
                        response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer) , customer);
                        return response;
                        //var verification = await _verificationService.StartVerificationAsync(user.PhoneNumber, "sms");
                        //if (verification.IsValid)
                        //{
                        //    response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer));
                        //    return response;
                        //}
                        //else
                        //{
                        //    response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                        //    Serilog.Log.Fatal(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                        //    return response;
                        //}
                    }
                }
                else
                {
                   // return null;
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("Verify")]
        public async Task<Response<CustomerViewModel>> Verify([FromBody] VerifyModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = _context.Customers.Include(c => c.User).Where(c => c.Id == model.Id).FirstOrDefault();
                    if (customer == null)
                    {
                        response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                        Serilog.Log.Fatal("Customer Id doesnot exist", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                        return response;
                    }
                    else
                    {
                        var user = await userManager.FindByNameAsync(customer.User.UserName);
                        if (user == null)
                        {
                            response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                            Serilog.Log.Fatal("Username doesnot exist", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                        else
                        {
                            var result = await _verificationService.CheckVerificationAsync(user.PhoneNumber, model.Code);
                            if (result.IsValid)
                            {
                                user.PhoneNumberConfirmed = true;
                                await userManager.UpdateAsync(user);
                                var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                                var token = new JwtSecurityToken(
                                    issuer: _configuration["JWT:ValidIssuer"],
                                    audience: _configuration["JWT:ValidAudience"],
                                    expires: DateTime.Now.AddDays(30),
                                    claims: authClaims,
                                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                                    );

                                customer.Token = new JwtSecurityTokenHandler().WriteToken(token);
                                customer.TokenExpiration = token.ValidTo;
                                _context.Update(customer);
                                await _context.SaveChangesAsync();
                                response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer));
                                return response;
                            }
                            response = APIContants<CustomerViewModel>.CostumIncorectCode(_localizer["WrongCode"], null);
                            Serilog.Log.Fatal(_localizer["WrongCode"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                    }
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("ResendVerifyCode")]
        public async Task<Response<CustomerViewModel>> ResendVerifyCode([FromBody] ResendCodeModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = _context.Customers.Include(c => c.User).Where(c => c.User.UserName == model.Username).FirstOrDefault();
                    if (customer == null)
                    {
                        response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                        Serilog.Log.Fatal("Customer Id doesnot exist", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                        return response;
                    }
                    else
                    {
                        var user = await userManager.FindByNameAsync(customer.User.UserName);
                        if (user == null)
                        {
                            response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                            Serilog.Log.Fatal("Username doesnot exist", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                        else
                        {
                            var verification = await _verificationService.StartVerificationAsync(user.PhoneNumber, "sms");
                            if (verification.IsValid)
                            {
                                response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer));
                                return response;
                            }
                            else
                            {
                                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                                Serilog.Log.Fatal(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                                return response;
                            }
                        }
                    }
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("UpdateProfile")]
        public async Task<Response<CustomerViewModel>> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var username = User.Identity.Name;
                    var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                    //customer.CityId = model.CityId;
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    customer.Gender = model.Gender;
                    customer.Address = model.Address;
                    customer.User.Email = model.Email;
                    if (model.ImageUrl != null || model.ImageUrl != "")
                    {
                        customer.ImageUrl = model.ImageUrl;
                    }

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    int nextGroup = customer.Group.ItemOrder + 1;
                    Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                    CustomerViewModel customerViewModel = Mapper.Map<CustomerViewModel>(customer);
                    if (group != null)
                    {
                        customerViewModel.NextGroupPoints = group.Points;
                    }
                    else
                    {
                        customerViewModel.NextGroupPoints = 0;
                    }
                    response = APIContants<CustomerViewModel>.CostumSuccessResult(customerViewModel); return response;
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }

        }

        [HttpPost]
        [Authorize]
        [Route("GetProfile")]
        public Response<CustomerViewModel> GetProfile([FromBody] UpdateProfileModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                var username = User.Identity.Name;
                var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                int nextGroup = customer.Group.ItemOrder + 1;
                Group group = _context.Groups.Where(g => g.ItemOrder == nextGroup).FirstOrDefault();
                CustomerViewModel customerViewModel = Mapper.Map<CustomerViewModel>(customer);
                if (group != null)
                {
                    customerViewModel.NextGroupPoints = group.Points;
                }
                else
                {
                    customerViewModel.NextGroupPoints = 0;
                }
                response = APIContants<CustomerViewModel>.CostumSuccessResult(customerViewModel);
                return response;
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }

        }

        [HttpPost]
        [Authorize]
        [Route("SetFCMToken")]
        public async Task<Response<CustomerViewModel>> SetFCMToken([FromBody] UpdateFCMTokenModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var username = User.Identity.Name;
                    var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                    customer.FCMToken = model.FCMToken;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer) , customer);
                    return response;
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("SetFacebookData")]
        public async Task<Response<CustomerViewModel>> SetFacebookData([FromBody] CustomerViewModel model)
        {
            Response<CustomerViewModel> response = new Response<CustomerViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var username = User.Identity.Name;
                    var customer = _context.Customers.Where(c => c.User.UserName == username).FirstOrDefault();
                    customer.FacbookAccessToken = model.FacbookAccessToken;
                    customer.FacbookEmail = model.FacbookEmail;
                    customer.FacbookFirstName = model.FacbookFirstName;
                    customer.FacbookLastName = model.FacbookLastName;
                    customer.FacbookId = model.FacbookId;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer));
                    return response;
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<Response<ApplicationUserViewModel>> ChangePassword([FromBody] ChangePasswordModel model)
        {
            Response<ApplicationUserViewModel> response = new Response<ApplicationUserViewModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var username = User.Identity.Name;
                    var user = await userManager.FindByNameAsync(username);

                    if (!await userManager.CheckPasswordAsync(user, model.OldPassword))
                    {
                        return APIContants<ApplicationUserViewModel>.CostumSometingWrong(_localizer["WrongPassword"], null);
                    }
                    else
                    {
                        var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            foreach (var error in changePasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            response = APIContants<ApplicationUserViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                            Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                        response = APIContants<ApplicationUserViewModel>.CostumSuccessResult(Mapper.Map<ApplicationUserViewModel>(user));
                        return response;
                    }
                }
                else
                {
                    response = APIContants<ApplicationUserViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<ApplicationUserViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }

        }


        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<Response<string>> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            Response<string> response = new Response<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Username);
                    if (user == null)
                    {
                        return APIContants<string>.CostumSometingWrong(_localizer["WrongPhone"], null);
                    }
                    else
                    {
                        if (user.LockoutEnd > DateTime.UtcNow)
                        {
                            return APIContants<string>.CostumSometingWrong(_localizer["LockedAccount"], null);
                        }

                        var code = await userManager.GeneratePasswordResetTokenAsync(user);
                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var customer = _context.Customers.Where(c => c.UserId == user.Id).FirstOrDefault();
                        customer.ResetCode = code;
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                        var verification = await _verificationService.StartVerificationAsync(user.PhoneNumber, "sms");
                        if (verification.IsValid)
                        {
                            response = APIContants<string>.CostumSuccessResult(_localizer["ResetCodeSent"]);
                            return response;
                        }
                        else
                        {
                            response = APIContants<string>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                            Serilog.Log.Fatal(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                    }
                }
                else
                {
                    response = APIContants<string>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<string>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<Response<string>> ResetPassword([FromBody] ResetPasswordModel model)
        {
            Response<string> response = new Response<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Username);
                    if (user == null)
                    {
                        return APIContants<string>.CostumSometingWrong(_localizer["WrongPhone"], null);
                    }
                    else
                    {
                        var customer = _context.Customers.Include(c => c.User).Where(c => c.UserId == user.Id).FirstOrDefault();
                        if (customer == null)
                        {
                            response = APIContants<string>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                            Serilog.Log.Fatal("Customer Id doesnot exist", "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                        else
                        {
                            var result = await _verificationService.CheckVerificationAsync(user.PhoneNumber, model.Code);
                            if (result.IsValid)
                            {
                                var changeResult = await userManager.ResetPasswordAsync(user, customer.ResetCode, model.Password);
                                if (changeResult.Succeeded)
                                {
                                    response = APIContants<string>.CostumSuccessResult(_localizer["PasswordChanged"]);
                                    return response;
                                }
                            }
                            response = APIContants<string>.CostumIncorectCode(_localizer["WrongCode"], null);
                            Serilog.Log.Fatal(_localizer["WrongCode"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                            return response;
                        }
                    }
                }
                else
                {
                    response = APIContants<string>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception e)
            {
                response = APIContants<string>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Fatal(e, "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("SaveMedia")]
        [AllowAnonymous]
        public Response<CustomerViewModel> SaveMedia()
        {
            Response<CustomerViewModel> response;
            try
            {
                var file = Request.Form.Files[0];
                if (file != null)
                {
                    Customer customer = new Customer();
                    FileInfo fi = new FileInfo(file.FileName);
                    var newFilename = "P" + customer.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\customers\" + newFilename);
                    var pathToSave = @"/images/customers/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    customer.ImageUrl = pathToSave;
                    response = APIContants<CustomerViewModel>.CostumSuccessResult(Mapper.Map<CustomerViewModel>(customer));
                    return response;
                }
                else
                {
                    response = APIContants<CustomerViewModel>.CostumSometingWrong(ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, null);
                    Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = APIContants<CustomerViewModel>.CostumSometingWrong(_localizer["SomethingWentWrong"], null);
                Serilog.Log.Warning(_localizer["SomethingWentWrong"], "{@RequestId}, {@Response}", CustomFilterAttribute.RequestId, response);
                return response;
            }
        }

        private static Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            Customer c = _context.Customers.Where(c => c.ShareCode == code).FirstOrDefault();
            if(c != null)
            {
                return RandomString(length);
            }
            else
            {
                return code;
            }
        }
    }
}
