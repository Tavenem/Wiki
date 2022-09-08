namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a user page.
/// </summary>
/// <param name="Groups">The groups to which this user belongs.</param>
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
/// <param name="User">
/// <para>
/// Info for the user.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the user exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
public record UserPageInfo(
    List<WikiUserInfo>? Groups,
    Article? Item,
    WikiPermission Permission,
    WikiUserInfo? User);
