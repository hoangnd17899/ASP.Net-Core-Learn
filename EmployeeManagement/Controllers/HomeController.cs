using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement
{
    
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private IWebHostEnvironment _iHostingEnvironment;
        private ILogger logger;

        public HomeController(IEmployeeRepository employeeRepository,IWebHostEnvironment iHostingEnvironment,
                                ILogger<HomeController> _logger)
        {
             logger=_logger;
            _employeeRepository = employeeRepository;
            _iHostingEnvironment = iHostingEnvironment;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            //throw new Exception("Exeption from details");
            logger.LogTrace("Trace log");
            logger.LogDebug("Debug log");
            logger.LogInformation("Information log");
            logger.LogWarning("Warning log");
            logger.LogError("Error log");
            logger.LogCritical("Critial log");

            Employee employee = _employeeRepository.GetEmployee(id ?? 1);

            if(employee==null){
                Response.StatusCode = 404;
                return View("EmployeeNotFound",id.Value);
            }
            HomeDetailViewModel homeDetailViewModel = new HomeDetailViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(homeDetailViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                Employee newEmp = new Employee(){
                    Name=model.Name,
                    Email=model.Email,
                    Department=model.Department,
                    PhotoPath=uniqueFileName
                };

                newEmp = _employeeRepository.Add(newEmp);
                return RedirectToAction("Details", new { id = newEmp.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee model = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = id,
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                ExistingPhotoPath = model.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee newEmp = new Employee(){
                    Id=model.Id,
                    Name=model.Name,
                    Email=model.Email,
                    Department=model.Department,
                };
                if(model.Photo!=null){
                    if(model.ExistingPhotoPath!=null){
                        string filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "contents/images",model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    newEmp.PhotoPath = ProcessUploadedFile(model);
                }

                newEmp = _employeeRepository.Update(newEmp);
                return RedirectToAction("Details", new { id = newEmp.Id });
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model){
            string uniqueFileName = null;
            if(model.Photo!=null){
                    string upLoadFolder = Path.Combine(_iHostingEnvironment.WebRootPath, "contents/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(upLoadFolder, uniqueFileName);
                    using(var fileStream = new FileStream(filePath, FileMode.Create)){
                        model.Photo.CopyTo(fileStream);
                    }
            }

            return uniqueFileName;
        }
    }
}
