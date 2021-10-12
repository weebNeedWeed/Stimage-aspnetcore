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
        private readonly List<string> allowedContentType = new List<string> { "image/gif", "image/jpeg", "image/png", "image/svg+xml", "image/webp" };

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

            if (!allowedContentType.Contains(uploadImageViewModel.ImageData.ContentType))
            {
                ModelState.AddModelError("", "Error. Allowed .gif .png .jpg .jpeg .svg .webp");
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
                    AppUserId = _userManager.GetUserId(User),
                    ContentType = uploadImageViewModel.ImageData.ContentType
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
            return File(imageData, image.ContentType);
        }

        [HttpGet("/[controller]/[action]/{slug}")]
        [Authorize]
        public async Task<IActionResult> Info([FromRoute]string slug)
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }
            else if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            var image = await _context.AppImages.FirstOrDefaultAsync(x => x.Slug == slug);

            if(image == null)
            {
                return RedirectToAction("Manager");
            }

            ViewBag.OldData = image;
            ViewBag.Base64ImageUrl = $"data:{image.ContentType};base64,{Convert.ToBase64String(image.ImageData)}";
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [Authorize]
        [HttpPost("/[controller]/[action]/{slug}")]
        public async Task<IActionResult> EditImage([FromForm]EditViewModel editViewModel, [FromRoute]string slug)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Unknown error. Please try again later!";
                return RedirectToAction("Info", new {slug = slug });
            }

            if(slug == null)
            {
                TempData["Error"] = "Unknown error. Please try again later!";
                return RedirectToAction("Info", new { slug = slug });
            }

            var image = await _context.AppImages.FirstOrDefaultAsync(x => x.Slug == slug);

            if(image == null)
            {
                TempData["Error"] = "Unknown error. Please try again later!";
                return RedirectToAction("Info", new { slug = slug });
            }

            image.IsPublic = editViewModel.IsPublic;
            image.Title = editViewModel.Title;
            image.Description = editViewModel.Description;

            try
            {
                _context.AppImages.Update(image);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["Error"] = "Unknown error. Please try again later!";
                return RedirectToAction("Info", new { slug = slug });
            }

            TempData["Message"] = "Your data have been updated!";
            return RedirectToAction("Info", new { slug = slug });
        }

        [HttpGet("/[controller]/[action]/{slug}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute]string slug)
        {
            if(slug == null)
            {
                return RedirectToAction("Manager");
            }

            var image = await _context.AppImages.FirstOrDefaultAsync(x => x.Slug == slug);

            if(image == null)
            {
                return RedirectToAction("Manager");
            }

            try
            {
                _context.AppImages.Remove(image);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return RedirectToAction("Manager");
            }


            return RedirectToAction("Manager");
        }
    }
}
