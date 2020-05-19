namespace SpiritTime.Shared.Api
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
        public string GetUserInfo => _getUserInfo;
        private readonly string _getUserInfo;


        // Workspace
        private readonly string _workspaceGetall;
        public string WorkspaceGetAll => _workspaceGetall;
        private readonly string _workspaceGetOne;
        public string WorkspaceGetOne => _workspaceGetOne;
        private readonly string _workspaceGetFirstOrDefault;
        public string WorkspaceGetFirstOrDefault => _workspaceGetFirstOrDefault;
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
        private readonly string _taskGetAllByWorkspaceLimitedByDays;
        public string TaskGetAllByWorkspaceLimitedByDays => _taskGetAllByWorkspaceLimitedByDays;
        private readonly string _taskGetById;
        public string TaskGetById => _taskGetById;
        private readonly string _taskAdd;
        public string TaskAdd => _taskAdd;
        private readonly string _taskEdit;
        public string TaskEdit => _taskEdit;
        private readonly string _taskUpdateTags;
        public string TaskUpdateTags => _taskUpdateTags;
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
            var account = ControllerNames.Account + "/";
            _register = basepath + account + ApiMethod.Register;
            _login = basepath + account + ApiMethod.Login;
            _confirmEmail = basepath + account + ApiMethod.ConfirmEmail;
            _changeUserEmail = basepath + account + ApiMethod.ChangeUserEmail;
            _confirmEmailAfterChange = basepath + account + ApiMethod.ConfirmEmailAfterChange;
            _changeUserPassword = basepath + account + ApiMethod.ChangeUserPassword;
            _deleteUser = basepath + account + ApiMethod.DeleteUser;
            _getUserInfo = basepath + account + ApiMethod.GetUserInfo;

            var workspace = ControllerNames.Workspaces + "/";;
            _workspaceGetall = basepath + workspace + ApiMethod.GetallByUserId;
            _workspaceGetOne = basepath + workspace + ApiMethod.GetOneById;
            _workspaceAdd = basepath + workspace + ApiMethod.Create;
            _workspaceDelete = basepath + workspace + ApiMethod.Delete;
            _workspaceEdit = basepath + workspace + ApiMethod.Update;
            _workspaceGetFirstOrDefault = basepath + workspace + ApiMethod.GetFirstOrDefault;

            var tag = ControllerNames.Tags + "/";
            _tagGetall = basepath + tag + ApiMethod.GetAll;
            _tagAdd = basepath + tag + ApiMethod.Create;
            _tagEdit = basepath + tag + ApiMethod.Update;
            _tagDelete = basepath + tag + ApiMethod.Delete;

            var task = ControllerNames.Tasks + "/";
            _taskGetall = basepath + task + ApiMethod.GetAllByWorkspace;
            _taskGetAllByWorkspaceLimitedByDays = basepath + task + ApiMethod.GetAllByWorkspaceLimitedByDays;
            _taskGetById = basepath + task + ApiMethod.GetOneById;
            _taskAdd = basepath + task + ApiMethod.Create;
            _taskEdit = basepath + task + ApiMethod.Update;
            _taskUpdateTags = basepath + task + ApiMethod.UpdateTagsForTask;
            _taskDelete = basepath + task + ApiMethod.Delete;
            

            var taskTag = ControllerNames.TaskTag + "/";
            _taskTagAddTag = basepath + taskTag + ApiMethod.AddTag;
            _taskTagRemoveTag = basepath + taskTag + ApiMethod.RemoveTag;
            _taskTagCompareTags = basepath + taskTag + ApiMethod.CompareTags;

            var taskTagRule = ControllerNames.TaskTagRules + "/";
            _taskTagRuleGetall = basepath + taskTagRule + ApiMethod.GetAll;
            _taskTagRuleAdd = basepath + taskTagRule + ApiMethod.Create;
            _taskTagRuleEdit = basepath + taskTagRule + ApiMethod.Update;
            _taskTagRuleDelete = basepath + taskTagRule + ApiMethod.Delete;
        }
    }
}