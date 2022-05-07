using AutoMapper;
using Hamburger.Models.Entities;
using Hamburger.Models.Requests.UserService;

namespace Hamburger.Models.UserService
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<User, LoginUserData>().ForMember(d => d.Roles, options => options.Ignore());
            CreateMap<RegisterRequest, User>();
        }
    }
}
