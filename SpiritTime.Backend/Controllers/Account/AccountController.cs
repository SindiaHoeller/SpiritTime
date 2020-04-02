using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpiritTime.Backend.Infrastructure.Jwt;
using SpiritTime.Core.Entities;
using SpiritTime.Shared.Models.Account.Authentication;
using SpiritTime.Shared.Models.Account.ChangeUserEmail;
using SpiritTime.Shared.Models.Account.ChangeUserPassword;
using SpiritTime.Shared.Models.Account.DeleteUser;
using SpiritTime.Shared.Models.Account.Registration;

namespace SpiritTime.Backend.Controllers.Account
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private const string Controller = "Account";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtAuthentication _jwtAuthentication;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtAuthentication jwtAuthentication,
            ILogger<AccountController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtAuthentication = jwtAuthentication;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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
        [HttpPost]
        public async Task<IActionResult> Register(RegisterResource registerResource)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(registerResource.Email) != null)
                    return new JsonResult(new RegisterResult { Error = "Email is already taken", Successful = false });

                var user = CreateUser(registerResource);

                var createResult = await _userManager.CreateAsync(user, registerResource.Password);

                if (!createResult.Succeeded) return Forbid("Password has to fulfill the requirements");
                await _userManager.UpdateSecurityStampAsync(user);
                
                var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //TODO: Email Confirmation wirklich einbauen
                await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
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
                return new JsonResult(new RegisterResult { Successful = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new RegisterResult { Error = ex.Message, Successful = false });
            }
        }

        private static ApplicationUser CreateUser(RegisterResource registerResource)
        {
            return new ApplicationUser
            {
                UserName = registerResource.Email,
                Email = registerResource.Email
            };
        }


        /// <summary>
        ///     Login User
        /// </summary>
        /// <param name="authenticationResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns a JsonWebToken</response>
        /// <response code="400">Invalid credentials</response>
        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationResource authenticationResource)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(authenticationResource.Email,
                authenticationResource.Password, false, false);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return new JsonResult(new AuthenticationResult { Successful = false, Error = "Your Account is locked." });
                if (signInResult.IsNotAllowed)
                    return new JsonResult(new AuthenticationResult { Successful = false, Error = "Your account is not allowed. Please confirm your Email." });
                return new JsonResult(new AuthenticationResult { Successful = false, Error = "Invalid email or password" });
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(r => r.Email == authenticationResource.Email);

            var jwt = _jwtAuthentication.GenerateToken(user);


            return new JsonResult(new AuthenticationResult { Successful = true, Token = jwt });
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
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var confirmationResult = _userManager.ConfirmEmailAsync(user, token).Result;

            return confirmationResult.Succeeded
                ? (IActionResult)Ok("Confirmation done")
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
        [Authorize]
        [HttpGet]
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
            return new JsonResult(new ChangeUserEmailResult{Error = null, Successful = true, Token = jwt});
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
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAfterChange(string email, string token, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var changeEmailResult = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (changeEmailResult.Succeeded)
                changeEmailResult = await _userManager.SetUserNameAsync(user, newEmail);

            return changeEmailResult.Succeeded
                ? (IActionResult)Ok("Your email has changed")
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordResource changeUserPasswordResource)
        {
            var user = await _userManager.FindByEmailAsync(changeUserPasswordResource.Email);
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changeUserPasswordResource.Password,
                changeUserPasswordResource.NewPassword);


            var result = !changePasswordResult.Succeeded
                ? new ChangeUserPasswordResult
                {
                    Error = changePasswordResult.Errors.ToList().Select(x => x.ToString()).ToList(), 
                    Successful = false
                }
                : new ChangeUserPasswordResult {Error = null, Successful = true};

            return new JsonResult(result);

        }

        /// <summary>
        ///     Deletes an User
        /// </summary>
        /// <param name="deleteUserResource"></param>
        /// <returns></returns>
        /// <response code="200">User successfully deleted</response>
        /// <response code="400">Delete user failed</response>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(DeleteUserResource deleteUserResource)
        {
            var user = await _userManager.FindByEmailAsync(deleteUserResource.Email);
            var deleteResult = await _userManager.DeleteAsync(user);


            var result = !deleteResult.Succeeded ? 
                new DeleteUserResult { Error = deleteResult.Errors.ToList().Select(x=>x.Description).ToList(), Successful = false} : 
                new DeleteUserResult { Successful = true};

            return new JsonResult(result);
        }
    }
}
