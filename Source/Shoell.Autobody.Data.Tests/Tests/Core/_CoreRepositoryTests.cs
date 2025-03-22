using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.Identity;
using Shoell.Shared.Exceptions;

namespace Shoell.Autobody.Data.Tests
{
    public abstract class CoreRepositoryTests<TEntity>
        where TEntity : class, new()
    {
        protected abstract CoreRepository<TEntity> Repository { get; }
        protected abstract IHttpContextAccessor HttpContextAccessor { get; }
        protected ClaimsPrincipal? User => HttpContextAccessor.HttpContext?.User;
        protected abstract AutobodyContext Context { get; }
        protected abstract string ReadPolicy { get; }

        #region Get
        [Fact]
        public virtual async Task Get_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.Get().ToListAsync());
        }

        [Fact]
        public virtual async Task Get_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.Get().ToListAsync());

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task Get_ShouldResolveAuthorization()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([ReadPolicy]);

            // Act
            var list = await Repository.Get()
                .ToListAsync();

            // Assert
            Assert.NotNull(list);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region Commit
        [Fact]
        public virtual async Task CommitAsync_ShouldCommitToDatabase()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                // Act
                var user = new User().PopulateDefaultValues();
                await Context.Users.AddAsync(user);
                await Repository.CommitAsync();

                var newUser = await Context.Users.FirstOrDefaultAsync(e => e.Id == user.Id);

                // Assert
                Assert.NotNull(newUser);
            }
            catch
            {
                throw;
            }

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        protected virtual async Task<User> SetupUserWithRoles(List<string?> roleNames)
        {
            var user = new User().PopulateDefaultValues();

            await Context.Users.AddAsync(user);
            var roles = await Context.Roles
                .Where(e => roleNames.Contains(e.Name))
                .ToListAsync();

            foreach (var role in roles)
                await Context.UserRoles.AddAsync(new User_Role { UserId = user.Id, RoleId = role.Id });

            await Context.SaveChangesAsync();

            await HttpContextAccessor.HttpContext!.SetUserWithRolesAsync(user.Id, Context);

            return user;
        }
    }
}
