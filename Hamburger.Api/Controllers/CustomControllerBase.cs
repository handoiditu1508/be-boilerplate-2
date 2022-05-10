using Hamburger.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Hamburger.Api.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public int UserId
        {
            get
            {
                var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return claim != null ? int.Parse(claim.Value) : throw CustomException.Authenticate.InvalidAccessToken;
            }
        }

        public string UserName => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }
}
