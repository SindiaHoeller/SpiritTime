using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiritTime.Frontend.Services.StaticDetails
{
    public class Paths
    {
        // Account
        public string Register => _register;
        private readonly string _register;
        public string Login => _login;
        private readonly string _login;
        public string ConfirmEmail => _confirmEmail;
        private readonly string _confirmEmail;
        public string ChangeUserEmail => _changeUserEmail;
        private readonly string _changeUserEmail;
        public string ConfirmEmailAfterChange => _confirmEmailAfterChange;
        private readonly string _confirmEmailAfterChange;
        public string ChangeUserPassword => _changeUserPassword;
        private readonly string _changeUserPassword;
        public string DeleteUser => _deleteUser;
        private readonly string _deleteUser;


        // Workspace
        private readonly string _workspaceGetall;
        public string WorkspaceGetAll => _workspaceGetall;
        private readonly string _workspaceGetOne;
        public string WorkspaceGetOne => _workspaceGetOne;
        private readonly string _workspaceAdd;
        public string WorkspaceAdd => _workspaceAdd;
        private readonly string _workspaceDelete;
        public string WorkspaceDelete => _workspaceDelete;
        private readonly string _workspaceEdit;
        public string WorkspaceEdit => _workspaceEdit;

        // Tags
        private readonly string _tagGetall;
        public string TagGetAll => _tagGetall;
        private readonly string _tagAdd;
        public string TagAdd => _tagAdd;
        private readonly string _tagEdit;
        public string TagEdit => _tagEdit;
        private readonly string _tagDelete;
        public string TagDelete => _tagDelete;

        // Tasks
        private readonly string _taskGetall;
        public string TaskGetAllByWorkspace => _taskGetall;
        private readonly string _taskGetById;
        public string TaskGetById => _taskGetById;
        private readonly string _taskAdd;
        public string TaskAdd => _taskAdd;
        private readonly string _taskEdit;
        public string TaskEdit => _taskEdit;
        private readonly string _taskDelete;
        public string TaskDelete => _taskDelete;

        // TaskTag
        private readonly string _taskTagAddTag;
        public string TaskTagAddTag => _taskTagAddTag;
        private readonly string _taskTagRemoveTag;
        public string TaskTagRemoveTag => _taskTagRemoveTag;
        private readonly string _taskTagCompareTags;
        public string TaskTagCompareTags => _taskTagCompareTags;

        // TaskTagRules
        private readonly string _taskTagRuleGetall;
        public string TaskTagRuleGetAll => _taskTagRuleGetall;
        private readonly string _taskTagRuleAdd;
        public string TaskTagRuleAdd => _taskTagRuleAdd;
        private readonly string _taskTagRuleEdit;
        public string TaskTagRuleEdit => _taskTagRuleEdit;
        private readonly string _taskTagRuleDelete;
        public string TaskTagRuleDelete => _taskTagRuleDelete;

        public Paths(string basepath)
        {
            var account = "/Account/";
            _register = basepath + account + "Register";
            _login = basepath + account + "Login";
            _confirmEmail = basepath + account + "ConfirmEmail";
            _changeUserEmail = basepath + account + "ChangeUserEmail";
            _confirmEmailAfterChange = basepath + account + "ConfirmEmailAfterChange";
            _changeUserPassword = basepath + account + "ChangeUserPassword";
            _deleteUser = basepath + account + "DeleteUser";

            var workspace = "/Workspaces/";
            _workspaceGetall = basepath + workspace + "GetallByUserId";
            _workspaceGetOne = basepath + workspace + "GetOneById";
            _workspaceAdd = basepath + workspace + "Create";
            _workspaceDelete = basepath + workspace + "Delete";
            _workspaceEdit = basepath + workspace + "Update";

            var tag = "/tags/";
            _tagGetall = basepath + tag + "GetAll";
            _tagAdd = basepath + tag + "Create";
            _tagEdit = basepath + tag + "Update";
            _tagDelete = basepath + tag + "Delete";

            var task = "/task/";
            _taskGetall = basepath + task + "GetAllByWorkspace";
            _taskGetById = basepath + task + "GetOneById";
            _taskAdd = basepath + task + "Create";
            _taskEdit = basepath + task + "Update";
            _taskDelete = basepath + task + "Delete";

            var taskTag = "/taskTag/";
            _taskTagAddTag = basepath + taskTag + "AddTag";
            _taskTagRemoveTag = basepath + taskTag + "RemoveTag";
            _taskTagCompareTags = basepath + taskTag + "CompareTags";

            var taskTagRule = "/taskTagRule/";
            _taskTagRuleGetall = basepath + taskTagRule + "GetAll";
            _taskTagRuleAdd = basepath + taskTagRule + "Create";
            _taskTagRuleEdit = basepath + taskTagRule + "Update";
            _taskTagRuleDelete = basepath + taskTagRule + "Delete";
        }
    }
}