using Hamburger.Models.FilterModels;
using Hamburger.Models.Requests.UserService;
using Hamburger.Models.Responses.UserService;
using Hamburger.Models.UserService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Services.Abstractions.UserService
{
    public interface IUserService
    {
        Task DeleteUser(int id);
        Task<LoginResponse> Login(LoginRequest request);
        Task<LoginResponse> Register(RegisterRequest request);
        Task<LoginResponse> RegisterUser(RegisterRequest request);
        Task<LoginResponse> RegisterAdmin(RegisterRequest request);
        Task<UserViewModel> GetById(int id);
        Task<IEnumerable<UserViewModel>> GetByFilter(UserFilterModel filter);
        Task UpdateUser(UserUpdateRequest request);
        Task<LoginResponse> RefreshToken(RefreshTokenRequest request);
        Task Logout(LogoutRequest request);
        Task<int> Count(UserFilterModel filter);
        Task<UserFullDetails> GetFullDetails(int id);
    }
}
