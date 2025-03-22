using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Services
{
    public class GroupsController(GroupRepository repository) : BaseModelController<Group>
    {
        protected override GroupRepository Repository => repository;


        protected override string[] ModifyScopes => [GroupRoles.Modify];

        protected override string[] RecycleScopes => [GroupRoles.Recycle];

        protected override string[] ArchiveScopes => [GroupRoles.Archive];

        protected override string[] ReadScopes => [GroupRoles.Read];
    }
}
