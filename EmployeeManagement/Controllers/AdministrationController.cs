using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement
{
    public class AdministrationController:Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            this.roleManager=roleManager;
            this.userManager=userManager;
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
                    return RedirectToAction("ListRole");
                }

                foreach(IdentityError er in result.Errors){
                    ModelState.AddModelError("",er.Description);
                }
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRole(){
            var role=roleManager.Roles;
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id){
            var role=await roleManager.FindByIdAsync(id);

            if(role==null){
                ViewBag.ErrorMessage=$"Role with Id={id} cannot be found";
                return View("NotFound");
            }
            var model=new EditRoleViewModel(){
                Id=id,
                RoleName=role.Name
            };

            foreach(var user in await userManager.Users.ToListAsync()){
                if(await userManager.IsInRoleAsync(user,role.Name)){
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model){
            var role=await roleManager.FindByIdAsync(model.Id);

            if(role==null){
                ViewBag.ErrorMessage=$"Role with Id={model.Id} cannot be found";
                return View("NotFound");
            }
            else{
                role.Name=model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if(result.Succeeded){
                    return RedirectToAction("ListRole");
                }

                foreach(var err in result.Errors){
                    ModelState.AddModelError("",err.Description);
                }
            }

            return View(model);
        } 

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId){
            ViewBag.roleId=roleId;

            var role=await roleManager.FindByIdAsync(roleId);

            if(role==null){
                ViewBag.ErrorMessage=$"Role with Id={roleId} cannot be found";
                return View("NotFound");
            }

            var model= new List<UserRoleViewModel>();
            foreach(var user in await userManager.Users.ToListAsync()){
                UserRoleViewModel userRoleView=new UserRoleViewModel(){
                    UserName=user.UserName,
                    UserId=user.Id,
                };
                if(await userManager.IsInRoleAsync(user,role.Name)){
                    userRoleView.IsSelected=true;
                }else{
                    userRoleView.IsSelected=false;
                };
                model.Add(userRoleView);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string roleId){
            var role=await roleManager.FindByIdAsync(roleId);
            if(role==null){
                ViewBag.ErrorMessage=$"Role with Id={roleId} cannot be found";
                return View("NotFound");
            }

            foreach(var userRoleView in model){
                var user=await userManager.FindByIdAsync(userRoleView.UserId);
                if(userRoleView.IsSelected && !await userManager.IsInRoleAsync(user,role.Name)){
                    await userManager.AddToRoleAsync(user,role.Name);
                }else if(!userRoleView.IsSelected && await userManager.IsInRoleAsync(user,role.Name)){
                    await userManager.RemoveFromRoleAsync(user,role.Name);
                }else{
                    continue;
                }
            }

            return RedirectToAction("EditRole",new {id=roleId});
        }
    }
}