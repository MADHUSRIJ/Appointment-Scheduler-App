using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

namespace Application_Scheduler
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // set the login page
                    options.AccessDeniedPath = "/Account/AccessDenied"; // set the access denied page
                });

            // other services
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

        }


    }
}
