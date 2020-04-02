using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Account.ChangeUserEmail
{
    public class ChangeUserEmailResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
