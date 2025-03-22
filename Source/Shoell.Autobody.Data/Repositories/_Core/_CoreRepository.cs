using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Exceptions;
using Shoell.Shared.Extensions;
using Shoell.Shared.Interfaces;
using Shoell.Shared.Interfaces.System;
using Shoell.Shared.Types;

namespace Shoell.Autobody.Data
{
    public abstract class CoreRepository<TEntity> : ICoreRepository<TEntity>
        where TEntity : class
    {
        protected abstract IAuthorizationService AuthorizationService { get; }
        protected abstract ILogContext<Log> Context { get; }
        protected abstract IDateTimeProvider DateTimeProvider { get; }
        protected abstract DbSet<TEntity> EntitySet { get; }
        protected abstract EntityType EntityType { get; }
        protected abstract IHttpContextAccessor HttpContextAccessor { get; }
        protected ClaimsPrincipal User => HttpContextAccessor?.HttpContext?.User ?? new();

        protected abstract string ReadPolicy { get; }

        public virtual async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> Get()
        {
            ResolveAuthorization(ReadPolicy);
            return EntitySet.AsNoTracking();
        }

        protected virtual void ResolveAuthorization(string policy)
        {
            ResolveAuthorizationAsync(policy).GetAwaiter().GetResult();
        }

        protected virtual async Task ResolveAuthorizationAsync(string policy, CancellationToken cancellationToken = default)
        {
            if (!(User.Identity?.IsAuthenticated).GetValueOrDefault())
                throw new UnauthenticatedException($"User is not authenticated, but needs to be in order to {policy.Split('.').Last().AsSpacedPascaleCase()} {EntityType.Name.AsSpacedPascaleCase().AsPlural()} in the datastore");


            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, policy);
            if (authorizationResult.Succeeded)
                return;

            throw new UnauthorizedException($"User is Authenticated, but not Authorized to {policy.Split('.').Last().AsSpacedPascaleCase()} {EntityType.Name.AsSpacedPascaleCase().AsPlural()} in the datastore");
        }

        protected virtual void ResolveAuthorization<T>(T resource, string policy)
        {
            ResolveAuthorizationAsync(resource, policy).GetAwaiter().GetResult();
        }

        protected virtual async Task ResolveAuthorizationAsync<T>(T resource, string policy, CancellationToken cancellationToken = default)
        {
            if (!(User.Identity?.IsAuthenticated).GetValueOrDefault())
                throw new UnauthenticatedException($"User is not authenticated, but needs to be in order to {policy.Split('.').Last().AsSpacedPascaleCase()} {EntityType.Name.AsSpacedPascaleCase().AsPlural()} in the datastore");


            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, resource, policy);
            if (authorizationResult.Succeeded)
                return;

            throw new UnauthorizedException($"User is Authenticated, but not Authorized to {policy.Split('.').Last().AsSpacedPascaleCase()} {EntityType.Name.AsSpacedPascaleCase().AsPlural()} in the datastore");
        }
    }
}
