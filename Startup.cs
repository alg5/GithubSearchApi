using GithubSearchApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GithubSearchApi.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GithubSearchApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddControllers(mvcOtions =>
            //{
            //    mvcOtions.EnableEndpointRouting = false;
            //});
            services.AddTokenAuthentication(Configuration);
            //for use asp.net user session
            services.AddDistributedMemoryCache();
            //services.AddSession();
            //services.AddSession(options => {
            //    options.IdleTimeout = TimeSpan.FromMinutes(100);//You can set Time   
            //});
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSession(options => {
                options.Cookie.Name = ".GithubSearchApi.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });
            services.AddControllers(mvcOtions =>
            {
                mvcOtions.EnableEndpointRouting = false;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddTransient<GithubSearchService>();
            services.AddTransient<AuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
 
            
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
             app.UseMvc(routes =>
            {
                routes.MapRoute("getdata", "GetRepos/{q}", new { controller = "Home", action = "GetRepos" });
                routes.MapRoute("login", "Login", new { controller = "Home", action = "Login" });
                routes.MapRoute("add", "AddRepoToBookmarked", new { controller = "Home", action = "AddRepoToBookmarked" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
    }
}
