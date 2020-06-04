using System.Threading.Tasks;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;
using SpiritTime.Shared.Models.Account.DeleteUser;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Frontend.Services.AuthServices
{
    public interface IAuthService
    {
        Task<AuthenticationResult> LoginAsync(AuthenticationResource user);
        Task<RegisterResult> RegisterUserAsync(RegisterResource user);
        //Task<AuthenticationResult> RefreshTokenAsync(AuthenticationResource refreshRequest);
        //Task<AuthenticationResult> GetUserByAccessTokenAsync(string accessToken);
        Task Logout();
        Task<UserInfoResult> GetUserInfo();
        Task<UserInfoResult> UpdateUserInfo(UserInfo userInfo);
        Task<ChangeUserPasswordResult> ChangeUserPassword(ChangeUserPasswordResource pwChangeInfo);
        Task<DeleteUserResult> DeleteOwnUser(DeleteUserResource deleteUser);
    }
}
