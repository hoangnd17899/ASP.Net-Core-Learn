using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement
{
    public class AccountController:Controller
    {
        // Quản lý các phương thức với tài khoản(CRUD)
        private readonly UserManager<ApplicationUser> userManager;
        // Quản lý đăng nhập
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> _userManager,SignInManager<ApplicationUser> _signInManager)
        {
            userManager=_userManager;
            signInManager=_signInManager; 
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(){
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model){
            if(ModelState.IsValid){
                // Tạo đối tượng Identity User
                var user=new ApplicationUser(){
                    Email=model.Email,
                    UserName=model.Email,
                    City=model.City
                };

                // Thêm mới IdentityUser
                var result= await userManager.CreateAsync(user,model.Password);

                // Kiểm tra thêm mới thành công
                if(result.Succeeded){
                    // Đăng nhập cho tài khoản vừa đăng ký
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index","Home");
                }

                foreach(var error in result.Errors){
                    ModelState.AddModelError("",error.Description);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout(){
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(){
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl){
            if(ModelState.IsValid){
                // Đăng nhập
                // isPersistent để thực hiện lưu thông tin đăng nhập persistent cookie
                var result= await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);

                if(result.Succeeded){
                    // Kiểm tra xem có returnUrl không để khi login thành công chạy vào url ấy 
                    // Kiểm tra xem có phải url cục bộ không, đề phòng hacker tấn công bằng việc thay đổi returnUrl trong
                    // Query String         
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)){
                        return Redirect(returnUrl);
                    }
                    // Nếu không có returnUrl mặc định về trang home sau khi đã đăng nhập
                    return RedirectToAction("Index","Home");
                }

                ModelState.AddModelError("","Invalid Login Attempt");
            }
            return View();
        }

        [AcceptVerbs("GET","POST")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email){
            var user=await userManager.FindByEmailAsync(email);
            if(user==null){
                return Json(true);
            }
            else{
                return Json($"Email {email} is already in use");
            }
        }
    }
}