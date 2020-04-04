using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.StaticDetails
{
    public class Paths
    {
        public string Register => _register;
        private readonly string _register;
        public string Login => _login;
        private readonly string _login;

        private readonly string _workspaceGetall;
        public string WorkspaceGetAll => _workspaceGetall;
        public Paths(string basepath)
        {
            _register = basepath + "/Account/Register";
            _login = basepath + "/Account/Login";
            _workspaceGetall = basepath + "/Workspace/Getall";
        }
    }
}
