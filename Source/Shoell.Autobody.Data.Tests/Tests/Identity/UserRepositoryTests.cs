using Microsoft.AspNetCore.Http;
using Shoell.Autobody.Identity;
using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Interfaces;

namespace Shoell.Autobody.Data.Tests
{
    public class UserRepositoryTests(
        AutobodyContext context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        UserRepository repository
    ) : BaseRepositoryTests<User>
    {
        protected override AutobodyContext Context => context;
        protected override IHttpContextAccessor HttpContextAccessor => httpContextAccessor;
        protected override UserRepository Repository => repository;

        protected override string ReadPolicy => UserRoles.Read;
        protected override string CreatePolicy => UserRoles.Create;
        protected override string UpdatePolicy => UserRoles.Update;
        protected override string DeletePolicy => UserRoles.Delete;
        protected override string RecyclePolicy => UserRoles.Recycle;
        protected override string RecoverPolicy => UserRoles.Recover;
        protected override string PurgePolicy => UserRoles.Purge;
        protected override string ArchivePolicy => UserRoles.Archive;
        protected override string RestorePolicy => UserRoles.Restore;

        protected override IDateTimeProvider DateTimeProvider => dateTimeProvider;

        public override async Task<User> GetDefaultModelValues_AddAsync(Guid userId)
        {
            var model = await base.GetDefaultModelValues_AddAsync(userId);

            model.UserName = Guid.NewGuid().ToString();

            return model;
        }
    }
}
