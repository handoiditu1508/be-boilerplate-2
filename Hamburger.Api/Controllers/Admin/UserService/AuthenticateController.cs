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

namespace Hamburger.Api.Controllers.Admin.UserService
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(IUserService userService, ILogger<AuthenticateController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Register user to specific roles.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="middleName">Middle name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="username">Username.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="roles">List of roles registered for users.</param>
        /// <param name="userAgent">Get from "User-Agent" header to create refresh token, not required.</param>
        /// <returns>Json Web Token.</returns>
        [HttpPost]
        [Route(nameof(Register))]
        [Authorize(PermissionClaimPolicies.AdminCreateUsers)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
        {
            try
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"];
                var result = _userService.Register(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Register user to admin role.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="middleName">Middle name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="username">Username.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">Password.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="roles">List of roles registered for users, not required</param>
        /// <param name="userAgent">Get from "User-Agent" header to create refresh token, not required.</param>
        /// <returns>Json Web Token.</returns>
        [HttpPost]
        [Route(nameof(RegisterAdmin))]
        [Authorize(PermissionClaimPolicies.AdminCreateUsers)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> RegisterAdmin(RegisterRequest request)
        {
            try
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"];
                var result = _userService.RegisterAdmin(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }
    }
}
