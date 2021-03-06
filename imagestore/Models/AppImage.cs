using System;
using System.ComponentModel.DataAnnotations;

namespace imagestore.Models
{
    public class AppImage
    {
        public int ImageId { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedAt { get; set; }
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }
        [MaxLength(200)]
        [Required]
        public string Slug { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [Required]
        public byte[] ImageData { get; set; }
        [Required]
        public bool? IsPublic { get; set; }
        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
