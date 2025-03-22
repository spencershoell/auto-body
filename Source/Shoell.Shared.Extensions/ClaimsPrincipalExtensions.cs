using System.Security.Claims;
using Shoell.Shared.Exceptions;

namespace Shoell.Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static readonly string GivenNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public static readonly string SurnameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public static readonly string OpenConnectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public static readonly string azureAdB2CIdClaimType = ClaimTypes.NameIdentifier;
        public static readonly string OnPremSidClaimType = "onprem_sid";
        public static readonly string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
        public static readonly string UpnClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        public static readonly string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        // Autobody Claims
        public static readonly string AutobodyIdClaimType = "Autobody_Id";
        public static readonly string AutobodyExternalIdClaimType = "Autobody_ExternalId";
        public static readonly string AutobodyNameClaimType = "Autobody_Name";
        public static readonly string AutobodyUserNameClaimType = "Autobody_UserName";

        public static string GivenName(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var givenNameClaim = principal.Claims.FirstOrDefault(e => e.Type == GivenNameClaimType);
                return givenNameClaim?.Value ?? string.Empty;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {GivenNameClaimType}");
        }

        public static string Surname(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var surnameClaim = principal.Claims.FirstOrDefault(e => e.Type == SurnameClaimType);
                return surnameClaim?.Value ?? string.Empty;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {SurnameClaimType}");
        }

        public static Guid OpenConnectId(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                Guid openConnectId = Guid.Empty;
                var openConnectIdClaim = principal.Claims.FirstOrDefault(e => e.Type == OpenConnectIdClaimType);

                if (openConnectIdClaim != null)
                    if (!Guid.TryParse(openConnectIdClaim.Value, out openConnectId))
                        openConnectId = Guid.Empty;

                return openConnectId;

            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {OpenConnectIdClaimType}");
        }

        public static Guid AzureAdB2CId(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                Guid AzureAdB2CId = Guid.Empty;
                var AzureAdB2CIdClaim = principal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);

                if (AzureAdB2CIdClaim != null)
                    if (!Guid.TryParse(AzureAdB2CIdClaim.Value, out AzureAdB2CId))
                        AzureAdB2CId = Guid.Empty;

                return AzureAdB2CId;

            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {ClaimTypes.NameIdentifier}");
        }

        public static string OnPremSid(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var onPremSidClaim = principal.Claims.FirstOrDefault(e => e.Type == OnPremSidClaimType);
                return onPremSidClaim?.Value ?? string.Empty;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {OnPremSidClaimType}");
        }

        public static Guid TenantId(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                Guid tenantId = Guid.Empty;

                var tenantIdClaim = principal.Claims.FirstOrDefault(e => e.Type == TenantIdClaimType);
                if (tenantIdClaim != null)
                    if (!Guid.TryParse(tenantIdClaim.Value, out tenantId))
                        tenantId = Guid.Empty;

                return tenantId;

            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {TenantIdClaimType}");

        }

        public static string Upn(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var upnClaim = principal.Claims.FirstOrDefault(e => e.Type == UpnClaimType);
                if (upnClaim != null)
                    return upnClaim.Value;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {UpnClaimType}");
        }

        public static string Name(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var nameClaim = principal.Claims.FirstOrDefault(e => e.Type == NameClaimType);
                return nameClaim?.Value ?? string.Empty;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {NameClaimType}");
        }

        #region Autobody Claim Extensions
        public static Guid AutobodyId(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault(e => e.Type == AutobodyIdClaimType);
                if (Guid.TryParse(claim?.Value, out var result))
                    return result;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {AutobodyIdClaimType}");
        }

        public static string AutobodyExternalId(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault(e => e.Type == AutobodyExternalIdClaimType);
                if (claim != null)
                    return claim.Value;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {AutobodyExternalIdClaimType}");

        }

        public static string AutobodyName(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault(e => e.Type == AutobodyNameClaimType);
                if (claim != null)
                    return claim.Value;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {AutobodyNameClaimType}");
        }

        public static string AutobodyUserName(this ClaimsPrincipal principal)
        {
            if (principal != null && principal.Claims != null)
            {
                var claim = principal.Claims.FirstOrDefault(e => e.Type == AutobodyUserNameClaimType);
                if (claim != null)
                    return claim.Value;
            }
            throw new InvalidUserClaimException($"Current User does not have required Claim: {AutobodyUserNameClaimType}");
        }
        #endregion
    }
}
