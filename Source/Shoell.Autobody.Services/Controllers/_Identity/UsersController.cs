using Shoell.Autobody.Data;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Services
{
    public class UsersController(UserRepository repository) : BaseModelController<User>
    {
        protected override UserRepository Repository => repository;

        protected override string[] ModifyScopes => [UserRoles.Modify];

        protected override string[] RecycleScopes => [UserRoles.Recycle];

        protected override string[] ArchiveScopes => [UserRoles.Archive];

        protected override string[] ReadScopes => [UserRoles.Read];
    }
}
