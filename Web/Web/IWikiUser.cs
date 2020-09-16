using System.Collections.Generic;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A user of the wiki.
    /// </summary>
    public interface IWikiUser
    {
        /// <summary>
        /// The unique ID of this user.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A list of the group IDs to which this user belongs (if any).
        /// </summary>
        List<string>? Groups { get; set; }

        /// <summary>
        /// Whether this user's account has been (soft) deleted.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Whether this user's account has been disabled.
        /// </summary>
        bool IsDisabled { get; set; }

        /// <summary>
        /// Whether this user is a wiki administrator.
        /// </summary>
        bool IsWikiAdmin { get; set; }

        /// <summary>
        /// <para>
        /// The total number of kilobytes of uploaded files permitted for this user.
        /// </para>
        /// <para>
        /// A negative value indicates that the user may upload files without limit.
        /// </para>
        /// <para>
        /// This value may be overridden by the value assigned to any <see cref="IWikiGroup"/> to
        /// which this user belongs. Any negative value indicates that the user may upload without
        /// limit. Otherwise, the maximum value among the groups and the user's individual limit is
        /// used.
        /// </para>
        /// </summary>
        int UploadLimit { get; set; }

        /// <summary>
        /// The user name for this user.
        /// </summary>
        string UserName { get; set; }
    }
}
