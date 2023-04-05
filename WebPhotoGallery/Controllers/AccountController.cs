using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebPhotoGallery.Helper;
using WebPhotoGallery.Models;

namespace WebPhotoGallery.Controllers
{
    [Route("Account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            var user = new ApplicationUser()
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.Phone,
                RegDate = DateTime.Now.Date,

            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {

                await _userManager.AddToRoleAsync(user, Constant.User);
                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerViewModel);
        }

        [HttpGet("ViewProfile")]
        public async Task<IActionResult> ViewProfile()
        {
            var userdetails = await _userManager.GetUserAsync(User);
            if (userdetails == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View();
            }

            return View(userdetails);
        }

        [HttpGet("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userdetails = new UpdateProfileVM()
            {
                Email = user.Email,
                Id = user.Id,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Password = null
            };
            return View(userdetails);
        }
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM updateProfileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updateProfileVM);
            }
            var user = await _userManager.FindByIdAsync(updateProfileVM.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return View(updateProfileVM);
            }
            var CheckPassword = await _userManager.CheckPasswordAsync(user, updateProfileVM.Password);
            if (CheckPassword)
            {
                user.FirstName = updateProfileVM.FirstName;
                user.LastName = updateProfileVM.LastName;
                user.UserName = updateProfileVM.UserName;
                user.Email = updateProfileVM.Email;
                user.PhoneNumber = updateProfileVM.Phone;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    return RedirectToAction("ViewProfile", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            if (!CheckPassword)
            {
                ViewBag.ErrorMessage = "Password does not correct";
                return View(updateProfileVM);
            }

            return View(updateProfileVM);
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user ==null)
                {
                    ViewBag.LoginError = "User does not exist";
                    return View(model);
                }
                var role = await _userManager.GetRolesAsync(user);
                if (user.EmailConfirmed == false && role.FirstOrDefault() == Constant.User)
                {
                    ViewBag.LoginError = "Account is not approved";
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.LoginError = "Invalid login attempt";
                    return View(model);
                }
            }
            return View(model);
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");

        }
        [HttpGet("AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
