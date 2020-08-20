namespace NeverFoundry.Wiki
{
    /// <summary>
    /// <para>
    /// Represents a group of users.
    /// </para>
    /// <para>
    /// Groups may be assigned ownership, viewing, or editing permissions on wiki items, just like
    /// individual users may.
    /// </para>
    /// </summary>
    public interface IWikiGroup
    {
        /// <summary>
        /// The display name for this group.
        /// </summary>
        string GroupName { get; set; }

        /// <summary>
        /// <para>
        /// Whether members of this group may upload files.
        /// </para>
        /// <para>
        /// If <see langword="false"/>, individual members with this property set to <see
        /// langword="true"/> may still upload. If <see langword="true"/>, members may upload
        /// regardless of their individual setting. This allows upload permission to be granted
        /// either individually, or to entire groups.
        /// </para>
        /// </summary>
        bool HasUploadPermission { get; set; }

        /// <summary>
        /// The unique ID of this group.
        /// </summary>
        string Id { get; }
    }
}
