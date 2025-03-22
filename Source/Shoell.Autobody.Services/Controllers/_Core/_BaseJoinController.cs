using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Identity.Web.Resource;
using Shoell.Autobody.Data;

namespace Shoell.Autobody.Services
{
    public abstract class IntersectController<TEntity> : CoreController<TEntity>
        where TEntity : class
    {
        protected abstract Expression<Func<TEntity, bool>> IdentifierPredicate(Guid key1, Guid key2);

        protected override abstract BaseJoinRepository<TEntity> Repository { get; }

        protected abstract string[] ModifyScopes { get; }

        [HttpGet]
        [EnableQuery]
        public virtual IActionResult Get([FromODataUri] Guid key1, [FromODataUri] Guid key2, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ReadScopes);

            var entities = Repository
                .Get()
                .Where(IdentifierPredicate(key1, key2));

            try
            {
                return Ok(SingleResult.Create(entities));
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        [HttpDelete]

        public virtual async Task<IActionResult> Delete([FromODataUri] Guid key1, [FromODataUri] Guid key2, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!(await Repository.ExistsAsync(key1, key2, cancellationToken)))
                return NotFound();

            try
            {
                await Repository.RemoveAsync(key1, key2, cancellationToken);
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
