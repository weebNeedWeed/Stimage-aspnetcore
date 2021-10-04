using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace imagestore.ViewModels
{
    public class UploadImageViewModel
    {
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }
        [MaxLength(200)]
        [Required]
        public string Slug { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [Required]
        public IFormFile ImageData { get; set; }
        [Required]
        public bool IsPublic { get; set; } = true;
    }
}
