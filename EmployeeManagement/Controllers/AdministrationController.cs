using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public class AdministrationController:Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager=roleManager;
        }

        [HttpGet]
        public IActionResult CreateRole(){
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model){
            if(ModelState.IsValid){
                IdentityRole role=new IdentityRole(){
                    Name=model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(role);

                if(result.Succeeded){
                    return RedirectToAction("Index","Home");
                }

                foreach(IdentityError er in result.Errors){
                    ModelState.AddModelError("",er.Description);
                }
            }
            
            return View(model);
        }
    }
}