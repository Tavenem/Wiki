using Tavenem.DataStorage;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a particular page of revision information for a wiki item.
/// </summary>
/// <param name="Editors">
/// <para>
/// Information about the editors who made the revisions on the page.
/// </para>
/// <para>
/// This is a de-duplicated list which can be referenced by the <see cref="Revision.Editor"/> fields
/// of each revision in the page data.
/// </para>
/// <para>
/// Only users who currently exist (<see cref="IWikiUser.IsDeleted"/> is not true) will be included.
/// </para>
/// <para>
/// The list will be <see langword="null"/> if the requesting user does not have permission to view
/// the wiki item.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for the wiki item.
/// </param>
/// <param name="Revisions">
/// <para>
/// The revisions included on the requested page.
/// </para>
/// <para>
/// The page will be <see langword="null"/> if the requesting user does not have permission to view
/// the wiki item.
/// </para>
/// </param>
public record PagedRevisionInfo(
    List<WikiUserInfo>? Editors,
    WikiPermission Permission,
    PagedListDTO<Revision>? Revisions);
