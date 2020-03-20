using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        // quản lý roles
        private readonly RoleManager<IdentityRole> roleManager;
        // Quản lý users
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// Hàm lấy danh sách users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListUsers()
        {
            var model = userManager.Users.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo role
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };
                // Thêm role mới
                IdentityResult result = await roleManager.CreateAsync(role);
                // Kiểm tra nếu thêm role thành công
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }

                foreach (IdentityError er in result.Errors)
                {
                    ModelState.AddModelError("", er.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            // Liệt kê tất cả các role
            var role = roleManager.Roles;
            return View(role);
        }

        /// <summary>
        /// Hàm đến trang sửa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel()
            {
                Id = id,
                RoleName = role.Name
            };

            foreach (var user in await userManager.Users.ToListAsync())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Hàm sửa role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            // xác định role bằng id
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                // Update role
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Hàm đến trang thay đổi user thuộc role
        /// </summary>
        /// <param name="roleId">id role cần sửa</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            // Xác định role cần sửa
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach (var user in await userManager.Users.ToListAsync())
            {
                UserRoleViewModel userRoleView = new UserRoleViewModel()
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleView.IsSelected = true;
                }
                else
                {
                    userRoleView.IsSelected = false;
                };
                model.Add(userRoleView);
            }

            return View(model);
        }


        /// <summary>
        /// Hàm thay đổi users thuộc role
        /// </summary>
        /// <param name="model">danh sách user</param>
        /// <param name="roleId">id của role cần sửa user</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
                return View("NotFound");
            }

            foreach (var userRoleView in model)
            {
                var user = await userManager.FindByIdAsync(userRoleView.UserId);
                if (userRoleView.IsSelected && !await userManager.IsInRoleAsync(user, role.Name))
                {
                    await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRoleView.IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
            }

            return RedirectToAction("EditRole", new { id = roleId });
        }

        /// <summary>
        /// Hàm sửa user
        /// </summary>
        /// <param name="id">id của user</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            // Tìm user theo id
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id={id} cannot be found";
                return View("NotFound");
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            var model = new EditUserViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                Id = user.Id,
                City = user.City,
                Roles = userRoles.ToList(),
                Claims = userClaims.Select(c => c.Value).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("listusers");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("listusers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("listrole");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("listrole");
        }
    }
}