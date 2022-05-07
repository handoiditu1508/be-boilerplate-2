using AutoMapper;
using Hamburger.Helpers;
using Hamburger.Helpers.Abstractions;
using Hamburger.Helpers.Comparers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.Entities;
using Hamburger.Models.FilterModels;
using Hamburger.Models.Requests.UserService;
using Hamburger.Models.Responses.UserService;
using Hamburger.Models.UserService;
using Hamburger.Repository.Abstraction.Repositories;
using Hamburger.Services.Abstractions.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hamburger.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IValidationHelper _validationHelper;
        private readonly IMapper _mapper;
        private readonly ILoginSessionRepository _loginSessionRepository;

        public UserService(IUserRepository userRepository,
            IRoleRepository roleRepository,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IValidationHelper validationHelper,
            IMapper mapper,
            ILoginSessionRepository loginSessionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _validationHelper = validationHelper;
            _mapper = mapper;
            _loginSessionRepository = loginSessionRepository;
        }

        public async Task DeleteUser(int id)
        {
            if (id < 1)
                throw CustomException.Validation.PropertyIsInvalid(nameof(id));

            await _userRepository.Remove(id);
        }

        private async Task DeleteLoginSessions(IEnumerable<Guid> loginSessionIds)
        {
            await _loginSessionRepository.RemoveMany(loginSessionIds.Select(id => new Guid[] { id }));
        }

        private string GenerateRefreshToken() => Guid.NewGuid().ToString();

        private async Task<JwtSecurityToken> GenerateAccessToken(User user, IEnumerable<Role> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // add claims for role claims
            var claims = new List<Claim>();
            var tasks = roles.Select(async role =>
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claims.AddRange(roleClaims);
            });
            await Task.WhenAll(tasks);
            authClaims.AddRange(claims.Distinct(new ClaimComparer()));

            // add claims for roles
            roles.ForEach(role => authClaims.Add(new Claim(ClaimTypes.Role, role.Name)));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Jwt.Secret));

            var token = new JwtSecurityToken(
                issuer: AppSettings.Jwt.Issuer,
                audience: AppSettings.Jwt.Audience,
                expires: DateTime.Now.AddHours(AppSettings.Jwt.Expiration),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        private async Task<LoginSession> CreateRefreshToken(int userId, string userAgent)
        {
            var refreshToken = new LoginSession
            {
                ExpirationDate = DateTime.UtcNow.AddHours(AppSettings.Jwt.RefreshTokenExpiration),
                RefreshToken = GenerateRefreshToken(),
                UserAgent = userAgent,
                UserId = userId
            };
            refreshToken = await _loginSessionRepository.Add(refreshToken);
            return refreshToken;
        }

        private async Task<LoginResponse> CreateLoginResponse(User user, string userAgent, IEnumerable<LoginSession> refreshTokens = null)
        {
            var roles = await _roleRepository.GetUserRoles(user.Id);
            var token = await GenerateAccessToken(user, roles);

            // create refresh token and store refresh token
            var refreshToken = await CreateRefreshToken(user.Id, userAgent);

            // delete outdate refresh token
            if (!refreshTokens.IsNullOrEmpty())
                await DeleteLoginSessions(refreshTokens.Where(t => t.ExpirationDate <= DateTime.UtcNow).Select(t => t.Id));

            var result = new LoginResponse
            {
                Expiration = token.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = _mapper.Map<LoginUserData>(user),
                RefreshToken = refreshToken.RefreshToken,
                RefreshTokenExpiration = refreshToken.ExpirationDate
            };
            result.User.Roles = roles.Select(r => r.Name);

            return result;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (request == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request));

            if (request.Username.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Username));

            if (request.Password.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Password));

            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
                throw CustomException.Authenticate.UserNotFound;

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw CustomException.Authenticate.IncorrectPassword;

            var refreshTokens = await _loginSessionRepository.GetUserLoginSessions(user.Id);

            return await CreateLoginResponse(user, request.UserAgent, refreshTokens);
        }

        private async Task SeedAdminRoleIfNotExist()
        {
            if (await _roleManager.RoleExistsAsync(EnumDefaultRole.Admin.GetDescription()))
                return;

            // create role
            var createRoleResult = await _roleManager.CreateAsync(new Role(EnumDefaultRole.Admin.GetDescription()));
            if (!createRoleResult.Succeeded)
            {
                var errorInfo = createRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw CustomException.Authenticate.AddRolesFailed(string.Join(", ", errorInfo));
            }

            // get created role
            var role = await _roleManager.FindByNameAsync(EnumDefaultRole.Admin.GetDescription());

            // add claims to role
            var tasks = typeof(PermissionClaimValues).GetFields().Select(f => _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, f.GetValue(null).ToString())));
            var addClaimsresults = await Task.WhenAll(tasks);
            if (addClaimsresults.Any(r => !r.Succeeded))
                throw CustomException.Authenticate.AddRolesFailed(string.Empty);
        }

        private async Task SeedUserRoleIfNotExist()
        {
            if (await _roleManager.RoleExistsAsync(EnumDefaultRole.User.GetDescription()))
                return;

            // create role
            var createRoleResult = await _roleManager.CreateAsync(new Role(EnumDefaultRole.User.GetDescription()));
            if (!createRoleResult.Succeeded)
            {
                var errorInfo = createRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw CustomException.Authenticate.AddRolesFailed(string.Join(", ", errorInfo));
            }

            // get created role
            var role = await _roleManager.FindByNameAsync(EnumDefaultRole.User.GetDescription());

            // add claims to role
            var tasks = new List<Task<IdentityResult>>
            {
                _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, PermissionClaimValues.ViewUsers)),
                _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, PermissionClaimValues.UpdateUsers))
            };
            var addClaimsresults = await Task.WhenAll(tasks);
            if (addClaimsresults.Any(r => !r.Succeeded))
                throw CustomException.Authenticate.AddRolesFailed(string.Empty);
        }

        private void ReformatRegisterRequest(RegisterRequest request)
        {
            request.Email = request.Email.Trim().ToLower();
            request.Username = request.Username.Trim().ToLower();
            request.FirstName = request.FirstName.Trim().ToTitleCase();
            request.MiddleName = request.MiddleName?.Trim().ToTitleCase() ?? null;
            request.LastName = request.LastName.Trim().ToTitleCase();
            request.PhoneNumber = request.PhoneNumber.Trim();
        }

        public async Task<LoginResponse> Register(RegisterRequest request)
        {
            if (request == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request));

            if (request.FirstName.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.FirstName));

            if (request.LastName.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.LastName));

            if (request.Username.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Username));

            if (request.Password.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Password));

            if (request.Email.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Email));

            if (request.PhoneNumber.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.PhoneNumber));

            if (request.Roles.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.Roles));

            if (!_validationHelper.IsValidEmail(request.Email))
                throw CustomException.Validation.InvalidEmail;

            if (!_validationHelper.IsValidPhoneNumber(request.PhoneNumber))
                throw CustomException.Validation.InvalidPhoneNumber;

            // check user has already existed
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
                throw CustomException.Authenticate.UserExisted;

            // check roles valid
            var rolesCount = await _roleManager.Roles.Where(r => request.Roles.Any(r2 => r2 == r.Name)).CountAsync();
            if (rolesCount != request.Roles.Count())
                throw CustomException.Authenticate.InvalidRole;

            // create new user
            ReformatRegisterRequest(request);
            user = _mapper.Map<User>(request);
            user.SecurityStamp = Guid.NewGuid().ToString();

            var registerResult = await _userManager.CreateAsync(user, request.Password);
            if (!registerResult.Succeeded)
            {
                var errorInfo = registerResult.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw CustomException.Authenticate.RegisterFailed(string.Join(", ", errorInfo));
            }

            // add roles to user
            var addRolesResult = await _userManager.AddToRolesAsync(user, request.Roles);
            if (!addRolesResult.Succeeded)
            {
                var errorInfo = registerResult.Errors.Select(e => $"{e.Code}: {e.Description}");
                throw CustomException.Authenticate.AddRolesFailed(string.Join(", ", errorInfo));
            }

            // create new JWT and refresh token
            return await CreateLoginResponse(user, request.UserAgent);
        }

        public async Task<LoginResponse> RegisterUser(RegisterRequest request)
        {
            if (request != null)
                request.Roles = new string[] { EnumDefaultRole.User.GetDescription() };

            await SeedUserRoleIfNotExist();

            return await Register(request);
        }

        public async Task<LoginResponse> RegisterAdmin(RegisterRequest request)
        {
            if (request != null)
                request.Roles = new string[] { EnumDefaultRole.Admin.GetDescription() };

            await SeedAdminRoleIfNotExist();

            return await Register(request);
        }

        public async Task<UserViewModel> GetById(int id)
        {
            if (id < 1)
                throw CustomException.Validation.PropertyIsInvalid(nameof(id));

            var user = await _userRepository.Get(id);
            var result = _mapper.Map<UserViewModel>(user);
            return result;
        }

        public async Task<IEnumerable<UserViewModel>> GetByFilter(UserFilterModel filter)
        {
            if (filter == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(filter));

            var users = await _userRepository.Get(filter);
            var result = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return result;
        }

        private void ReformatUserUpdateRequest(UserUpdateRequest request)
        {
            request.FirstName = request.FirstName.Trim().ToTitleCase();
            request.MiddleName = request.MiddleName?.Trim().ToTitleCase() ?? null;
            request.LastName = request.LastName.Trim().ToTitleCase();
        }

        public async Task UpdateUser(UserUpdateRequest request)
        {
            if (request == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request));

            if (request.Id < 1)
                throw CustomException.Validation.PropertyIsInvalid(nameof(request.Id));

            if (request.FirstName.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.FirstName));

            if (request.LastName.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.LastName));

            var user = await _userRepository.Get(request.Id);

            ReformatUserUpdateRequest(request);
            user.FirstName = request.FirstName;
            user.MiddleName = request.MiddleName;
            user.LastName = request.LastName;

            await _userRepository.Update(user);
        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest request)
        {
            if (request == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request));

            if (request.AccessToken.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.AccessToken));

            if (request.RefreshToken.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.RefreshToken));

            // validate access token
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);

            // get user
            string username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw CustomException.Authenticate.UserNotFound;

            // get all user's refresh tokens in database
            var refreshTokens = await _loginSessionRepository.GetUserLoginSessions(user.Id);

            // delete current refresh token
            var currentRefreshToken = refreshTokens.FirstOrDefault(t => t.RefreshToken == request.RefreshToken);

            if (currentRefreshToken == null)
                throw CustomException.Authenticate.LoginSessionExpired;

            await _loginSessionRepository.Remove(currentRefreshToken.Id);

            // throw error if currentRefreshToken is expired
            if (currentRefreshToken.ExpirationDate < DateTime.UtcNow)
                throw CustomException.Authenticate.LoginSessionExpired;

            // create new JWT and refresh token
            return await CreateLoginResponse(user, request.UserAgent, refreshTokens.Where(t => t.RefreshToken != request.RefreshToken));
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Jwt.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (principal == null || securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw CustomException.Authenticate.InvalidAccessToken;

            return principal;
        }

        public async Task Logout(LogoutRequest request)
        {
            if (request.UserId < 1)
                throw CustomException.Validation.PropertyIsInvalid(nameof(request.UserId));

            if (request.RefreshToken.IsNullOrWhiteSpace())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(request.RefreshToken));

            // get all user's refresh tokens in database
            var refreshTokens = await _loginSessionRepository.GetUserLoginSessions(request.UserId);

            // get all outdated refresh token's Id and target refresh token's Id
            var idsToRemove = refreshTokens
                .Where(t => t.ExpirationDate <= DateTime.UtcNow || t.RefreshToken == request.RefreshToken).Select(t => t.Id);

            // remove refresh tokens
            await DeleteLoginSessions(idsToRemove);
        }

        public async Task<int> Count(UserFilterModel filter)
        {
            if (filter == null)
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(filter));

            var result = await _userRepository.GetTotalCount(filter);

            return result;
        }
    }
}
