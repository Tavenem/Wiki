namespace Tavenem.Wiki;

/// <summary>
/// A user of the wiki.
/// </summary>
public interface IWikiUser : IWikiOwner
{
    /// <summary>
    /// A list of the group IDs to which this user belongs (if any).
    /// </summary>
    IList<string>? Groups { get; set; }

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
}
