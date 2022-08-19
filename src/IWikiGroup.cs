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
public interface IWikiGroup
{
    /// <summary>
    /// The display name for this group.
    /// </summary>
    string GroupName { get; set; }

    /// <summary>
    /// The unique ID of this group.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>
    /// The total number of kilobytes of uploaded files permitted for users who belong to this
    /// group.
    /// </para>
    /// <para>
    /// A negative value indicates that group members may upload files without limit.
    /// </para>
    /// <para>
    /// This value may be overridden by the value assigned to individual group members, or by
    /// the value of other groups to which a user belongs. Any negative value indicates that the
    /// user may upload without limit. Otherwise, the maximum value among the groups and the
    /// user's individual limit is used.
    /// </para>
    /// </summary>
    int UploadLimit { get; set; }
}
