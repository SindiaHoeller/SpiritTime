using System;
using System.Collections.Generic;
using System.Text;

namespace SpiritTime.Shared.Messages
{
    public static class SuccessMsg
    {
        public const string SuccessedDeletion = "The Deletion was successful.";
        public const string SuccessedUpdate = "Updated successfully";

        public const string TimerStarted = "Timer successfully started.";
        public const string TimerStopped= "Timer successfully stopped.";

        public const string WorkspaceAdded = "Workspace successfully added: ";
        public const string WorkspaceEdited = "Workspace successfully edited: ";

        public const string TagAdded = "Tag successfully added: ";
        public const string TagEdited = "Tag successfully edited: ";
        
        public const string TaskAdded = "Task successfully added: ";
        public const string TaskEdited = "Task successfully edited: ";
        public const string TaskDeleted = "Task successfully deleted: ";
        
        public const string RuleAdded = "Rule successfully added for the Trigger: ";
        public const string RuleEdited = "Rule successfully edited for the Trigger: ";

        public const string UpdatedOptions = "Successfully updated this Option";
        public const string UpdatedUserInfo = "Successfully updated the User Infos";

        public const string TagsForTaskEdited = "Tags for Task successfully edited.";

        public const string PasswordUpdated = "Successfully updated user password";
    }
}
