namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a group page.
/// </summary>
/// <param name="Group">
/// <para>
/// Info for the group.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the group exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Item">
/// <para>
/// The <see cref="Article"/>.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the item exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for this item.
/// </param>
/// <param name="Users">The users in this group.</param>
public record GroupPageInfo(
    WikiUserInfo? Group,
    Article? Item,
    WikiPermission Permission,
    List<WikiUserInfo>? Users);
