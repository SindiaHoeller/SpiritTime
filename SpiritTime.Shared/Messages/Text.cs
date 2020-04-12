using System;
using SpiritTime.Shared.Helper;

namespace SpiritTime.Shared.Messages
{
    public static class Text
    {
        public const string Loading = "... Loading ...";
        public const string Done = "Done";
        public const string Confirm = "Confirm";
        public const string Abort = "Abort";
        public const string ConfirmDeletion = "Are you sure, that you want to delete this item?";
        public const string NoElements = "No Element available. Please create one first.";
        public const string Save = "Save";

        public const string Options = "Options";

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
        

    }
}
