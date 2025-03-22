using Microsoft.AspNetCore.Http;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Interfaces;

namespace Shoell.Autobody.Data.Tests
{
    public class GroupRepositoryTests(
        AutobodyContext context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        GroupRepository repository
    ) : BaseRepositoryTests<Group>
    {
        protected override AutobodyContext Context => context;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected override GroupRepository Repository => repository;

        protected override string ReadPolicy => GroupRoles.Read;
        protected override string CreatePolicy => GroupRoles.Create;
        protected override string UpdatePolicy => GroupRoles.Update;
        protected override string DeletePolicy => GroupRoles.Delete;
        protected override string RecyclePolicy => GroupRoles.Recycle;
        protected override string RecoverPolicy => GroupRoles.Recover;
        protected override string PurgePolicy => GroupRoles.Purge;
        protected override string ArchivePolicy => GroupRoles.Archive;
        protected override string RestorePolicy => GroupRoles.Restore;

        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;
    }
}
