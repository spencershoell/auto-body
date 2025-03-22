using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Identity.Web.Resource;
using Microsoft.OData;
using Shoell.Autobody.Data;
using Shoell.Shared.Exceptions;

namespace Shoell.Autobody.Services
{
    public abstract class CoreController<TEntity> : ODataController
        where TEntity : class
    {
        protected abstract CoreRepository<TEntity> Repository { get; }

        protected abstract string[] ReadScopes { get; }

        // GET: api/TEntity      
        [HttpGet]
        [EnableQuery]
        public virtual IActionResult Get()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized();

            try
            {
                HttpContext.VerifyUserHasAnyAcceptedScope(ReadScopes);

                return Ok(Repository.Get());
            }
            catch (Exception ex)
            {
                return ReturnODataErrorResult(ex);
            }
        }

        protected IActionResult ReturnODataErrorResult(Exception ex)
        {
            var odataError = BuildODataError(ex);

            switch (odataError.Code)
            {
                case "400":
                    return BadRequest(odataError);
                case "401":
                    return Unauthorized(odataError);
                case "403":
                    return ODataErrorResult(odataError);
                case "404":
                    return NotFound(odataError);
                default:
                    break;
            }

            return ODataErrorResult(odataError);
        }

        protected ODataError BuildODataError(Exception ex)
        {
            var result = new ODataError
            {
                Code = "400",
                Target = "Bad Request",
                Message = ex.Message,
            };

            if (ex is BadRequestException)
            {
                result.Code = "400";
                result.Target = "Bad Request";
            }

            if (ex is InvalidUserClaimException)
            {
                result.Code = "401";
                result.Target = "Invalid User Claim";
            }

            if (ex is NotFoundException)
            {
                result.Code = "404";
                result.Target = "Not Found";
            }

            if (ex is UserNotFoundException)
            {
                result.Code = "401";
                result.Target = "User Not Found";
            }

            if (ex is UnauthenticatedException)
            {
                result.Code = "401";
                result.Target = "Unauthenticated";
            }

            if (ex is UnauthorizedException)
            {
                result.Code = "403";
                result.Target = "Unauthorized";
            }

            return result;
        }
    }
}
