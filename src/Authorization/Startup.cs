using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Infrastructure.Authorization;
using Authorization.Services;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();

            // Add Authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Sales, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("department", "sales");
                });
                options.AddPolicy(Policies.Over18, policy =>
                {
                    policy.Requirements.Add(new MinimumAgeRequirement(18));
                });
            });

            // register resource authorization handlers
            services.AddSingleton<IAuthorizationHandler, ProductAuthorizationHandler>();

            // register services
            services.AddSingleton<IProductsStore, InMemoryProductsStore>();
            services.AddSingleton<IDiscountPermissionService, DefaultDiscountPermissionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Configure the HTTP request pipeline.

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseCookieAuthentication(options =>
            {
                options.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.LoginPath = CookieAuthenticationDefaults.LoginPath; // -> /Account/Login
                options.AccessDeniedPath = CookieAuthenticationDefaults.AccessDeniedPath; // -> /Account/AccessDenied

                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            app.UseClaimsTransformation(principal =>
            {
                principal.Identities.First().AddClaim(new Claim("status", "junior"));
                return Task.FromResult(principal);
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
