using imagestore.Data;
using imagestore.Models;
using imagestore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            ViewBag.Message = $"https://{Request.Host}/Image/Show/{slug}";
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manager()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;

            List<AppImage> images = await _context.AppImages.Where(u => u.AppUserId == userId).ToListAsync();

            ViewBag.Hostname = Request.Host;
           
            return View(images);
        }

        [HttpGet("/[controller]/[action]/{slug}")]
        public async Task<IActionResult> Show([FromRoute]string slug) {
            AppImage image = await _context.AppImages.Where(x => x.Slug == slug).FirstOrDefaultAsync();

            if(image == null)
            {
                return NotFound();
            }

            if (!(bool)(image.IsPublic))
            {
                return BadRequest("Private Image");
            }

            byte[] imageData = image.ImageData;

            return File(imageData, "image/jpg");
        }
    }
}
