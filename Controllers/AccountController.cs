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

namespace Application_Scheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppointmentSchedulerDbContext _context;
        private readonly IConfiguration configuration;
        private readonly SqlConnection sqlConnection;
        private readonly IServiceCollection services;
        public static UserModel User { get; set; }

        public AccountController(AppointmentSchedulerDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
            sqlConnection = new SqlConnection(configuration.GetConnectionString("AppointmentSchedulerDb"));

        }

        [HttpGet]
        public IActionResult Login()
        {
            UserModel user = getCurrentUser();
            ViewBag.user = user;

            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {

            if (user.VerifyUser(sqlConnection))
            {
                User = user;
                Startup hello = new Startup();
                //hello.ConfigureServices(services,configuration);
                Console.WriteLine("Inside Login Post");
                string token = CreateToken();
                //Response.Headers.Add("Authorization", "Bearer "+ token);
                Response.Cookies.Append("auth_token", token);
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpPost]
        public string CreateToken()
        {
            List<Claim> claim = new List<Claim>() {
            new Claim(ClaimTypes.Name, User.UserName!),
            new Claim(ClaimTypes.Sid,User.UserId.ToString()),
            new Claim(ClaimTypes.MobilePhone,User.MobileNumber!),
            new Claim(ClaimTypes.Email,User.Email!)
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


        [HttpGet]
        public IActionResult register()
        {

            return View();
        }

        private UserModel getCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Console.WriteLine(identity);

            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                
            }
            if (identity != null)
            {
                var userClaims = identity.Claims;
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



        
    }
}
