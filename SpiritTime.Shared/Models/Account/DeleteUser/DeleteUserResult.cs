using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Models.Account.DeleteUser
{
    public class DeleteUserResult
    {
        public bool Successful { get; set; }
        public List<string> Error { get; set; }
    }
}
