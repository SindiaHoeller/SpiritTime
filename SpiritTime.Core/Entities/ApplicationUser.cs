using Microsoft.AspNetCore.Identity;

namespace SpiritTime.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
    }
}
