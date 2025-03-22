using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Identity.Web.Resource;
using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;

namespace Shoell.Autobody.Services
{
    public class MeController(UserRepository userRepository) : ODataController
    {
        protected UserRepository UserRepository => userRepository;

        [HttpGet]
        [EnableQuery]
        public IActionResult Roles()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope([RoleRoles.Read]);

            return Ok(UserRepository.GetRoles());
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Info(CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope([UserRoles.Read]);

            return Ok(await UserRepository.GetLoggedInUserAsync(cancellationToken));
        }
    }
}
