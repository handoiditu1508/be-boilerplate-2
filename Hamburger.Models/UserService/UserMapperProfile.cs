using AutoMapper;
using Hamburger.Models.Entities;
using Hamburger.Models.Requests.UserService;
using System.Linq;

namespace Hamburger.Models.UserService
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<User, UserFullDetails>().ForMember(d => d.Roles, options => options.MapFrom(s => s.Roles.Select(r => r.Name)));
            CreateMap<RegisterRequest, User>();
        }
    }
}
