using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Account.Authentication
{
    public class AuthenticationResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public int WorkspaceId { get; set; }
    }
}
