using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Core;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Api;
using SpiritTime.Shared.Helper;
using SpiritTime.Shared.Models.Account;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserEmail;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;
using SpiritTime.Shared.Models.Account.DeleteUser;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Backend.Controllers.Account
{
    /// <summary>
    /// AccountController
    /// </summary>
    [ApiController]
    [Route(ControllerNames.Account)]
    public class AccountController : ControllerHelper
    {
        private const    string                         Controller = "Account";
        private readonly IHttpContextAccessor           _httpContextAccessor;
        private readonly JwtAuthentication              _jwtAuthentication;
        private readonly ILogger<AccountController>     _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser>   _userManager;
        private readonly IUnitOfWork                    _unitOfWork;
        private readonly IMapper                        _mapper;

        /// <summary>
        /// AccountController
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="jwtAuthentication"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        public AccountController(
            UserManager<ApplicationUser>   userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtAuthentication              jwtAuthentication,
            ILogger<AccountController>     logger,
            IHttpContextAccessor           httpContextAccessor,
            IUnitOfWork                    unitOfWork, IMapper mapper) : base(httpContextAccessor)
        {
            _userManager         = userManager;
            _signInManager       = signInManager;
            _jwtAuthentication   = jwtAuthentication;
            _logger              = logger;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork          = unitOfWork;
            _mapper              = mapper;
        }

        /// <summary>
        ///     Generates a new User
        /// </summary>
        /// <param name="registerResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Send email doesnt work</response>
        /// <response code="403">
        ///     Password has to fulfill the requirements (min 6 chars long, one number, one special character, one
        ///     uppercase)
        /// </response>
        /// <response code="409">Email is already taken</response>
        [HttpPost(ApiMethod.Register)]
        public async Task<IActionResult> Register(RegisterResource registerResource)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(registerResource.Email) != null)
                    return new JsonResult(new RegisterResult {Error = "Email is already taken", Successful = false});

                var user = CreateUser(registerResource);

                var createResult = await _userManager.CreateAsync(user, registerResource.Password);

                if (!createResult.Succeeded) return Forbid("Password has to fulfill the requirements");
                await _userManager.UpdateSecurityStampAsync(user);

                var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //TODO: Email Confirmation wirklich einbauen
                await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
                //Default Workspace for each User
                await _unitOfWork.WorkspaceRepository.AddAsync(new Workspace {UserId = user.Id, Name = "Default"});
                await _unitOfWork.SaveAsync();
                //var callbackUrl = _httpContextAccessor.HttpContext.Request.Host.Value +
                //                  $"/{Controller}/ConfirmEmail?email={user.Email}&token={HttpUtility.UrlEncode(emailConfirmationCode)}";

                //var htmlContent = $@"Thank you for register. Please confirm the email by clicking this link: 
                //            <br><a href='{callbackUrl}'>Confirm new email</a>";

                //var sendEmailResult = await _emailSender.SendEmailAsync(user.Email, "VocaDrill Register", htmlContent);

                //if (!sendEmailResult)
                //{
                //    await _userManager.DeleteAsync(user);
                //    return new JsonResult(new RegisterResult
                //    {
                //        Successful = false,
                //        Error = "Our Email Server is currently busy, please try again!"
                //    });
                //}
                return new JsonResult(new RegisterResult {Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new RegisterResult {Error = ex.Message, Successful = false});
            }
        }

        private static ApplicationUser CreateUser(RegisterResource registerResource)
        {
            return new ApplicationUser
            {
                UserName = registerResource.Email,
                Email    = registerResource.Email
            };
        }


        /// <summary>
        ///     Login User
        /// </summary>
        /// <param name="authenticationResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        [HttpPost(ApiMethod.Login)]
        public async Task<IActionResult> Login(AuthenticationResource authenticationResource)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(authenticationResource.Email,
                authenticationResource.Password, false, false);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return new JsonResult(new AuthenticationResult {Successful = false, Error = "Your Account is locked."});
                if (signInResult.IsNotAllowed)
                    return new JsonResult(new AuthenticationResult {Successful = false, Error = "Your account is not allowed. Please confirm your Email."});
                return new JsonResult(new AuthenticationResult {Successful = false, Error = "Invalid email or password"});
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == authenticationResource.Email);

            var jwt              = _jwtAuthentication.GenerateToken(user);
            var defaultWorkspace = await _unitOfWork.WorkspaceRepository.GetUniqueByAsync(x => x.UserId == user.Id);
            if (defaultWorkspace != null) return new JsonResult(new AuthenticationResult {Successful = true, Token = jwt, WorkspaceId = defaultWorkspace.Id});

            defaultWorkspace = new Workspace {UserId = user.Id, Name = "Default"};
            await _unitOfWork.WorkspaceRepository.AddAsync(defaultWorkspace);
            await _unitOfWork.SaveAsync();

            return new JsonResult(new AuthenticationResult {Successful = true, Token = jwt, WorkspaceId = defaultWorkspace.Id});
            //return Ok(jwt);
        }

        /// <summary>
        ///     Sets the email confirmation flag true, if the token is valid.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <response code="200">Returns Confirmation done</response>
        /// <response code="404">Returns Something went wrong!</response>
        [HttpGet(ApiMethod.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user               = await _userManager.FindByEmailAsync(email);
            var confirmationResult = _userManager.ConfirmEmailAsync(user, token).Result;

            return confirmationResult.Succeeded
                ? (IActionResult) Ok("Confirmation done")
                : NotFound("Something went wrong!");
        }

        /// <summary>
        ///     Sends an confirmationmail to the new emailaddress.
        /// </summary>
        /// <param name="changeUserEmailResource"></param>
        /// <returns></returns>
        /// <response code="200">Email sent</response>
        /// <response code="400">Send email doesnt work</response>
        /// <response code="404">User not Fount</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiMethod.ChangeUserEmail)]
        public async Task<IActionResult> ChangeUserEmail(ChangeUserEmailResource changeUserEmailResource)
        {
            var user = await _userManager.FindByEmailAsync(changeUserEmailResource.Email);

            if (user == null) return NotFound("User not found!");

            await _userManager.UpdateSecurityStampAsync(user);

            var emailConfirmationCode =
                await _userManager.GenerateChangeEmailTokenAsync(user, changeUserEmailResource.NewEmail);

            await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);

            //var callbackUrl = _httpContextAccessor.HttpContext.Request.Host.Value +
            //                  $"/{Controller}/ConfirmEmailAfterChange?email={user.Email}&token={HttpUtility.UrlEncode(emailConfirmationCode)}&newEmail={changeUserEmailResource.NewEmail}";

            //var htmlContent = $@"Thank you for updating your email. Please confirm the email by clicking this link: 
            //            <br><a href='{callbackUrl}'>Confirm new email</a>";

            //var sendEmailResult = await _emailSender.SendEmailAsync(changeUserEmailResource.NewEmail,
            //    "VocaDrill Email Change", htmlContent);

            //return sendEmailResult
            //    ? (ActionResult)Ok("Confirmation email sent")
            //    : BadRequest("Our Email Server is currently busy, please try again!");
            var jwt = _jwtAuthentication.GenerateToken(user);
            return new JsonResult(new ChangeUserEmailResult {Error = null, Successful = true, Token = jwt});
        }

        /// <summary>
        ///     Change the email and the username, if the token is valid.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        /// <response code="200">Email and Username changed</response>
        /// <response code="400">Returns Something went wrong</response>
        [HttpGet(ApiMethod.ConfirmEmailAfterChange)]
        public async Task<IActionResult> ConfirmEmailAfterChange(string email, string token, string newEmail)
        {
            var user              = await _userManager.FindByEmailAsync(email);
            var changeEmailResult = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (changeEmailResult.Succeeded)
                changeEmailResult = await _userManager.SetUserNameAsync(user, newEmail);

            return changeEmailResult.Succeeded
                ? (IActionResult) Ok("Your email has changed")
                : BadRequest("Something went wrong!");
        }

        /// <summary>
        ///     Changes the user password.
        /// </summary>
        /// <param name="changeUserPasswordResource"></param>
        /// <returns></returns>
        /// <response code="200">Password successfully changed</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="409">Password rules are broken</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(ApiMethod.ChangeUserPassword)]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordResource changeUserPasswordResource)
        {
            //TODO : Include check for authenticated user
            var userId = GetUserId();
            var user   = await _unitOfWork.ApplicationUserRepository.GetUniqueByAsync(x => x.Id == userId);
            if (user.Email != changeUserPasswordResource.Email)
                return new JsonResult(
                    new ChangeUserPasswordResult{Error = new List<string>{"The User does not have the permission to change this mail adress!"}, 
                        Successful = false});
            
            //var user = await _userManager.FindByEmailAsync(changeUserPasswordResource.Email);
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changeUserPasswordResource.Password,
                changeUserPasswordResource.NewPassword);

            var result = !changePasswordResult.Succeeded
                ? new ChangeUserPasswordResult
                {
                    Error      = changePasswordResult.Errors.ToList().Select(x => x.ToString()).ToList(),
                    Successful = false
                }
                : new ChangeUserPasswordResult { Error = null, Successful = true };

            return new JsonResult(result);
        }

        /// <summary>
        ///     Deletes an User
        /// </summary>
        /// <param name="deleteUserResource"></param>
        /// <returns></returns>
        /// <response code="200">User successfully deleted</response>
        /// <response code="400">Delete user failed</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(ApiMethod.DeleteOwnUser)]
        public async Task<IActionResult> DeleteOwnUser(DeleteUserResource deleteUserResource)
        {
            var userId = GetUserId();
            var user   = await _unitOfWork.ApplicationUserRepository.GetUniqueByAsync(x => x.Id == userId);
            if(user.Email != deleteUserResource.Email)
                return new JsonResult(new DeleteUserResult{Error = new List<string>{"You are not authenticated to delete this user."}, Successful = false});
            //var user         = await _userManager.FindByEmailAsync(deleteUserResource.Email);
            var deleteResult = await _userManager.DeleteAsync(user);

            var result = !deleteResult.Succeeded 
                ? new DeleteUserResult {Error = deleteResult.Errors.ToList().Select(x => x.Description).ToList(), Successful = false} 
                : new DeleteUserResult {Successful = true};

            return new JsonResult(result);
        }


        /// <summary>
        /// Gets the Info of a user
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiMethod.GetUserInfo)]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userId   = GetUserId();
                var user     = await _unitOfWork.ApplicationUserRepository.GetUniqueByAsync(x => x.Id == userId);
                var userInfo = _mapper.Map<UserInfo>(user);
                return new JsonResult(new UserInfoResult {UserInfo = userInfo, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new UserInfoResult {Error = ex.Message, Successful = true});
            }
        }

        /// <summary>
        /// Gets the Info of a user
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiMethod.EditUserInfo)]
        public async Task<IActionResult> EditUserInfo(UserInfo info)
        {
            try
            {
                var userId = GetUserId();
                var user   = await _unitOfWork.ApplicationUserRepository.GetUniqueByAsync(x => x.Id == userId);
                user.Firstname = string.IsNullOrEmpty(info.Firstname) ? "" : info.Firstname;
                user.Lastname  = string.IsNullOrEmpty(info.Lastname) ? "" : info.Lastname;
                _unitOfWork.ApplicationUserRepository.Update(user);
                await _unitOfWork.SaveAsync();

                var userInfo = _mapper.Map<UserInfo>(user);
                return new JsonResult(new UserInfoResult {UserInfo = userInfo, Successful = true});
            }
            catch (Exception ex)
            {
                return new JsonResult(new UserInfoResult {Error = ex.Message, Successful = true});
            }
        }
    }
}