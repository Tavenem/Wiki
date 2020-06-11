using NeverFoundry.DataStorage;

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
    public class WikiGroup : IdItem, IWikiGroup
    {
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
        public bool HasUploadPermission { get; set; }

        /// <summary>
        /// The display name for this group.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiGroup"/>.
        /// </summary>
        /// <param name="groupName">
        /// The display name for this group.
        /// </param>
        public WikiGroup(string groupName) => GroupName = groupName;

        /// <summary>
        /// Initializes a new instance of <see cref="WikiGroup"/>.
        /// </summary>
        /// <param name="groupName">
        /// The display name for this group.
        /// </param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public WikiGroup(string id, string groupName, bool hasUploadPermission) : base(id)
        {
            GroupName = groupName;
            HasUploadPermission = hasUploadPermission;
        }
    }
}
