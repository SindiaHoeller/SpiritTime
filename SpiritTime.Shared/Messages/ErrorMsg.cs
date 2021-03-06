namespace SpiritTime.Shared.Messages
{
    public static class ErrorMsg
    {
        public const string NotAuthorizedForAction = "You are not authorized to perform this action!";
        public const string NameCanNotBeEmpty = "The name is not allowed to be empty";
        public const string WorkspaceNotAdded = "The Workspace could not be added.";
        public const string WorkspaceNotEdited = "The Workspace could not be edited";
        public const string WorkspaceChoose = "Please choose a workspace first.";
        public const string ChooseOption = "Please choose an Option first.";

        public const string TagsCouldNotLoaded = "Could not load Tags";

        public const string TagNotAdded = "The Tag could not be added.";
        public const string TagNotEdited = "The Tag could not be edited";
        public const string TaskEmpty = "You have to fill name or description field.";

        public const string PasswordDoesNotMeetRequirements = "The password could not be changed, because it does not meet the requirements.";
        public const string PasswordDoNotMatch = "The passwords do not match";

        public const string Abort = "Aborted current action";

        public const string ProxyError = "Connection to server with this proxy settings did not work!";
    }
}