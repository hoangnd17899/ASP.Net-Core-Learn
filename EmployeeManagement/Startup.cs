using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace EmployeeManagement
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
            services.AddMvc(options => {
                options.EnableEndpointRouting = false;

                // Add authentication globally
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddIdentity<IdentityUser,IdentityRole>(options=>{
                options.Password.RequiredLength=10;
                options.Password.RequireUppercase=false;
                options.Password.RequireNonAlphanumeric=false;
            }).AddEntityFrameworkStores<AppDBContext>();

            // Thay đổi validation required cho IdentityUser
            // services.Configure<IdentityOptions>(options=>{
            //     options.Password.RequiredLength=10;
            //     options.Password.RequireUppercase=false;
            //     options.Password.RequireNonAlphanumeric=false;
            // });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddDbContext<AppDBContext>(
                optionsAction => optionsAction.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Trả về trang error mặc định của môi trường developement
                // app.UseDeveloperExceptionPage();

                // Trả về trang lỗi custom
                // Bắt lỗi cụ thể
                app.UseExceptionHandler("/Error");
                // Bắt lỗi theo status code
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                // // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
