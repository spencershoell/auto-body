using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Identity.Web.Resource;
using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Services.Role_GroupDtoModels;

namespace Shoell.Autobody.Services
{
    public class Role_GroupsController(Role_GroupRepository repository) : IntersectController<Role_Group>
    {
        protected override Role_GroupRepository Repository => repository;

        protected override Expression<Func<Role_Group, bool>> IdentifierPredicate(Guid roleId, Guid groupId)
        {
            return e => e.RoleId == roleId && e.GroupId == groupId;
        }

        protected override string[] ModifyScopes => [GroupRoles.Modify];
        protected override string[] ReadScopes => [RoleRoles.Read, GroupRoles.Read];

        // GET: odata/RoleGroups(roleId=[RoleId],groupId=[GroupId])
        [HttpGet]
        [EnableQuery]
        public override IActionResult Get([FromODataUri] Guid keyroleId, [FromODataUri] Guid keygroupId, CancellationToken cancellationToken = default)
        {
            return base.Get(keyroleId, keygroupId, cancellationToken);
        }

        // DELETE: odata/RoleGroups(roleId=[RoleId],groupId=[GroupId])
        [HttpDelete]
        public override async Task<IActionResult> Delete([FromODataUri] Guid keyroleId, [FromODataUri] Guid keygroupId, CancellationToken cancellationToken = default)
        {
            return await base.Delete(keyroleId, keygroupId, cancellationToken);
        }

        // POST: odata/RoleGroups/AddToGroups
        [HttpPost]
        public async Task<IActionResult> AddToGroups(ODataActionParameters parameters, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var model = (AddRoleToGroupsDto)parameters.FirstOrDefault(e => e.Key == "model").Value;

                await Repository.AddToGroups(model.RoleId, model.GroupIds, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: odata/RoleGroups/AddToRoles
        [HttpPost]
        public async Task<IActionResult> AddToRoles(ODataActionParameters parameters, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = (AddGroupToRolesDto)parameters.FirstOrDefault(e => e.Key == "model").Value;

                await Repository.AddToRoles(model.GroupId, model.RoleIds, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }
    }
}
