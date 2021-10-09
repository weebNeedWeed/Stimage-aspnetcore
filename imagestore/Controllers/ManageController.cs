using imagestore.Models;
using imagestore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace imagestore.Controllers
{
    public class ManageController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public ManageController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            var user = await _userManager.GetUserAsync(User);

            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Unknown Error. Please try again later!";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            PasswordVerificationResult passresult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordViewModel.OldPassword);

            if (passresult != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Your old password is not correct!";
                return RedirectToAction("Index");
            }

            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmNewPassword)
            {
                TempData["Error"] = "ConfirmPassword not match password!";
                return RedirectToAction("Index");
            }

            try
            {
                await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unknown Error. Please try again later!";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Your password has been updated!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangeUsername([FromForm][Required][MinLength(5)][MaxLength(30)] string newUsername)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            user.UserName = newUsername;

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unknown Error. Please try again later!";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Your username has been updated!";
            return RedirectToAction("Index");
        }
    }
}
