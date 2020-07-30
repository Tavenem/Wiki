using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NeverFoundry.Wiki.MvcSample
{
    /// <summary>
    /// A collection of extension methods related to a web implementation of NeverFoundry.Wiki.
    /// </summary>
    public static class AuthorizationExtensionMethods
    {
        /// <summary>
        /// Determines whether the <see cref="ClaimsPrincipal"/> has the given boolean <see
        /// cref="Claim"/> type.
        /// </summary>
        /// <param name="principal">A <see cref="ClaimsPrincipal"/>.</param>
        /// <param name="claimType">A <see cref="Claim"/> type.</param>
        /// <returns>
        /// <see langword="true"/> if the given <see cref="ClaimsPrincipal"/> specifies a value of
        /// "true" for a <see cref="Claim"/> with the given type; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public static bool HasBoolClaim(this ClaimsPrincipal principal, string claimType)
            => principal.HasClaim(x => x.ValueType == ClaimValueTypes.Boolean
            && x.Type == claimType
            && bool.TryParse(x.Value, out var value)
            && value);

        /// <summary>
        /// Determines whether the collection of <see cref="Claim"/> objects has the given boolean
        /// <see cref="Claim"/> type.
        /// </summary>
        /// <param name="claims">A collection of <see cref="Claim"/> objects.</param>
        /// <param name="claimType">A <see cref="Claim"/> type.</param>
        /// <returns>
        /// <see langword="true"/> if the given collection of <see cref="Claim"/> objects includes a
        /// <see cref="Claim"/> which specifies a value of "true" and has the given type; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public static bool HasBoolClaim(this IEnumerable<Claim> claims, string claimType)
            => claims.Any(x => x.ValueType == ClaimValueTypes.Boolean
            && x.Type == claimType
            && bool.TryParse(x.Value, out var value)
            && value);
    }
}
