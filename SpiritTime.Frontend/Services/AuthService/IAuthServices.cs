using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Frontend.Services.AuthService
{
    public interface IAuthServices
    {
        Task<AuthenticationResult> LoginAsync(AuthenticationResource user);
        Task<RegisterResult> RegisterUserAsync(RegisterResource user);
        Task<AuthenticationResult> RefreshTokenAsync(AuthenticationResource refreshRequest);
        Task<AuthenticationResult> GetUserByAccessTokenAsync(string accessToken);
        Task Logout();
    }
}
