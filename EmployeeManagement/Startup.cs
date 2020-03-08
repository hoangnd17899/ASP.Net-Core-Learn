using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

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
            services.AddMvc(options => options.EnableEndpointRouting = false);
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
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                // // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
