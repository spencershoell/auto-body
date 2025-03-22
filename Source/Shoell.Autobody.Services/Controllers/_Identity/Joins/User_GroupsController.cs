using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Identity.Web.Resource;
using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Autobody.Services.User_GroupDtoModels;

namespace Shoell.Autobody.Services
{
    public class User_GroupsController(User_GroupRepository repository) : IntersectController<User_Group>
    {
        protected override User_GroupRepository Repository => repository;

        protected override Expression<Func<User_Group, bool>> IdentifierPredicate(Guid userId, Guid groupId)
        {
            return e => e.UserId == userId && e.GroupId == groupId;
        }

        protected override string[] ModifyScopes => [UserRoles.Modify, GroupRoles.Modify];
        protected override string[] ReadScopes => [UserRoles.Read, GroupRoles.Read];


        // GET: odata/UserGroups(userId=[UserId],groupId=[GroupId])
        [HttpGet]
        [EnableQuery]
        public override IActionResult Get([FromODataUri] Guid keyuserId, [FromODataUri] Guid keygroupId, CancellationToken cancellationToken = default)
        {
            return base.Get(keyuserId, keygroupId, cancellationToken);
        }

        // DELETE: odata/UserGroups(userId=[UserId],groupId=[GroupId])
        [HttpDelete]
        public override async Task<IActionResult> Delete([FromODataUri] Guid keyuserId, [FromODataUri] Guid keygroupId, CancellationToken cancellationToken = default)
        {
            return await base.Delete(keyuserId, keygroupId, cancellationToken);
        }

        // POST: odata/UserGroups/AddToGroups
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
                var model = (AddUserToGroupsDto)parameters.FirstOrDefault(e => e.Key == "model").Value;

                await Repository.AddToGroups(model.UserId, model.GroupIds, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: odata/UserGroups/AddToUsers
        [HttpPost]
        public async Task<IActionResult> AddToUsers(ODataActionParameters parameters, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = (AddGroupToUsersDto)parameters.FirstOrDefault(e => e.Key == "model").Value;

                await Repository.AddToUsers(model.GroupId, model.UserIds, cancellationToken);
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
