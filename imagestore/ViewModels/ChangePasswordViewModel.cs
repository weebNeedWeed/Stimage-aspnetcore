using System.ComponentModel.DataAnnotations;

namespace imagestore.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(30)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
