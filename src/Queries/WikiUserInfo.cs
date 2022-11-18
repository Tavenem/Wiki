namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a wiki user or group.
/// </summary>
/// <param name="Id">
/// The ID of the <see cref="IWikiUser"/> or <see cref="IWikiGroup"/> requested.
/// </param>
/// <param name="Entity">
/// <para>
/// The user or group. May be <see langword="null"/> if no such user/group currently exists.
/// </para>
/// <para>
/// The properties of this object may be set to <see langword="null"/> or a default value, depending
/// on the permissions of the requesting user. The user or group itself, and wiki administrators,
/// will have access to all its information. Other users will typically have access only to the <see
/// cref="IWikiOwner.Id"/>, <see cref="IWikiOwner.DisplayName"/>, and <see
/// cref="IWikiUser.IsWikiAdmin"/> (for users). If a user has been (soft) deleted, only <see
/// cref="IWikiOwner.Id"/> will be set for other users.
/// </para>
/// </param>
public record WikiUserInfo(string Id, IWikiOwner? Entity);
