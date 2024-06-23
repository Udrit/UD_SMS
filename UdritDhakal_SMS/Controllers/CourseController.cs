using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UdritDhakal_SMS.Data;
using UdritDhakal_SMS.Infrastructure.IRepository;
using UdritDhakal_SMS.Models.Entity;

namespace UdritDhakal_SMS.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class CourseController : Controller
    {
        private readonly ICrudService<CourseInfo> _courseInfo;
        private readonly UserManager<AppUser> _userManager;

        public CourseController(ICrudService<CourseInfo> courseInfo,
            UserManager<AppUser> userManager)
        {
            _courseInfo = courseInfo;
            _userManager = userManager;
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            var courseInfoList = await _courseInfo.GetAllAsync();
            return View(courseInfoList);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            CourseInfo courseInfo = new CourseInfo();
            courseInfo.IsActive = true;
            if (id > 0)
            {
                courseInfo = await _courseInfo.GetAsync(id);
            }

            return View(courseInfo);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> AddEdit(CourseInfo courseInfo)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    if (courseInfo.Id == 0)
                    {
                        courseInfo.CreatedDate = DateTime.Now;
                        courseInfo.CreatedBy = userId;
                        await _courseInfo.InsertAsync(courseInfo);
                        TempData["success"] = "Data  Addded Successfully";
                    }
                    else
                    {
                        var OrgCourseInfo = await _courseInfo.GetAsync(courseInfo.Id);
                        OrgCourseInfo.CourseName = courseInfo.CourseName;
                        OrgCourseInfo.CourseDescription = courseInfo.CourseDescription;
                        OrgCourseInfo.IsActive = courseInfo.IsActive;
                        OrgCourseInfo.ModifiedDate = courseInfo.ModifiedDate;
                        OrgCourseInfo.ModifiedBy = userId;
                        await _courseInfo.UpdateAsync(OrgCourseInfo);
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
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var courseInfo = await _courseInfo.GetAsync(id);
            _courseInfo.Delete(courseInfo);
            TempData["error"] = "Data Successefully Deleted";
            return RedirectToAction("Index");
        }
    }
}


