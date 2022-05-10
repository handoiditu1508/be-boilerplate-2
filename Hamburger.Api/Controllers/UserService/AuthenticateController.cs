using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.Requests.UserService;
using Hamburger.Models.Responses.UserService;
using Hamburger.Services.Abstractions.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hamburger.Api.Controllers.UserService
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticateController : CustomControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(IUserService userService, ILogger<AuthenticateController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="userAgent">Get from "User-Agent" header to create refresh token, not required.</param>
        /// <returns>Json Web Token.</returns>
        [HttpPost]
        [Route(nameof(Login))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"];
                var result = _userService.Login(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, null);
                return Unauthorized(ex.ToSimpleError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Register user to user role.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="middleName">Middle name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="username">Username.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="roles">List of roles registered for users, not required.</param>
        /// <param name="userAgent">Get from "User-Agent" header to create refresh token, not required.</param>
        /// <returns>Json Web Token.</returns>
        [HttpPost]
        [Route(nameof(RegisterUser))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterRequest request)
        {
            try
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"];
                var result = _userService.RegisterUser(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Refresh JWT.
        /// </summary>
        /// <param name="accessToken">Outdated access token.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="userAgent">Get from "User-Agent" header to create refresh token, not required.</param>
        /// <returns>Json Web Token.</returns>
        [HttpPost]
        [Route(nameof(RefreshToken))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"];
                var result = await _userService.RefreshToken(request);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, null);
                return Unauthorized(ex.ToSimpleError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Delete user's login session in database.
        /// </summary>
        /// <param name="userId">Will get from claim, not required.</param>
        /// <param name="refreshToken">Refresh token.</param>
        [HttpPost]
        [Route(nameof(Logout))]
        [Authorize]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            try
            {
                request.UserId = UserId;
                await _userService.Logout(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }
    }
}
