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
using Microsoft.AspNetCore.Http;

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

            services.AddIdentity<ApplicationUser,IdentityRole>(options=>{
                options.Password.RequiredLength=6;
                options.Password.RequireUppercase=false;
                options.Password.RequireNonAlphanumeric=false;
                // Kiểm tra email của tài khoản đã confirm hay chưa
                options.SignIn.RequireConfirmedEmail=true;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

            // Thay đổi validation required cho IdentityUser
            // services.Configure<IdentityOptions>(options=>{
            //     options.Password.RequiredLength=10;
            //     options.Password.RequireUppercase=false;
            //     options.Password.RequireNonAlphanumeric=false;
            // });

            // Thêm Claim sử dụng cho Authorize
            services.AddAuthorization(configure=>{
                configure.AddPolicy("DeleteRolePolicy",policy=>policy.RequireClaim("Delete Role","true"));
                // Add policy với 2 trường hợp
                // configure.AddPolicy("EditRolePolicy",policy=>policy.RequireAssertion(context=>
                //     // Điều kiện là thuộc Admin và chứa claim Edit Role
                //     context.User.IsInRole("Admin") && context.User.HasClaim(x=>x.Type=="Edit Role" && x.Value=="true") ||
                //     // Điều kiện là thuộc Super Admin
                //     context.User.IsInRole("Super Admin")
                // ));

                // Sử dụng Custom Authorization Requirement and Handler
                configure.AddPolicy("EditRolePolicy",policy=>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())
                );

                configure.AddPolicy("AdminPolicy",policy=>policy.RequireRole("Admin"));
            });


            // Set thời hạn của token confirm email hay reset password là 5 tiếng
            services.Configure<DataProtectionTokenProviderOptions>(options=>
                options.TokenLifespan=TimeSpan.FromHours(5)
            );

            // Add service Google và Facebook Authentication
            services.AddAuthentication()
            // Add Google
            .AddGoogle(options=>{
                options.ClientId="1043742852297-nvhj5d4hfmrhn7doicgpspb872hcf1i9.apps.googleusercontent.com";
                options.ClientSecret="U8IstixU17Ql-v4NdUEyjDY5";
            })
            // Add Facebook  
            .AddFacebook(options=>{
                options.ClientId="219600535765265";
                options.ClientSecret="995f98456ce1b40f20f89c25b513302e";
            });

            services.ConfigureApplicationCookie(opstions=>{
                opstions.AccessDeniedPath=new PathString("/Administration/AccessDenied");
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            // DI cho CanEditOnlyOtherAdminRolesAndClaimsHandler
            services.AddSingleton<IAuthorizationHandler,CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            // DI cho SuperAdminHandler
            services.AddSingleton<IAuthorizationHandler,SuperAdminHandler>();
            // Tạo instance cho Protector Value
            services.AddSingleton<DataProtectionPurposeStrings>();

            services.AddDbContext<AppDBContext>(
                optionsAction => optionsAction.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Trả về trang error mặc định của môi trường developement
                app.UseDeveloperExceptionPage();

                // Trả về trang lỗi custom
                // Bắt lỗi cụ thể
                //app.UseExceptionHandler("/Error");
                // Bắt lỗi theo status code
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            else
            {
                // app.UseExceptionHandler("/Error");
                // // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
