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
        

        public const string Workspace = "Workspace";
        public const string AddWorkspace = "Add " + SD.Workspace;
        public const string WorkspaceRemove = "Remove " + SD.Workspace;
        public const string WorkspaceEdit = "Edit  " + SD.Workspace;
        public const string WorkspaceName = SD.Workspace + " Name";

        public const string TagAdd = "Add " + SD.Tag;
        public const string TagRemove = "Remove " + SD.Tag;
        public const string TagEdit = "Edit  " + SD.Tag;
        public const string TagName = SD.Tag + " Name";

    }
}
