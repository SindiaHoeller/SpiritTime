using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using SpiritTime.Frontend.Config;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;
using SpiritTime.Shared.Models.Account.DeleteUser;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Frontend.Services.AuthServices
{
    public class AuthService : ServiceBase<AuthenticationResource>, IAuthService
    {
        private readonly Paths _path;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(HttpClient httpClient, 
            IOptions<AppSettings> appSettings, 
            AuthenticationStateProvider provider,
            ILocalStorageService localStorageService) : base(httpClient, appSettings, localStorageService)
        {
            var appSetting = appSettings.Value;
            _path = new Paths(appSetting.BackendBaseAddress);
            httpClient.BaseAddress = new Uri(appSetting.BackendBaseAddress);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");
            
            _authenticationStateProvider = provider;
        }

        public async Task<AuthenticationResult> LoginAsync(AuthenticationResource userResource)
        {

            try
            {
                Console.WriteLine("Service.LoginAsync called..." + _path.Login);
                var result = await _httpClient.PostJsonAsync<AuthenticationResult>(_path.Login, userResource);
                Console.WriteLine("Result: " + result.Successful);
                if (result.Successful)
                {
                    var (workspaceSet, error) = await SetCurrentWorkspace(result.WorkspaceId);
                    if(workspaceSet)
                        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token, userResource.Email);
                    return workspaceSet ? result : new AuthenticationResult{ Successful = false, Error = error };
                }

                return result;
            }
            catch (Exception ex)
            {
                return new AuthenticationResult { Successful = false, Error = ex.Message };
            }
        }



        public async Task<RegisterResult> RegisterUserAsync(RegisterResource user)
        {
            try
            {
                var result = await _httpClient.PostJsonAsync<RegisterResult>(_path.Register, user);
                return result;
            }
            catch (Exception ex)
            {
                return new RegisterResult { Successful = false, Error = ex.Message };
            }
        }

        public async Task<UserInfoResult> GetUserInfo()
        {
            try
            {
                await SetAuthenticationHeader();
                return await _httpClient.GetJsonAsync<UserInfoResult>(_path.GetUserInfo);
            }
            catch (Exception ex)
            {
                return new UserInfoResult { Successful = false, Error = ex.Message };
            }
        }
        public async Task<UserInfoResult> UpdateUserInfo(UserInfo userInfo)
        {
            try
            {
                await SetAuthenticationHeader();
                return await _httpClient.PostJsonAsync<UserInfoResult>(_path.EditUserInfo, userInfo);
            }
            catch (Exception ex)
            {
                return new UserInfoResult { Successful = false, Error = ex.Message };
            }
        }
        
        public async Task<ChangeUserPasswordResult> ChangeUserPassword(ChangeUserPasswordResource pwChangeInfo)
        {
            try
            {
                await SetAuthenticationHeader();
                return await _httpClient.PutJsonAsync<ChangeUserPasswordResult>(_path.ChangeUserPassword, pwChangeInfo);
            }
            catch (Exception ex)
            {
                return new ChangeUserPasswordResult { Successful = false, Error = new List<string>{ex.Message} };
            }
        }
        public async Task<DeleteUserResult> DeleteOwnUser(DeleteUserResource deleteUser)
        {
            try
            {
                await SetAuthenticationHeader();
                return await _httpClient.PutJsonAsync<DeleteUserResult>(_path.DeleteOwnUser, deleteUser);
            }
            catch (Exception ex)
            {
                return new DeleteUserResult { Successful = false, Error = new List<string>{ex.Message} };
            }
        }


        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
