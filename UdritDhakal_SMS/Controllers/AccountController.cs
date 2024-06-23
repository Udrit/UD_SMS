using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text;
using System;
using UdritDhakal_SMS.Data;
using UdritDhakal_SMS.Infrastructure.IRepository;
using UdritDhakal_SMS.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using UdritDhakal_SMS.ViewModels;

namespace UdritDhakal_SMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICrudService<StudentInfo> _studentInfo;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<RegisterViewModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;

        public AccountController(ICrudService<StudentInfo> studentInfo,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterViewModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IUserStore<AppUser> userStore
            )
        {
            _studentInfo = studentInfo;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            // Fetch additional user details as needed
            var userInfo = new UserInfo
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RoleName = "", // You'll need to retrieve the role name separately
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };

            // Fetch role information
            var role = await _roleManager.FindByIdAsync(user.UserRoleId);
            if (role != null)
            {
                userInfo.RoleName = role.Name;
            }

            // Create a list to hold user info for display in the view
            var userInfos = new List<UserInfo> { userInfo };

            return View(userInfos);
        }

        public async Task<IActionResult> AddUser()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.UserRoleId = user.UserRoleId;
            registerViewModel.IsActive = true;

            //var roleinfo = await _roleManager.FindByIdAsync(user.UserRoleId);
            //if (roleinfo.Name == "ADMIN")
            //{
            //    ViewBag.StudentInfos = await _studentInfo.GetAllAsync();
            //}
            //else
            //{
            //    ViewBag.StudentInfos = await _studentInfo.GetAllAsync(p => p.Id == user.UserRoleId);
            //}
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var userIds = _userManager.GetUserId(HttpContext.User);
                var users = await _userManager.FindByIdAsync(userIds);

                var user = CreateUser();
                user.FirstName = registerViewModel.FirstName;
                user.MiddleName = registerViewModel.MiddleName;
                user.LastName = registerViewModel.LastName;
                user.Address = registerViewModel.Address;
                user.PhoneNumber = registerViewModel.PhoneNumber;
                user.PictureUrl = registerViewModel.PictureUrl;
                user.CreatedBy = users.Id;
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;

                var returnUrl = Url.Content("~/");
                var role = _roleManager.FindByNameAsync(registerViewModel.UserRoleId).Result;
                user.UserRoleId = role.Id;

                await _userStore.SetUserNameAsync(user, registerViewModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, registerViewModel.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {

                    if (role != null)
                    {
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(registerViewModel.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
        public async Task<IActionResult> UserStatus(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            bool status = false;
            if (user.IsActive == true)
            {
                status = false;
            }
            else
            {
                status = true;
            }
            user.IsActive = status;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetPassword(string Id)
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            var user = await _userManager.FindByIdAsync(Id);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            registerViewModel.Email = user.Email;
            registerViewModel.Code = code;
            registerViewModel.Id = Id;
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(RegisterViewModel registerViewModel)
        {
            var user = await _userManager.FindByIdAsync(registerViewModel.Id);

            var result = await _userManager.ResetPasswordAsync(user, registerViewModel.Code, registerViewModel.Password);
            TempData["success"] = "Password Reset Completed";
            return RedirectToAction(nameof(Index));
        }
    }
}
