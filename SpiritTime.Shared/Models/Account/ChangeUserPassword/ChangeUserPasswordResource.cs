using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Account.ChangeUserPassword
{
    public class ChangeUserPasswordResource
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
