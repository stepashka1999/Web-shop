using Microsoft.AspNetCore.Identity;

namespace Web_shop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}