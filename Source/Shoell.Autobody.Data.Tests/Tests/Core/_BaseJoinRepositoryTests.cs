using Shoell.Shared.Exceptions;

namespace Shoell.Autobody.Data.Tests
{
    public abstract class BaseJoinRepositoryTests<TEntity> : CoreRepositoryTests<TEntity>
        where TEntity : class, new()
    {
        protected abstract override BaseJoinRepository<TEntity> Repository { get; }

        protected abstract string DeletePolicy { get; }

        #region ExistsAsync_model
        [Fact]
        public virtual async Task ExistsAsync_model_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.ExistsAsync(new TEntity()));
        }

        [Fact]
        public virtual async Task ExistsAsync_model_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.ExistsAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public abstract Task ExistsAsync_model_WhenModelExists_ShouldReturnTrue();

        public abstract Task ExistsAsync_model_WhenModelExistsLocally_ShouldReturnTrue();

        [Fact]
        public virtual async Task ExistsAsync_model_WhenModelDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([ReadPolicy]);

            // Act
            var result = await Repository.ExistsAsync(new TEntity());

            // Assert
            Assert.False(result);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region ExistsAsync_id
        [Fact]
        public virtual async Task ExistsAsync_id_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public virtual async Task ExistsAsync_id_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public abstract Task ExistsAsync_id_WhenModelExists_ShouldReturnTrue();

        public abstract Task ExistsAsync_id_WhenModelExistsLocally_ShouldReturnTrue();

        [Fact]
        public virtual async Task ExistsAsync_id_WhenModelDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([ReadPolicy]);

            // Act
            var result = await Repository.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.False(result);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region GetAsync
        [Fact]
        public virtual async Task GetAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([ReadPolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.GetAsync(Guid.NewGuid(), Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.GetAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public virtual async Task GetAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.GetAsync(Guid.NewGuid(), Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public abstract Task GetAsync_ShouldResolveAuthorization();
        #endregion

        #region RemoveAsync
        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([DeletePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.RemoveAsync(Guid.NewGuid(), Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.RemoveAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.RemoveAsync(Guid.NewGuid(), Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public abstract Task RemoveAsync_ShouldPurgeResource_WhenUserIsAuthorizedAndResourceExists();
        #endregion
    }
}
