namespace SpiritTime.Shared.Api
{
    public static class ApiMethod
    {
        public const string Register = "Register";
        public const string Login = "Login";
        public const string ConfirmEmail = "ConfirmEmail";
        public const string ChangeUserEmail = "ChangeUserEmail";
        public const string ConfirmEmailAfterChange = "ConfirmEmailAfterChange";
        public const string ChangeUserPassword = "ChangeUserPassword";
        public const string DeleteUser = "DeleteUser";
        
        public const string GetAll = "GetAll";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        
        public const string GetAllByWorkspace = "GetAllByWorkspace";
        public const string GetOneById = "GetOneById";
        public const string GetFirstOrDefault = "GetFirstOrDefault";
        public const string GetallByUserId = "GetallByUserId";
        
        public const string AddTag = "AddTag";
        public const string RemoveTag = "RemoveTag";
        public const string CompareTags = "CompareTags";
    }
}