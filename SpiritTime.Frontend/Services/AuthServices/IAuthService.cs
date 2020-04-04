using System.Threading.Tasks;
using SpiritTime.Shared.Models.Account.Authentication;
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
    }
}
