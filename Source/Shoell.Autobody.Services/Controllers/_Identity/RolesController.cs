using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Identity.Web.Resource;
using Microsoft.OData;
using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Services
{
    public class RolesController(RoleRepository repository) : CoreController<Role>
    {
        protected override string[] ReadScopes => [RoleRoles.Read];

        protected override RoleRepository Repository => repository;

        // GET: odata/Roles({key})
        [HttpGet]
        [EnableQuery]
        public IActionResult Get([FromODataUri] Guid key)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ReadScopes);

            try
            {
                var entities = Repository
                    .Get()
                    .Where(e => e.Id == key);

                if (!entities.Any())
                    return NotFound();

                try
                {
                    return Ok(SingleResult.Create(entities));
                }
                catch (Exception ex)
                {
                    return ReturnODataErrorResult(ex);
                }
            }
            catch
            {
                return Unauthorized(new ODataError { Code = "403" });
            }
        }
    }
}
