using imagestore.Models;
using System.ComponentModel.DataAnnotations;

namespace imagestore.ViewModels
{
    public class EditViewModel
    {
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [Required]
        public bool IsPublic { get; set; } = true;
    }
}
