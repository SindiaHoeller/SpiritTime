using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SpiritTime.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        
        public ICollection<Workspace> Workspaces { get; set; }
        public ICollection<TaskTagRules> TaskTagRules { get; set; }
    }
}
