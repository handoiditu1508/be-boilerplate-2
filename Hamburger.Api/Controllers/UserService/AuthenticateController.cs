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
        /// <returns>Json Web Token.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "username": "user1",// Username.
        ///        "password": "********",// Password.
        ///        "userAgent": "string"// Get from "User-Agent" header to create refresh token, not required.
        ///     }
        ///
        /// </remarks>
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
                var result = await _userService.Login(request);
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
        /// <returns>Json Web Token.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "firstName": "John",// First name.
        ///        "middleName": "Test",// Middle name.
        ///        "lastName": "Doe",// Last name.
        ///        "username": "user1",// Username.
        ///        "email": "user1@example.com",// Email address.
        ///        "password": "********",// Password.
        ///        "phoneNumber": "0987654321",// Phone number.
        ///        "roles": ["Admin", "User"],// List of roles registered for users, not required.
        ///        "userAgent": "string"// Get from "User-Agent" header to create refresh token, not required.
        ///     }
        ///
        /// </remarks>
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
                var result = await _userService.RegisterUser(request);
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
        /// <returns>Json Web Token.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "accessToken": "string",// Outdated access token.
        ///        "refreshToken": "string",// Refresh token.
        ///        "userAgent": "string"// Get from "User-Agent" header to create refresh token, not required.
        ///     }
        ///
        /// </remarks>
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
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "id": 1,// User Id will get from claim, not required.
        ///        "refreshToken": "string"// Refresh token.
        ///     }
        ///
        /// </remarks>
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
                request.Id = UserId;
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
