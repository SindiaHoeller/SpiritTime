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

        private readonly string _workspaceAdd;
        public string WorkspaceAdd => _workspaceAdd;

        private readonly string _workspaceDelete;
        public string WorkspaceDelete => _workspaceDelete;

        private readonly string _workspaceEdit;
        public string WorkspaceEdit => _workspaceEdit;

        private readonly string _tagGetall;
        public string TagGetAll => _tagGetall;
        private readonly string _tagAdd;
        public string TagAdd => _tagAdd;
        private readonly string _tagEdit;
        public string TagEdit => _tagEdit;
        private readonly string _tagDelete;
        public string TagDelete => _tagDelete;

        public Paths(string basepath)
        {
            _register = basepath + "/Account/Register";
            _login = basepath + "/Account/Login";

            var workspace = "/Workspaces/";
            _workspaceGetall = basepath + workspace + "GetallByUserId";
            _workspaceAdd = basepath + workspace + "Create";
            _workspaceDelete = basepath + workspace + "Delete";
            _workspaceEdit = basepath + workspace + "Update";

            var tag = "/tags/";
            _tagGetall = basepath + tag + "GetAll";
            _tagAdd = basepath + tag + "Create";
            _tagEdit = basepath + tag + "Update";
            _tagDelete = basepath + tag + "Delete";
        }
    }
}
