using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UdritDhakal_SMS.Data;
using UdritDhakal_SMS.Infrastructure.IRepository;
using UdritDhakal_SMS.Models.Entity;

namespace UdritDhakal_SMS.Controllers
{
    [Authorize(Roles = "ADMIN, STUDENT")]

    public class StudentController : Controller
    {
        private readonly ICrudService<StudentInfo> _studentInfoService;
        private readonly ICrudService<CourseInfo> _courseInfoService;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(ICrudService<StudentInfo> studentInfoService,
            ICrudService<CourseInfo> courseInfoService,
            UserManager<AppUser> userManager)
        {
            _studentInfoService = studentInfoService;
            _courseInfoService = courseInfoService;
            _userManager = userManager;
        }
        [Authorize(Roles = "ADMIN, STUDENT")]
        public async Task<IActionResult> Index(String searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var students = await _studentInfoService.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString) ||
                                               s.Email.Contains(searchString) ||
                                               s.PhoneNumber.Contains(searchString)).ToList();
            }
            ViewBag.CourseInfos = await _courseInfoService.GetAllAsync(p => p.IsActive == true);
            return View(students);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);
            StudentInfo studentInfo = new StudentInfo();
            studentInfo.IsActive = true;

            ViewBag.CourseInfos = await _courseInfoService.GetAllAsync(p => p.IsActive == true);

            if (id > 0)
            {
                studentInfo = await _studentInfoService.GetAsync(id);
            }
            ViewBag.Sections = new List<SelectListItem>
            {
                new SelectListItem { Value = "A", Text = "Section A" },
                new SelectListItem { Value = "B", Text = "Section B" },
                new SelectListItem { Value = "C", Text = "Section C" },
                new SelectListItem { Value = "D", Text = "Section D" },
                new SelectListItem { Value = "E", Text = "Section E" },
                new SelectListItem { Value = "F", Text = "Section F" }
            };

            return View(studentInfo);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> AddEdit(StudentInfo studentInfo)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            ViewBag.CourseInfos = await _courseInfoService.GetAllAsync(p => p.IsActive == true);

            if (ModelState.IsValid)
            {
                try
                {
                    //Handle Image File Upload
                    if (studentInfo.ImageFile != null)
                    {
                        string fileDirectory = $"wwwroot/StudentImage";

                        if (!Directory.Exists(fileDirectory))
                        {
                            Directory.CreateDirectory(fileDirectory);
                        }
                        string uniqueFileName = Guid.NewGuid() + "_" + studentInfo.ImageFile.FileName;
                        string filePath = Path.Combine(Path.GetFullPath($"wwwroot/StudentImage"), uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await studentInfo.ImageFile.CopyToAsync(fileStream);
                            studentInfo.ImageUrl = $"StudentImage/" + uniqueFileName;

                        }

                    }
                    if (studentInfo.Id == 0)
                    {
                        studentInfo.CreatedDate = DateTime.Now;
                        studentInfo.CreatedBy = userId;
                        await _studentInfoService.InsertAsync(studentInfo);
                        TempData["success"] = "Data  Addded Successfully";
                    }
                    else
                    {
                        var OrgStudentInfo = await _studentInfoService.GetAsync(studentInfo.Id);
                        OrgStudentInfo.Name = studentInfo.Name;
                        OrgStudentInfo.Email = studentInfo.Email;
                        OrgStudentInfo.PhoneNumber = studentInfo.PhoneNumber;
                        OrgStudentInfo.CourseId = studentInfo.CourseId;
                        OrgStudentInfo.Gender = studentInfo.Gender;
                        OrgStudentInfo.Address = studentInfo.Address;
                        OrgStudentInfo.Class = studentInfo.Class;
                        OrgStudentInfo.Section = studentInfo.Section;
                        OrgStudentInfo.IsActive = studentInfo.IsActive;
                        OrgStudentInfo.ModifiedDate = studentInfo.ModifiedDate;
                        OrgStudentInfo.ModifiedBy = userId;

                        if (studentInfo.ImageFile != null)
                        {
                            OrgStudentInfo.ImageUrl = studentInfo.ImageUrl;
                        }

                        await _studentInfoService.UpdateAsync(OrgStudentInfo);
                        TempData["success"] = "Data Updated Sucessfully";
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["error"] = "Something went wrong, please try again later: ";
                    return RedirectToAction(nameof(AddEdit));
                }

            }
            // If ModelState is not valid, return to the form with error messages
            TempData["warning"] = "Please input Valid Data";
            return RedirectToAction(nameof(AddEdit));
        }
        [Authorize(Roles = "ADMIN, STUDENT")]
        public async Task<IActionResult> Details(int id)
        {
            var studentInfo = await _studentInfoService.GetAsync(id);
            ViewBag.CourseInfos = await _courseInfoService.GetAllAsync(p => p.IsActive == true);

            return View(studentInfo);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var studentInfo = await _studentInfoService.GetAsync(id);
            _studentInfoService.Delete(studentInfo);
            TempData["error"] = "Data Deleted Sucessfully";
            return RedirectToAction("Index");
        }
        //[Authorize(Roles = "STUDENT")]
        //public async Task<IActionResult> StDetails(int id)
        //{
        //    var studentInfo = await _studentInfoService.GetAsync(id);
        //    ViewBag.CourseInfos = await _courseInfoService.GetAllAsync(p => p.IsActive == true);

        //    return View(studentInfo);
        //}
    }
}
