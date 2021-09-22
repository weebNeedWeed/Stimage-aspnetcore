using imagestore.Models;
using imagestore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace imagestore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            if(await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
            {
                ModelState.AddModelError("", "Email existed!");
                return View(registerViewModel);
            }

            if(await _userManager.FindByNameAsync(registerViewModel.UserName) != null)
            {
                ModelState.AddModelError("", "UserName existed!");
                return View(registerViewModel);
            }

            AppUser newUser = new AppUser
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Unknown Error. Please try again later!");
                return View(registerViewModel);
            }

            await _userManager.AddToRoleAsync(newUser, "default");

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm]LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if(user == null)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(loginViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, true, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(loginViewModel);
            }

            return RedirectToAction("Index", "Home");
        }   
    }
}
