using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.Requests.UserService;
using Hamburger.Models.UserService;
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
    public class UserController : CustomControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get user information by id claim.
        /// </summary>
        /// <returns>UserViewModel.</returns>
        [HttpGet]
        [Authorize(PermissionClaimPolicies.ViewUsers)]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserViewModel>> Get()
        {
            try
            {
                return Ok(await _userService.GetById(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Get full details user information by id claim.
        /// </summary>
        /// <returns>UserFullDetails.</returns>
        [HttpGet]
        [Route(nameof(FullDetails))]
        [Authorize(PermissionClaimPolicies.ViewUsers)]
        [ProducesResponseType(typeof(UserFullDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserFullDetails>> FullDetails()
        {
            try
            {
                return Ok(await _userService.GetFullDetails(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Update user information.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "id": 1,// Will get from claim, not required.
        ///        "firstName": "John",// New first name.
        ///        "middleName": "Test",// New middle name.
        ///        "lastName": "Doe"// New last name.
        ///     }
        ///
        /// </remarks>
        [HttpPut]
        [Authorize(PermissionClaimPolicies.UpdateUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(UserUpdateRequest request)
        {
            try
            {
                request.Id = UserId;
                await _userService.UpdateUser(request);
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
