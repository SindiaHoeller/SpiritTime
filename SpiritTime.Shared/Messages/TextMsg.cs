using System;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Shared.Messages
{
    public static class TextMsg
    {
        public const string RegisterText = "Create a new account.";
        public const string LoginText = "Log in with your existing account.";
        
        public const string Loading = "... Loading ...";
        public const string Done = "Done";
        public const string Confirm = "Confirm";
        public const string Abort = "Abort";
        public const string ConfirmDeletion = "Are you sure, that you want to delete this item?";
        public const string NoElements = "No Element available. Please create one first.";
        public const string Save = "Save";
        public const string ChangeEmail = "Change Email";
        public const string ChangePassword = "Change Password";
        public const string DeleteUser = "Delete my User";

        public const string Options = "Options";
        public const string Task = "Task";
        public const string AccountInfo = "Account Information";

        public const string Workspace = "Workspace";
        public const string AddWorkspace = "Add " + SD.Workspace;
        public const string WorkspaceRemove = "Remove " + SD.Workspace;
        public const string WorkspaceEdit = "Edit  " + SD.Workspace;
        public const string WorkspaceName = SD.Workspace + " Name";

        public const string TaskEdit = "Edit Task";

        public const string TagAdd = "Add " + SD.Tag;
        public const string TagRemove = "Remove " + SD.Tag;
        public const string TagEdit = "Edit  " + SD.Tag;
        public const string TagName = SD.Tag + " Name";
        
        public const string TagRuleAdd = "Add " + SD.TagRule;
        public const string TagRuleRemove = "Remove " + SD.TagRule;
        public const string TagRuleEdit = "Edit  " + SD.TagRule;
        public const string TagRuleName = SD.TagRule + " Name";
        public const string TagRuleTriggerText = "Text that triggers the Rule";
        public const string TagRuleTriggerDescription = "Trigger field 'Description'";
        public const string TagRuleTriggerName = "Trigger field 'Name'";
        public const string TagRuleReplaceTrigger = "Replace triggertext in field";
        public const string DescriptionField = "Field 'description'";
        public const string NameField = "Field 'name'";
        public const string TriggerOn = "Triggers on this fields";
        public const string ReplaceTrigger = "Trigger will get replaced.";

        public const string DeleteUserMsg = "Are you sure, that you want to delete your Account?";
        public const string DeleteUserDetail = "It is NOT possible to undo this! Your account and all contained data will be immediately deleted!";
        public const string DeleteUserDetail2 = "If you are sure, please type in  '" + DeleteConfirmString +"' into the box below.";
        public const string DeleteConfirmString = "DELETE";


    }
}
