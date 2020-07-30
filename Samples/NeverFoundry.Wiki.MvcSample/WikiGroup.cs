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
        /// The display name for this group.
        /// </summary>
        public string GroupName { get; set; }

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
        /// The type discriminator for this type.
        /// </summary>
        public const string WikiGroupIdItemTypeName = ":WikiGroup:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public string IdItemTypeName => WikiGroupIdItemTypeName;

        /// <summary>
        /// <para>
        /// The owner of this group.
        /// </para>
        /// <para>
        /// May be a user or another group.
        /// </para>
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="UserGroup"/>.
        /// </summary>
        /// <param name="groupName">
        /// The display name for this group.
        /// </param>
        /// <param name="owner">
        /// <para>
        /// The owner of this group.
        /// </para>
        /// <para>
        /// May be a user or another group.
        /// </para>
        /// </param>
        /// <param name="hasUploadPermission">
        /// <para>
        /// Whether members of this group may upload files.
        /// </para>
        /// <para>
        /// If <see langword="false"/>, individual members with this property set to <see
        /// langword="true"/> may still upload. If <see langword="true"/>, members may upload
        /// regardless of their individual setting. This allows upload permission to be granted
        /// either individually, or to entire groups.
        /// </para>
        /// </param>
        public WikiGroup(
            string groupName,
            string owner,
            bool hasUploadPermission)
        {
            GroupName = groupName;
            Owner = owner;
            HasUploadPermission = hasUploadPermission;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UserGroup"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="groupName">
        /// The display name for this group.
        /// </param>
        /// <param name="owner">
        /// <para>
        /// The owner of this group.
        /// </para>
        /// <para>
        /// May be a user or another group.
        /// </para>
        /// </param>
        /// <param name="hasUploadPermission">
        /// <para>
        /// Whether members of this group may upload files.
        /// </para>
        /// <para>
        /// If <see langword="false"/>, individual members with this property set to <see
        /// langword="true"/> may still upload. If <see langword="true"/>, members may upload
        /// regardless of their individual setting. This allows upload permission to be granted
        /// either individually, or to entire groups.
        /// </para>
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers.
        /// </remarks>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public WikiGroup(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: Used by deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string groupName,
            string owner,
            bool hasUploadPermission) : base(id)
        {
            GroupName = groupName;
            Owner = owner;
            HasUploadPermission = hasUploadPermission;
        }
    }
}
