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
        /// <returns>Array of UserViewModel.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "id": 1,// User id.
        ///        "userName: "user1",// Username.
        ///        "email": "user1@example.com",// Email.
        ///        "emailConfirmed": true,// true, false or null.
        ///        "phoneNumber": "0987654321",// Phone number.
        ///        "phoneNumberConfirmed": true,// true, false or null.
        ///        "twoFactorEnabled": true,// true, false or null.
        ///        "isLockout": true,// true, false or null.
        ///        "lockoutEnabled": true,// true, false or null.
        ///        "accessFailedCount": 0,// Access failed count.
        ///        "accessFailedCountOperator": 0,// 0 - lesser than, 1 - equal to, 2 - greater than.
        ///        "name": "John",// Search by FirstName, MiddleName or LastName.
        ///        "createdDate": "1989-06-30",// Created Date.
        ///        "modifiedDate": "1989-06-30",// Modified Date.
        ///        "limit": 10,// Page's maximum items.
        ///        "offset": 20// Skip number of items.
        ///     }
        ///
        /// </remarks>
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
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "id": 1,// Id of user to update, will get from url, not required.
        ///        "firstName: "John",// New first name.
        ///        "middleName": "Test",// New middle name.
        ///        "lastName": "Doe"// New last name.
        ///     }
        ///
        /// </remarks>
        [HttpPut]
        [Route("{id}")]
        [Authorize(PermissionClaimPolicies.AdminUpdateUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleError), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, UserUpdateRequest request)
        {
            try
            {
                request.Id = id;
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
