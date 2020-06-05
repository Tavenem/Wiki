using Microsoft.AspNetCore.Identity;
using System;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A user of the wiki.
    /// </summary>
    /// <remarks>
    /// You are not required to use this class (or ASP.NET Identity at all) for your primary
    /// identity store, if you have other business requirements. In such cases you may maintain a
    /// separate user store for the wiki, and coordinate between the two sets of users via their
    /// IDs.
    /// </remarks>
    public class WikiUser : IdentityUser
    {
        /// <summary>
        /// <para>
        /// The date and time when this user's account was last disabled.
        /// </para>
        /// <para>
        /// <seealso cref="IsDisabled"/>
        /// </para>
        /// </summary>
        public DateTimeOffset? DisabledStart { get; set; }

        /// <summary>
        /// Whether this user may upload files.
        /// </summary>
        public bool HasUploadPermission { get; set; }

        /// <summary>
        /// Whether this user's account has been (soft) deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Whether this user's account has been disabled.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// The date and time when this account was last used to sign in.
        /// </summary>
        public DateTimeOffset LastAccess { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiUser"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public WikiUser() { }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiUser"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public WikiUser(string userName) : base(userName) { }
    }
}
