using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Application_Scheduler.Data;
using Application_Scheduler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Application_Scheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppointmentSchedulerDbContext _context;
        private readonly IConfiguration configuration;
        readonly AppointmentSchedulerDbContext db;
        private readonly SqlConnection sqlConnection;
        private readonly IServiceCollection services;
        public static UserModel User { get; set; }

        public AccountController(AppointmentSchedulerDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
            sqlConnection = new SqlConnection(configuration.GetConnectionString("AppointmentSchedulerDb"));

        }

/*
        // GET: AccountController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel user)
        {

            try
            {
                user.UserName = Request.Form["UserName"];
                user.Password = Request.Form["Password"];
                Console.WriteLine("Passsss " + user.Password);
                ;

                if (user.VerifyUser(sqlConnection))
                {
                    ViewBag.Error = "";
                    HttpContext.Response.Cookies.Append("logged_in", "true");
                    HttpContext.Response.Cookies.Append("current_user_email", user.Email!);
                    HttpContext.Response.Cookies.Append("current_user_name", user.UserName!);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Username or Password Incorrect";
                    return View();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View();
            }
        }

        // GET: AccountController/Logout
        public ActionResult logout()
        {
            Response.Cookies.Delete("logged_in");
            return RedirectToAction("Index", "Home");

        }*/

        [HttpGet]
        public IActionResult Login()
        {
           

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel user)
        {
            if (user.VerifyUser(sqlConnection))
            {
                User = user;
                var identity = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Sid, user.UserId.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.MobileNumber),
            new Claim(ClaimTypes.Email, user.Email),
        }, CookieAuthenticationDefaults.AuthenticationScheme);

                try
                {
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                }
                catch(Exception ex)
                {
                    Console.WriteLine("Login Exception "+ex.Message);
                       
                }
                Console.WriteLine("Inside Login Post");
                string token = CreateToken();
                Console.WriteLine("Token " + token);

                UserModel ser = GetCurrentUser();

                Response.Headers.Add("Authorization", "Bearer " + token);
                Response.Cookies.Append("auth_token", token);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        public string CreateToken()
        {
            Console.WriteLine("Create Token "+ User.UserName);
            var claim = new List<Claim>() {
        new Claim(ClaimTypes.Name, User.UserName!),
        new Claim(ClaimTypes.Sid, User.UserId.ToString()),
        new Claim(ClaimTypes.MobilePhone, User.MobileNumber!),
        new Claim(ClaimTypes.Email, User.Email!)
    };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value!
                ));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claim,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Console.WriteLine("Curre user "+User.UserName);

            if (identity != null)
            {
                Console.WriteLine("Identity name: " + identity.Name);
                Console.WriteLine("Identity authentication type: " + identity.AuthenticationType);
                Console.WriteLine("Identity claims count: " + identity.Claims.Count());

                var userClaims = identity.Claims;
                Console.WriteLine("Get curr "+ userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value);
                return new UserModel
                {
                    UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    UserId = Convert.ToInt32((userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value)),
                    MobileNumber = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.MobilePhone)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                };
            }

            return null;
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Login","Account");
        }


        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }






    }
}
