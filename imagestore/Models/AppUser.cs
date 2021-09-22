using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imagestore.Models
{
    public class AppUser: IdentityUser
    {
        public ICollection<AppImage> AppImages { get; set; }
    }
}
