using Microsoft.AspNetCore.Http;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;

namespace Shoell.Autobody.Data.Tests
{
    public class RoleRepositoryTests(
        AutobodyContext context,
        IHttpContextAccessor httpContextAccessor,
        RoleRepository repository
    ) : CoreRepositoryTests<Role>
    {
        protected override AutobodyContext Context => context;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected override RoleRepository Repository => repository;

        protected override string ReadPolicy => RoleRoles.Read;
    }
}
