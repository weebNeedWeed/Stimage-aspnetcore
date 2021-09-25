using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace imagestore.Models
{
    public class AppUser: IdentityUser
    {
        public ICollection<AppImage> AppImages { get; set; }
    }
}
