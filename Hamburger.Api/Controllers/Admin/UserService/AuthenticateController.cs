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
        ///        "roles": ["Admin", "User"],// List of roles registered for users.
        ///        "userAgent": "string"// Get from "User-Agent" header to create refresh token, not required.
        ///     }
        ///
        /// </remarks>
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
                var result = await _userService.Register(request);
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
                var result = await _userService.RegisterAdmin(request);
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
