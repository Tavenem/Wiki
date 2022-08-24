namespace Tavenem.Wiki;

/// <summary>
/// <para>
/// Represents a group of users.
/// </para>
/// <para>
/// Groups may be assigned ownership, viewing, or editing permissions on wiki items, just like
/// individual users may.
/// </para>
/// </summary>
public interface IWikiGroup : IWikiOwner
{
    /// <summary>
    /// The <see cref="IWikiOwner.Id"/> of the <see cref="IWikiUser"/> who acts as the
    /// owner/administrator of this group.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the owner of a group may create, delete, or move its group page, set view permission on
    /// its group page, invite or remove users, and change the group's ownership.
    /// </para>
    /// <para>
    /// Although this property is nullable to avoid causing trouble for initialization and/or
    /// (de)serialization processes, a group is considered invalid without a current owner. Such
    /// groups will be treated as not existing when encountered in most situations.
    /// </para>
    /// </remarks>
    string? OwnerId { get; set; }
}
