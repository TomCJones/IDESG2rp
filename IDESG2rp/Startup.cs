using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IDESG2rp.Data;
using IDESG2rp.Models;
using IDESG2rp.Services;
using Microsoft.AspNetCore.Http;  // for statuscodes
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IDESG2rp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            //
            string CId = Configuration["TCGoogleClientID"];
            string CSec = Configuration["TCGoogleSecret"];
            string OId = "IDESG2rp";
            string OSec = "bazzfazz";
            /*            GoogleOptions gOptions = new GoogleOptions()
            {
                ClientId = CId,
                ClientSecret = CSec
            };
            gOptions.Scope.Add("email");   //added at suggestion on http://stackoverflow.com/questions/19775321/owins-getexternallogininfoasync-always-returns-null/29921451#29921451 
                                           //this may not be needed when user record already exists, but when it doesn't 
                                           //then it could be used to populate the user and email names


            if (!string.IsNullOrWhiteSpace(CId) && !string.IsNullOrWhiteSpace(CSec))
            {
                app.UseGoogleAuthentication(gOptions);
            }
            else
            {
                // TODO Call application insights in this case
            }
            */

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o => o.LoginPath = new PathString("/signin"))
            .AddGoogle(o=> { o.ClientId = CId;  o.ClientSecret = CSec;  })
 //           .AddOpenIdConnect(o => { o.ClientId = OId; o.ClientSecret = OSec; o.Authority = "https://localhost:44370"; });
             .AddOpenIdConnect(o => { o.ClientId = OId; o.ClientSecret = OSec;
 //                o.Scope.Add("email")  ;                  // to existing openid (for sid) and profile (for name)
                 o.Authority = "https://idesg-idp.azurewebsites.net"; });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
            int setPortNumber = 44371;     // will need to change this for deployment - load this from the launch settings at runtime - or default if 443 - or always set to something?
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = setPortNumber;   
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            string[] lsAdmins = null;
            try
            {
                string sAdmins = Configuration["BaseAdministrators:admin"];  // TODO  not working
                lsAdmins = sAdmins.Split(' ');  // get the list of users who are system admins
            }
            catch { }   // no problem if BaseAdministrators is not needed


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
