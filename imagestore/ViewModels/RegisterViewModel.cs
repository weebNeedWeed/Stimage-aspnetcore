using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace imagestore.ViewModels
{
    public class RegisterViewModel
    {
        [StringLength(25, MinimumLength = 8, ErrorMessage ="UserName must be less than 25 and greater than 8")]
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage ="Must be email")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be less than 20 and greater than 8")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Must be compare with Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }
    }
}
