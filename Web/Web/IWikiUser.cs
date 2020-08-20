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
        /// <para>
        /// Whether this user may upload files.
        /// </para>
        /// <para>
        /// If <see langword="false"/>, this user may still upload if it belongs to a <see
        /// cref="IWikiGroup"/> with this property set to <see langword="true"/>. If <see
        /// langword="true"/>, this user may upload regardless of the setting of any group to which
        /// it belongs. This allows upload permission to be granted either individually, or to
        /// entire groups.
        /// </para>
        /// </summary>
        bool HasUploadPermission { get; set; }

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
        /// The user name for this user.
        /// </summary>
        string UserName { get; set; }
    }
}
