
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application_Scheduler.Data;
using Application_Scheduler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

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

        [HttpGet]
        public IActionResult Login()
        {
           

            return View();
        }


        [HttpPost]
        [Route("/Home")]
        public IActionResult Login(UserModel user)
        {
            //check if the employee already exists

            user.UserName = Request.Form["UserName"];
            user.Password = Request.Form["Password"];

            Console.WriteLine("User " + user.UserName);
            //check if the employee exists in the database
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand("select * from Users where username=@Username", sqlConnection);
            cmd.Parameters.AddWithValue("@Username", user.UserName);
            SqlDataReader reader = cmd.ExecuteReader();

            //Generate jwt token for authentication if passwod matches and also import the necessary package
            if (reader.Read())
            {
                User = user;
                try
                {
                  
                    var passwordHash = new PasswordHasher<UserModel>();
                    // Hash the plain-text password using the same algorithm and settings as the one used to hash the password stored in the database
                    string hashedPassword = passwordHash.HashPassword(user, reader["Password"].ToString());

                    var result = passwordHash.VerifyHashedPassword(user, hashedPassword, user.Password);
                  
                    if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                    {
                     
                        var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                     
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
                       
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                   issuer: configuration["Jwt:Issuer"],
                   audience: configuration["Jwt:Issuer"],
                   claims: claims,
                   expires: DateTime.Now.AddDays(7), // Set the expiration time to 7 days
                   signingCredentials: creds);

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                        // Set the token as a cookie in the response
                        Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddDays(7) // Set the expiration time to 7 days
                        });
                        Console.WriteLine("Token " + new JwtSecurityTokenHandler().WriteToken(token));

                        

                    }
                    
                    reader.Close();
                    sqlConnection.Close();
                    // Redirect to Home/Index action
                    return View("~/Views/Home/Index.cshtml");

                
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error " + ex.Message);
                }
            }
            Console.WriteLine("12");
            return RedirectToAction("Login", "Account");

        }


        [HttpGet]
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
