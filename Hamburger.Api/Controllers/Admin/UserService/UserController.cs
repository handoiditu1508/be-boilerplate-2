using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.FilterModels;
using Hamburger.Models.Requests.UserService;
using Hamburger.Models.UserService;
using Hamburger.Services.Abstractions.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Api.Controllers.Admin.UserService
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get user information by id.
        /// </summary>
        /// <param name="id">Id of user to get.</param>
        /// <returns>UserViewModel.</returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize(PermissionClaimPolicies.AdminViewUsers)]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserViewModel>> Get(int id)
        {
            try
            {
                return Ok(await _userService.GetById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Filter list user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="userName">Username</param>
        /// <param name="email">Email.</param>
        /// <param name="emailConfirmed">true, false or null.</param>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="phoneNumberConfirmed">true, false or null.</param>
        /// <param name="twoFactorEnabled">true, false or null.</param>
        /// <param name="isLockout">true, false or null.</param>
        /// <param name="lockoutEnabled">true, false or null.</param>
        /// <param name="accessFailedCount">Access failed count.</param>
        /// <param name="accessFailedCountOperator">0 - lesser than, 1 - equal to, 2 - greater than.</param>
        /// <param name="name">Search by FirstName, MiddleName or LastName.</param>
        /// <param name="createdDate">Created Date.</param>
        /// <param name="modifiedDate">Modified Date.</param>
        /// <param name="limit">Page maximum items.</param>
        /// <param name="offset">Skip number of items.</param>
        /// <returns>Array of UserViewModel.</returns>
        [HttpPost]
        [Route(nameof(FilterUser))]
        [Authorize(PermissionClaimPolicies.AdminViewUsers)]
        [ProducesResponseType(typeof(IEnumerable<UserViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> FilterUser(UserFilterModel filter)
        {
            try
            {
                var totalCount = await _userService.Count(filter);
                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(await _userService.GetByFilter(filter));
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
        /// <param name="id">Id of user to update.</param>
        /// <param name="firstName">New first name.</param>
        /// <param name="middleName">New middle name.</param>
        /// <param name="lastName">New last name.</param>
        [HttpPut]
        [Authorize(PermissionClaimPolicies.AdminUpdateUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(UserUpdateRequest request)
        {
            try
            {
                await _userService.UpdateUser(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="id">Id of User to delete.</param>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(PermissionClaimPolicies.AdminDeleteUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUser(id);
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
