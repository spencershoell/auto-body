using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Identity.Web.Resource;
using Shoell.Autobody.Data;
using Shoell.Autobody.Models.System;
using Shoell.Shared.Interfaces;

namespace Shoell.Autobody.Services
{
    public abstract class BaseModelController<TEntity> : CoreController<TEntity>
        where TEntity : class, IBaseModel
    {
        protected override abstract BaseRepository<TEntity> Repository { get; }

        protected abstract string[] ModifyScopes { get; }
        protected abstract string[] RecycleScopes { get; }
        protected abstract string[] ArchiveScopes { get; }

        // GET: api/TEntity(5)
        [HttpGet]
        [EnableQuery]
        public virtual IActionResult Get([FromODataUri] Guid key)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ReadScopes);

            try
            {
                var entities = Repository.Get()
                    .Where(e => e.Id.Equals(key));

                if (!entities.Any())
                    return NotFound();

                return Ok(SingleResult.Create(entities));
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // PUT: api/TEntity(5)
        [HttpPut]
        public virtual async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] TEntity model, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                model.Id = key;
                var entity = await Repository.UpdateAsync(model, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // PATCH: /api/TEntity(5)
        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<TEntity> model, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.PatchAsync(key, model, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: api/TEntity
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TEntity model, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entity = await Repository.AddAsync(model, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return Created(entity);
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // DELETE: api/TEntity(5)
        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromODataUri] Guid key, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ModifyScopes);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.RemoveAsync(key, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: api/TEntity(5)/Archive
        [HttpPost]
        public virtual async Task<IActionResult> Archive([FromODataUri] Guid key, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ArchiveScopes);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.ArchiveAsync(key, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: api/TEntity(5)/Restore
        [HttpPost]
        public virtual async Task<IActionResult> Restore([FromODataUri] Guid key, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ArchiveScopes);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.RestoreAsync(key, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: api/TEntity(5)/Recover
        [HttpPost]
        public virtual async Task<IActionResult> Recover([FromODataUri] Guid key, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(RecycleScopes);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.RecoverAsync(key, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // POST: api/TEntity(5)/Purge
        [HttpPost]
        public virtual async Task<IActionResult> Purge([FromODataUri] Guid key, CancellationToken cancellationToken = default)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(RecycleScopes);

            try
            {
                if (!await Repository.ExistsAsync(key, cancellationToken))
                    return NotFound();

                await Repository.PurgeAsync(key, cancellationToken);
                await Repository.CommitAsync(cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        // GET: api/TEntity/Recycle
        [HttpGet]
        [EnableQuery]
        public virtual IActionResult Recycle()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(RecycleScopes);

            try
            {
                return Ok(Repository.GetRecycle());
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        [HttpGet]
        [EnableQuery]
        public virtual IActionResult Logs([FromODataUri] Guid key)
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            HttpContext.VerifyUserHasAnyAcceptedScope(ReadScopes);

            try
            {
                return Ok(Repository.GetLogs(key));
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }
    }
}
