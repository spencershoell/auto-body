using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Shoell.Shared.Exceptions;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data.Tests
{
    public abstract class BaseRepositoryTests<TEntity> : CoreRepositoryTests<TEntity>
        where TEntity : class, IBaseModel, new()
    {
        protected abstract override BaseRepository<TEntity> Repository { get; }
        protected abstract IDateTimeProvider DateTimeProvider { get; }

        protected abstract string CreatePolicy { get; }
        protected abstract string UpdatePolicy { get; }
        protected abstract string DeletePolicy { get; }

        protected abstract string RecyclePolicy { get; }
        protected abstract string RecoverPolicy { get; }
        protected abstract string PurgePolicy { get; }

        protected abstract string ArchivePolicy { get; }
        protected abstract string RestorePolicy { get; }

        #region GetRecycle
        [Fact]
        public virtual async Task GetRecycle_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.GetRecycle().ToListAsync());

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetRecycle_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.GetRecycle().ToListAsync());

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetRecycle_ShouldResolveAuthorization()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([RecyclePolicy]);

            // Act
            var result = await Repository.GetRecycle()
                .ToListAsync();

            // Assert
            Assert.NotNull(result);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region AddAsync
        [Fact]
        public virtual async Task AddAsync_WhenModelExists_ShouldThrowBadRequestException()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();
            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () => await Repository.AddAsync(model));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task AddAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.AddAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task AddAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.AddAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task AddAsync_WhenModelDoesNotExist_ShouldAddModelToEntitySet()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            var model = await GetDefaultModelValues_AddAsync();

            await SetupUserWithRoles([CreatePolicy]);

            // Act
            await Repository.AddAsync(model);
            await Repository.CommitAsync();
            var result = await Repository.Get().FirstOrDefaultAsync(e => e.Id == model.Id);

            Assert.NotNull(result);

            Assert.Equal(User?.AutobodyId(), result.CreatedById);
            Assert.Equal(User?.AutobodyId(), result.ModifiedById);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateCreated, new TimeSpan(0, 1, 0));
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Null(result.DateArchived);
            Assert.Null(result.DateDeleted);
            Assert.Null(result.ArchivedById);
            Assert.Null(result.DeletedById);
            Assert.False(result.IsArchived);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task AddAsync_AfterModelCreation_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            var model = await GetDefaultModelValues_AddAsync();

            await SetupUserWithRoles([CreatePolicy]);

            // Act
            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            // Assert
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Create)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }

        public async virtual Task<TEntity> GetDefaultModelValues_AddAsync()
        {
            return await GetDefaultModelValues_AddAsync(Guid.Empty);
        }

        public async virtual Task<TEntity> GetDefaultModelValues_AddAsync(Guid userId)
        {
            return await Task.FromResult(new TEntity
            {
                Id = Guid.NewGuid(),
                CreatedById = userId,
                ModifiedById = userId,
                ArchivedById = null,
                DeletedById = null,
                DateCreated = DateTime.MinValue,
                DateModified = DateTime.MinValue,
                DateArchived = null,
                DateDeleted = null
            });
        }
        #endregion

        #region ArchiveAsync
        [Fact]
        public virtual async Task ArchiveAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([ArchivePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.ArchiveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ArchiveAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.ArchiveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ArchiveAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.ArchiveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ArchiveAsync_ShouldArchiveResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await SetupUserWithRoles([ArchivePolicy]);

            // Act
            await Repository.ArchiveAsync(initial.Id);
            await Repository.CommitAsync();

            var result = await Repository.GetAsync(initial.Id);

            // Assert
            Assert.NotNull(result.DateArchived);
            Assert.NotNull(result.ArchivedById);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Equal(DateTimeProvider.UtcNow, result.DateArchived.GetValueOrDefault(), new TimeSpan(0, 1, 0));
            Assert.Equal(User?.AutobodyId(), result.ModifiedById);
            Assert.Equal(User?.AutobodyId(), result.ArchivedById);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ArchiveAsync_AfterModelArchive_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([ArchivePolicy]);

            // Act
            await Repository.ArchiveAsync(model.Id);
            await Repository.CommitAsync();

            // Assert
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Archive)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region ExistsAsync_model
        [Fact]
        public virtual async Task ExistsAsync_model_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.ExistsAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
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
        public virtual async Task ExistsAsync_model_WhenModelExists_ShouldReturnTrue()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            // Act
            var result = await Repository.ExistsAsync(model);

            // Assert
            Assert.True(result);

            // Cleanup
            await transaction.RollbackAsync();
        }

        public virtual async Task ExistsAsync_model_WhenModelExistsLocally_ShouldReturnTrue()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);

            // Act
            var result = await Repository.ExistsAsync(model);

            // Assert
            Assert.True(result);

            // Cleanup
            await transaction.RollbackAsync();
        }

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
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.ExistsAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ExistsAsync_id_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.ExistsAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ExistsAsync_id_WhenModelExists_ShouldReturnTrue()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            // Act
            var result = await Repository.ExistsAsync(model.Id);

            // Assert
            Assert.True(result);

            // Cleanup
            await transaction.RollbackAsync();
        }

        public virtual async Task ExistsAsync_id_WhenModelExistsLocally_ShouldReturnTrue()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);

            // Act
            var result = await Repository.ExistsAsync(model.Id);

            // Assert
            Assert.True(result);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task ExistsAsync_id_WhenModelDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([ReadPolicy]);

            // Act
            var result = await Repository.ExistsAsync(Guid.NewGuid());

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
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.GetAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.GetAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.GetAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetAsync_ShouldResolveAuthorization()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await SetupUserWithRoles([ReadPolicy]);

            // Act
            var result = await Repository.GetAsync(initial.Id);

            // Assert
            Assert.NotNull(result);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region GetRecycleAsync
        [Fact]
        public virtual async Task GetRecycleAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([RecyclePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.GetRecycleAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetRecycleAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.GetRecycleAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetRecycleAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.GetRecycleAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetRecycleAsync_ShouldResolveAuthorization()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, DeletePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await Repository.RemoveAsync(initial.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([RecyclePolicy]);

            // Act
            var result = await Repository.GetRecycleAsync(initial.Id);

            // Assert
            Assert.NotNull(result);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region PatchAsync
        [Fact]
        public virtual async Task PatchAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.PatchAsync(Guid.NewGuid(), new Delta<TEntity>()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PatchAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.PatchAsync(Guid.NewGuid(), new Delta<TEntity>()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PatchAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.PatchAsync(Guid.NewGuid(), new Delta<TEntity>()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PatchAsync_ShouldUpdateResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);
            var model = await GetDefaultModelValues_AddAsync();

            model = await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act
            var entity = await Repository.GetAsync(model.Id);
            var delta = new Delta<TEntity>();

            delta.TrySetPropertyValue(nameof(entity.Id), entity.Id);
            delta.TrySetPropertyValue(nameof(entity.RowId), entity.RowId);
            delta.TrySetPropertyValue(nameof(entity.IsArchived), entity.IsArchived);
            delta.TrySetPropertyValue(nameof(entity.DateCreated), entity.DateCreated);
            delta.TrySetPropertyValue(nameof(entity.DateModified), entity.DateModified);
            delta.TrySetPropertyValue(nameof(entity.DateArchived), entity.DateArchived);
            delta.TrySetPropertyValue(nameof(entity.DateDeleted), entity.DateDeleted);
            delta.TrySetPropertyValue(nameof(entity.CreatedById), entity.CreatedById);
            delta.TrySetPropertyValue(nameof(entity.ModifiedById), entity.ModifiedById);
            delta.TrySetPropertyValue(nameof(entity.ArchivedById), entity.ArchivedById);
            delta.TrySetPropertyValue(nameof(entity.DeletedById), entity.DeletedById);


            var result = await Repository.PatchAsync(model.Id, delta);
            await Repository.CommitAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.RowId, result.RowId);
            Assert.False(result.IsArchived);

            Assert.Equal(model.CreatedById, result.CreatedById);
            Assert.Equal(HttpContextAccessor.HttpContext?.User?.AutobodyId(), result.ModifiedById);
            Assert.Null(result.ArchivedById);
            Assert.Null(result.DeletedById);

            Assert.Equal(model.DateCreated, result.DateCreated);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Null(result.DateArchived);
            Assert.Null(result.DateDeleted);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PatchAsync_AfterModelPatch_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act
            var delta = new Delta<TEntity>();
            await Repository.PatchAsync(model.Id, delta);
            await Repository.CommitAsync();

            // Assert
            var logEntry = await Context.Logs.FirstOrDefaultAsync(l => l.EntityId == model.Id && l.Action == LogAction.Update);
            Assert.NotNull(logEntry);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region PurgeAsync
        [Fact]
        public virtual async Task PurgeAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            await SetupUserWithRoles([PurgePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.PurgeAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PurgeAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();
            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.PurgeAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PurgeAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.PurgeAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PurgeAsync_ShouldPurgeResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, DeletePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await Repository.RemoveAsync(initial.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([PurgePolicy]);

            // Act
            await Repository.PurgeAsync(initial.Id);
            await Repository.CommitAsync();

            // Assert
            var user = User;
            await SetupUserWithRoles([ReadPolicy, RecyclePolicy]);

            var checkFilter = await Repository.GetRecycle()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            var result = await Repository.Get()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            Assert.Null(checkFilter);
            Assert.Null(result);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task PurgeAsync_AfterModelPurge_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, DeletePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await Repository.RemoveAsync(model.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([PurgePolicy]);

            // Act
            await Repository.PurgeAsync(model.Id);
            await Repository.CommitAsync();

            // Assert
            await SetupUserWithRoles([ReadPolicy]);
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Purge)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region RestoreAsync
        [Fact]
        public virtual async Task RestoreAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([ArchivePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.RestoreAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RestoreAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.RestoreAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RestoreAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.RestoreAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RestoreAsync_ShouldRestoreResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, ArchivePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await Repository.ArchiveAsync(initial.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([RestorePolicy]);

            // Act
            await Repository.RestoreAsync(initial.Id);
            await Repository.CommitAsync();

            var result = await Repository.GetAsync(initial.Id);

            // Assert
            Assert.Null(result.DateArchived);
            Assert.Null(result.ArchivedById);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Equal(User?.AutobodyId(), result.ModifiedById);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RestoreAsync_AfterModelRestore_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, ArchivePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await Repository.ArchiveAsync(model.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([RestorePolicy]);

            // Act
            await Repository.RestoreAsync(model.Id);
            await Repository.CommitAsync();

            // Assert
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Restore)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region RemoveAsync
        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([DeletePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.RemoveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.RemoveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RemoveAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.RemoveAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RemoveAsync_ShouldDeleteResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await SetupUserWithRoles([DeletePolicy]);

            // Act
            await Repository.RemoveAsync(initial.Id);
            await Repository.CommitAsync();

            // Assert
            var user = User;
            await SetupUserWithRoles([ReadPolicy, RecyclePolicy]);

            var checkFilter = await Repository.Get()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            var result = await Repository.GetRecycle()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            Assert.Null(checkFilter);
            Assert.NotNull(result);
            Assert.NotNull(result.DateDeleted);
            Assert.NotNull(result.DeletedById);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Equal(DateTimeProvider.UtcNow, result.DateDeleted.GetValueOrDefault(), new TimeSpan(0, 1, 0));
            Assert.Equal(user?.AutobodyId(), result.ModifiedById);
            Assert.Equal(user?.AutobodyId(), result.DeletedById);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RemoveAsync_AfterModelDelete_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([DeletePolicy]);

            // Act
            await Repository.RemoveAsync(model.Id);
            await Repository.CommitAsync();

            // Assert
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Delete)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region RecoverAsync
        [Fact]
        public virtual async Task RecoverAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([RecoverPolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.RecoverAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RecoverAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.RecoverAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RecoverAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.RecoverAsync(Guid.NewGuid()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RecoverAsync_ShouldRecoverResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, DeletePolicy]);

            var initial = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(initial);
            await Repository.CommitAsync();

            await Repository.RemoveAsync(initial.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([RecoverPolicy]);

            // Act
            await Repository.RecoverAsync(initial.Id);
            await Repository.CommitAsync();

            // Assert
            var user = User;
            await SetupUserWithRoles([ReadPolicy, RecyclePolicy]);

            var checkFilter = await Repository.GetRecycle()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            var result = await Repository.Get()
                .FirstOrDefaultAsync(e => e.Id == initial.Id);

            Assert.Null(checkFilter);
            Assert.NotNull(result);
            Assert.Null(result.DateDeleted);
            Assert.Null(result.DeletedById);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Equal(user?.AutobodyId(), result.ModifiedById);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task RecoverAsync_AfterModelRecovery_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy, DeletePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await Repository.RemoveAsync(model.Id);
            await Repository.CommitAsync();

            await SetupUserWithRoles([RecoverPolicy]);

            // Act
            await Repository.RecoverAsync(model.Id);
            await Repository.CommitAsync();

            // Assert
            await SetupUserWithRoles([ReadPolicy]);
            var logs = await Repository.GetLogs(model.Id)
                .Where(e => e.Action == LogAction.Recover)
                .ToListAsync();

            Assert.NotNull(logs);
            Assert.NotEmpty(logs);
            Assert.Single(logs);

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public virtual async Task UpdateAsync_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.UpdateAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task UpdateAsync_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.UpdateAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task UpdateAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await Repository.UpdateAsync(new TEntity()));

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task UpdateAsync_ShouldUpdateResource_WhenUserIsAuthorizedAndResourceExists()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);
            var model = await GetDefaultModelValues_AddAsync();

            model = await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act
            var entity = await Repository.GetAsync(model.Id);
            entity.Id = model.Id;
            entity.RowId = 0;
            entity.IsArchived = true;
            entity.CreatedById = Guid.Empty;
            entity.ModifiedById = Guid.Empty;
            entity.ArchivedById = Guid.Empty;
            entity.DeletedById = Guid.Empty;
            entity.DateCreated = DateTime.MinValue;
            entity.DateModified = DateTime.MinValue;
            entity.DateArchived = DateTime.MinValue;
            entity.DateDeleted = DateTime.MinValue;

            var result = await Repository.UpdateAsync(entity);
            await Repository.CommitAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.RowId, result.RowId);
            Assert.False(result.IsArchived);

            Assert.Equal(model.CreatedById, result.CreatedById);
            Assert.Equal(HttpContextAccessor.HttpContext?.User?.AutobodyId(), result.ModifiedById);
            Assert.Null(result.ArchivedById);
            Assert.Null(result.DeletedById);

            Assert.Equal(model.DateCreated, result.DateCreated);
            Assert.Equal(DateTimeProvider.UtcNow, result.DateModified, new TimeSpan(0, 1, 0));
            Assert.Null(result.DateArchived);
            Assert.Null(result.DateDeleted);

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task UpdateAsync_AfterModelUpdate_LogEntryShouldBePresent()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([CreatePolicy]);

            var model = await GetDefaultModelValues_AddAsync();

            await Repository.AddAsync(model);
            await Repository.CommitAsync();

            await SetupUserWithRoles([UpdatePolicy]);

            // Act
            var entity = await Repository.GetAsync(model.Id);
            await Repository.UpdateAsync(entity);
            await Repository.CommitAsync();

            // Assert
            var logEntry = await Context.Logs.FirstOrDefaultAsync(l => l.EntityId == entity.Id && l.Action == LogAction.Update);
            Assert.NotNull(logEntry);

            // Cleanup
            await transaction.RollbackAsync();
        }

        // Existing code


        #endregion

        #region GetLogs
        [Fact]
        public virtual async Task GetLogs_ShouldThrowAuthenticationError_WhenUserIsNotSetup()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            HttpContextAccessor.HttpContext?.ResetUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthenticatedException>(async () => await Repository.GetLogs(Guid.NewGuid()).ToListAsync());

            // Cleanup
            await transaction.RollbackAsync();
        }

        [Fact]
        public virtual async Task GetLogs_ShouldThrowAuthorizationError_WhenUserIsNotAuthorizedToRead()
        {
            // Arrange
            var transaction = await Context.Database.BeginTransactionAsync();

            await SetupUserWithRoles([]);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(async () => await Repository.GetLogs(Guid.NewGuid()).ToListAsync());

            // Cleanup
            await transaction.RollbackAsync();
        }
        #endregion
    }
}
