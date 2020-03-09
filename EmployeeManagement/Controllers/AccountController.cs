using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public class AccountController:Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> _userManager,SignInManager<IdentityUser> _signInManager)
        {
            userManager=_userManager;
            signInManager=_signInManager; 
        }
        [HttpGet]
        public IActionResult Register(){
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model){
            if(ModelState.IsValid){
                var user=new IdentityUser(){
                    Email=model.Email,
                    UserName=model.Email
                };
                var result= await userManager.CreateAsync(user,model.Password);

                if(result.Succeeded){
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
        public IActionResult Login(){
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model){
            if(ModelState.IsValid){
                var result= await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);

                if(result.Succeeded){
                    return RedirectToAction("Index","Home");
                }

                ModelState.AddModelError("","Invalid Login Attempt");
            }
            return View();
        }
    }
}