using imagestore.Data;
using imagestore.Models;
using imagestore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace imagestore.Controllers
{
    public class ImageController : Controller
    {
        private readonly StimageContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ImageController(StimageContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm]UploadImageViewModel uploadImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Unknown error. Please try again later!");
                return View(uploadImageViewModel);
            }

            string slug = uploadImageViewModel.Slug.Replace(" ", "-");

            using (var ms = new MemoryStream())
            {
                uploadImageViewModel.ImageData.CopyTo(ms);
                byte[] fileData = ms.ToArray();

                AppImage img = new AppImage
                {
                    Description = uploadImageViewModel.Description,
                    Title = uploadImageViewModel.Title,
                    ImageData = fileData,
                    Slug = slug,
                    IsPublic = uploadImageViewModel.IsPublic,
                    AppUserId = _userManager.GetUserId(User)
                }; 
                try
                {
                    await _context.AppImages.AddAsync(img);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unknown error. Please try again later!");
                    return View();
                }
            }

            ViewBag.Message = $"Success. Your image url is https://{Request.Host}/Image/Show/{slug}";
            return View();
        }
    }
}
