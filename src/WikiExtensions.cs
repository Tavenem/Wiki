using System.Globalization;
using System.Linq.Expressions;
using Tavenem.DataStorage;
using Tavenem.Wiki.Queries;

namespace Tavenem.Wiki;

/// <summary>
/// Extension methods.
/// </summary>
public static class WikiExtensions
{
    /// <summary>
    /// Creates or revises a <see cref="Page"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="editor">
    /// The wiki user who is making this revision.
    /// </param>
    /// <param name="title">
    /// The title of the page.
    /// </param>
    /// <param name="markdown">
    /// <para>
    /// The raw markdown content.
    /// </para>
    /// <para>
    /// If left <see langword="null"/> the existing markdown will be retained.
    /// </para>
    /// </param>
    /// <param name="revisionComment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="isDeleted">Indicates that this page has been marked as deleted.</param>
    /// <param name="owner">
    /// <para>
    /// The new owner of the page.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The users allowed to edit this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The users allowed to view this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="redirectTitle">
    /// If this page will redirect to another, this indicates the title of the destination.
    /// </param>
    /// <param name="originalTitle">
    /// The original title of the page, if it is being renamed.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the page was revised; <see langword="false"/> if the page could
    /// not be revised (usually because the editor did not have permission to make an associated
    /// change).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <para>
    /// The namespace was <see cref="WikiOptions.FileNamespace"/>.
    /// </para>
    /// <para>
    /// Or, a <paramref name="redirectTitle"/> was provided for a page in the category, group, or
    /// user namespaces. Redirects in those namespaces are not permitted.
    /// </para>
    /// <para>
    /// Or, a page in the group or user namespaces was given a domain. Pages in those namespaces
    /// cannot be given domains.
    /// </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>
    /// The target namespace is reserved.
    /// </para>
    /// <para>
    /// Or, the namespace of either the original or renamed page is the category, file, group,
    /// script, or user namespace, and the namespaces of the original and renamed page do not match.
    /// Moving a page to or from those namespaces is not permitted.
    /// </para>
    /// </exception>
    public static async Task<bool> AddOrReviseWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IWikiUser editor,
        PageTitle title,
        string? markdown = null,
        string? revisionComment = null,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null,
        PageTitle? originalTitle = null)
    {
        if (string.Equals(
            title.Namespace,
            options.FileNamespace,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"Pages cannot be edited in the {nameof(WikiOptions.FileNamespace)} namespace with this method.",
                nameof(title));
        }
        if (!string.IsNullOrEmpty(title.Domain)
            && (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0
            || string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0))
        {
            throw new ArgumentException($"Cannot assign a page in the {title.Namespace} namespace to a domain", nameof(title));
        }

        var page = await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            editor,
            true)
            .ConfigureAwait(false);
        var hasPermission = CheckEditPermissions(
            options,
            page,
            isDeleted
                || (originalTitle.HasValue && !originalTitle.Equals(title)),
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups);
        if (!hasPermission)
        {
            return false;
        }

        Page? originalPage = null;
        if (originalTitle.HasValue
            && !originalTitle.Equals(title))
        {
            originalPage = await GetWikiPageAsync(
                dataStore,
                options,
                userManager,
                groupManager,
                originalTitle.Value,
                editor,
                true)
                .ConfigureAwait(false);

            var hasOriginalPermission = CheckEditPermissions(
                options,
                originalPage,
                true,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups);
            if (!hasOriginalPermission)
            {
                return false;
            }
        }

        if (originalPage is not null)
        {
            await originalPage.RenameAsync(
                options,
                dataStore,
                title,
                editor.Id,
                isDeleted ? null : markdown ?? originalPage.MarkdownContent,
                revisionComment,
                owner,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups,
                redirectTitle)
                .ConfigureAwait(false);
        }
        else if (isDeleted)
        {
            await page.UpdateAsync(
                options,
                dataStore,
                editor.Id,
                null,
                revisionComment,
                owner,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups,
                null)
                .ConfigureAwait(false);
        }
        else
        {
            await page.UpdateAsync(
                options,
                dataStore,
                editor.Id,
                markdown ?? page.MarkdownContent,
                revisionComment,
                owner,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups,
                redirectTitle)
                .ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Gets the index of the first character in this <see cref="string"/> which satisfies the
    /// given <paramref name="condition"/>.
    /// </summary>
    /// <param name="str">This <see cref="string"/>.</param>
    /// <param name="condition">A condition which a character in the <see cref="string"/> must
    /// satisfy.</param>
    /// <param name="startIndex">The index at which to begin searching.</param>
    /// <returns>
    /// The index of the first character in the <see cref="string"/> which satisfies the given
    /// <paramref name="condition"/>; or -1 if no characters satisfy the condition.
    /// </returns>
    public static int Find(this string str, Func<char, bool> condition, int startIndex = 0)
    {
        for (var i = startIndex; i < str.Length; i++)
        {
            if (condition(str[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the category page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the category.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="Category"/> which corresponds to the <paramref name="title"/> given.
    /// </returns>
    public static async Task<Category> GetCategoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        IWikiUser? user = null)
    {
        var page = await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            user,
            true)
            .ConfigureAwait(false);

        if (page is not Category category
            || !page.Permission.HasFlag(WikiPermission.Read))
        {
            var empty = Category.Empty(title);
            empty.Permission = page.Permission;
            if (string.IsNullOrEmpty(empty.Title.Title))
            {
                empty.DisplayTitle ??= options.MainPageTitle;
            }
            return empty;
        }

        var pages = new List<Page>();
        var files = new List<WikiFile>();
        var subcategories = new List<Category>();
        if (category.Children is not null)
        {
            foreach (var childTitle in category.Children)
            {
                var child = await GetExistingWikiPageAsync(dataStore, options, childTitle)
                    .ConfigureAwait(false);
                if (child is null)
                {
                    continue;
                }
                if (child is Category subcategory)
                {
                    subcategories.Add(subcategory);
                }
                else if (child is WikiFile file)
                {
                    files.Add(file);
                }
                else
                {
                    pages.Add(child);
                }
            }
        }

        category.Files = files
            .Select(x => new CategoryFile(x.Title, x.FileSize))
            .GroupBy(x => StringInfo.GetNextTextElement(x.Title.Title ?? string.Empty, 0))
            .ToDictionary(
                x => x.Key,
                x => x.OrderBy(y => y.Title).ToList());
        category.Pages = pages
            .Select(x => x.Title)
            .GroupBy(x => StringInfo.GetNextTextElement(x.Title ?? string.Empty, 0))
            .ToDictionary(
                x => x.Key,
                x => x.OrderBy(y => y.Title).ToList());
        category.Subcategories = subcategories
            .Select(x => new Subcategory(x.Title, x.Children?.Count ?? 0))
            .GroupBy(x => StringInfo.GetNextTextElement(x.Title.Title ?? string.Empty, 0))
            .ToDictionary(
                x => x.Key,
                x => x.OrderBy(y => y.Title.Title).ToList());

        return category;
    }

    /// <summary>
    /// Gets the category page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the category.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="Category"/> which corresponds to the <paramref name="title"/> given.
    /// </returns>
    public static async Task<Category> GetCategoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        string userId) => await GetCategoryAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false));

    /// <summary>
    /// Gets the group page with the given group ID.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="groupId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of the group.
    /// </para>
    /// <para>
    /// If no group with the given ID is found, an attempt will be made to find a group with a
    /// matching <see cref="IWikiOwner.DisplayName"/>.
    /// </para>
    /// </param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="GroupPage"/> which corresponds to the group ID given.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the group or page does not exist.
    /// </para>
    /// </returns>
    public static async Task<GroupPage> GetGroupPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string groupId,
        IWikiUser? user = null)
    {
        GroupPage page;
        var articleGroup = await groupManager.FindByIdAsync(groupId)
            .ConfigureAwait(false);
        if (articleGroup is null)
        {
            articleGroup = await groupManager.FindByNameAsync(groupId);
            page = await dataStore.GetWikiPageAsync<GroupPage>(
                options,
                new(articleGroup?.Id, options.GroupNamespace),
                true,
                true)
                .ConfigureAwait(false);
        }
        else
        {
            page = await dataStore.GetWikiPageAsync<GroupPage>(
                options,
                new(articleGroup.Id, options.GroupNamespace),
                true,
                true)
                .ConfigureAwait(false);
        }

        page.Permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            page.Title,
            page)
            .ConfigureAwait(false);

        if (!page.Permission.HasFlag(WikiPermission.Read))
        {
            var empty = GroupPage.Empty(new(groupId, options.GroupNamespace));
            empty.Permission = page.Permission;
            if (string.IsNullOrEmpty(empty.Title.Title))
            {
                empty.DisplayTitle ??= options.MainPageTitle;
            }
            return empty;
        }

        if (user is not null && articleGroup is not null)
        {
            if (user is not null
                && (user.IsWikiAdmin
                || user.Groups?.Contains(articleGroup.Id) == true))
            {
                page.OwnerObject = articleGroup;
            }
            else
            {
                page.OwnerObject = new WikiGroup
                {
                    DisplayName = articleGroup.DisplayName,
                    Id = articleGroup.Id,
                };
            }
        }

        var users = await groupManager.GetUsersInGroupAsync(articleGroup);
        if (users.Count > 0)
        {
            page.Users = [.. users];
        }

        return page;
    }

    /// <summary>
    /// Gets the group page with the given group ID.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="groupId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of the group.
    /// </para>
    /// <para>
    /// If no group with the given ID is found, an attempt will be made to find a group with a
    /// matching <see cref="IWikiOwner.DisplayName"/>.
    /// </para>
    /// </param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="GroupPage"/> which corresponds to the group ID given.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the group or page does not exist.
    /// </para>
    /// </returns>
    public static async Task<GroupPage> GetGroupPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string groupId,
        string userId) => await GetGroupPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            groupId,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false));

    /// <summary>
    /// Gets a page of revision information for a wiki page.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="request">A request record.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="PagedRevisionInfo"/> record with information for the requested wiki page.
    /// </para>
    /// </returns>
    public static async Task<PagedRevisionInfo> GetHistoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        HistoryRequest request,
        IWikiUser? user = null)
    {
        var page = await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            request.Title,
            user,
            true)
            .ConfigureAwait(false);
        if (request.Start > request.End
            || !page.Permission.HasFlag(WikiPermission.Read))
        {
            return new(null, page.Permission, null);
        }

        var history = await page.GetHistoryAsync(
            dataStore,
            request.PageNumber,
            request.PageSize,
            request.Start.HasValue
                ? new DateTimeOffset(request.Start.Value, TimeSpan.Zero)
                : null,
            request.End.HasValue
                ? new DateTimeOffset(request.End.Value, TimeSpan.Zero)
                : null);

        List<IWikiUser>? editors = null;
        var editorIds = new HashSet<string>();
        foreach (var revision in history)
        {
            if (editorIds.Contains(revision.Editor))
            {
                continue;
            }

            var editor = await userManager.FindByIdAsync(revision.Editor)
                .ConfigureAwait(false);
            if (editor?.IsDeleted != false)
            {
                continue;
            }

            editorIds.Add(editor.Id);

            if (user is not null
                && (user.IsWikiAdmin
                || string.CompareOrdinal(user.Id, editor.Id) == 0))
            {
                (editors ??= []).Add(editor);
            }
            else if (editor.IsDeleted)
            {
                (editors ??= []).Add(new WikiUser
                {
                    Id = editor.Id,
                });
            }
            else
            {
                (editors ??= []).Add(new WikiUser
                {
                    DisplayName = editor.DisplayName,
                    Id = editor.Id,
                    IsWikiAdmin = editor.IsWikiAdmin,
                });
            }
        }

        return new PagedRevisionInfo(
            editors,
            page.Permission,
            history);
    }

    /// <summary>
    /// Gets a page of revision information for a wiki page.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="request">A request record.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="PagedRevisionInfo"/> record with information for the requested wiki page.
    /// </para>
    /// </returns>
    public static async Task<PagedRevisionInfo> GetHistoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        HistoryRequest request,
        string? userId = null) => await GetHistoryAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            request,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false));

    /// <summary>
    /// Determines the permission the given user has for the wiki page with the given <paramref
    /// name="title"/>.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/> instance.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static ValueTask<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        PageTitle title,
        IWikiUser? user)
    {
        if (user?.IsDeleted == true
            || user?.IsDisabled == true)
        {
            return ValueTask.FromResult(WikiPermission.None);
        }
        if (user?.IsWikiAdmin == true)
        {
            return ValueTask.FromResult(WikiPermission.All);
        }

        return GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, title);
    }

    /// <summary>
    /// Determines the permission the given user has for the given page.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="page">The wiki page.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/> instance.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static ValueTask<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        Page page,
        IWikiUser? user)
    {
        if (user?.IsDeleted == true
            || user?.IsDisabled == true)
        {
            return ValueTask.FromResult(WikiPermission.None);
        }
        if (user?.IsWikiAdmin == true)
        {
            return ValueTask.FromResult(WikiPermission.All);
        }

        return GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, page.Title, page);
    }

    /// <summary>
    /// Determines the permission the user with the given ID has for the given page.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="page">The wiki page.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static async Task<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        Page page,
        string? userId = null) => await GetPermissionAsync(
            userManager,
            options,
            dataStore,
            groupManager,
            page,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false))
        .ConfigureAwait(false);

    /// <summary>
    /// Determines the permission the user with the given ID has for the wiki page with the given
    /// <paramref name="title"/>.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static async Task<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        PageTitle title,
        string? userId = null) => await GetPermissionAsync(
            userManager,
            options,
            dataStore,
            groupManager,
            title,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false))
        .ConfigureAwait(false);

    /// <summary>
    /// Gets a page of wiki items which fit one of the special conditions in the <see
    /// cref="SpecialListType"/> enumeration.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="request">A record with information about the request.</param>
    /// <returns>
    /// A <see cref="PagedList{T}"/> with <see cref="LinkInfo"/> records for all the pages that
    /// satisfy the request.
    /// </returns>
    /// <remarks>
    /// This method should not be used for <see cref="SpecialListType.What_Links_Here"/>, which has
    /// its own call that takes a different type of request record (<see
    /// cref="GetWhatLinksHereAsync(IDataStore, WikiOptions, TitleRequest)"/>). If this
    /// method is called with <see cref="SpecialListType.What_Links_Here"/> as the value of <see
    /// cref="SpecialListRequest.Type"/>, the result will always be empty.
    /// </remarks>
    public static async Task<PagedList<LinkInfo>> GetSpecialListAsync(
        this IDataStore dataStore,
        SpecialListRequest request)
    {
        if (request.Type == SpecialListType.Missing_Pages)
        {
            return await GetMissingPagesAsync(dataStore, request);
        }
        else if (request.Type == SpecialListType.What_Links_Here)
        {
            return new(null, 1, request.PageSize, 0);
        }

        var items = await GetSpecialListInnerAsync(request, dataStore);
        return new(
            items.Select(x => new LinkInfo(
                x.Title,
                x is Category category ? category.Children?.Count ?? 0 : 0,
                x is WikiFile file1 ? file1.FileSize : 0,
                x is WikiFile file2 ? file2.FileType : null)),
            items.PageNumber,
            items.PageSize,
            items.TotalCount);
    }

    /// <summary>
    /// Gets a page of wiki pages which share the given title parts.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="request">A <see cref="TitleRequest"/> instance.</param>
    /// <returns>
    /// A <see cref="PagedList{T}"/> with <see cref="LinkInfo"/> records for all the pages that
    /// satisfy the request.
    /// </returns>
    public static async Task<PagedList<LinkInfo>> GetTitleAsync(
        this IDataStore dataStore,
        TitleRequest request)
    {
        var items = await GetListAsync<Page>(
            dataStore,
            request.PageNumber,
            request.PageSize,
            request.Sort,
            request.Descending,
            request.Filter,
            x => x.Title.IsMatch(request.Title));
        return new(
            items.Select(x => new LinkInfo(
                x.Title,
                x is Category category ? category.Children?.Count ?? 0 : 0,
                x is WikiFile file1 ? file1.FileSize : 0,
                x is WikiFile file2 ? file2.FileType : null)),
            items.PageNumber,
            items.PageSize,
            items.TotalCount);
    }

    /// <summary>
    /// Gets the user page with the given user ID.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of the user.
    /// </para>
    /// <para>
    /// If no user with the given ID is found, an attempt will be made to find a user with a
    /// matching <see cref="IWikiOwner.DisplayName"/>.
    /// </para>
    /// </param>
    /// <param name="requestingUser">
    /// <para>
    /// The requesting <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="UserPage"/> which corresponds to the user ID given.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the user or page does not exist.
    /// </para>
    /// </returns>
    public static async Task<UserPage> GetUserPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string userId,
        IWikiUser? requestingUser = null)
    {
        UserPage page;
        var articleUser = await userManager.FindByIdAsync(userId)
            .ConfigureAwait(false);
        if (articleUser is null)
        {
            articleUser = await userManager.FindByNameAsync(userId)
                .ConfigureAwait(false);
            page = await dataStore.GetWikiPageAsync<UserPage>(
                options,
                new(articleUser?.Id, options.UserNamespace),
                true,
                true)
                .ConfigureAwait(false);
        }
        else
        {
            page = await dataStore.GetWikiPageAsync<UserPage>
                (options,
                new(articleUser.Id, options.UserNamespace),
                true,
                true)
                .ConfigureAwait(false);
        }

        var permission = await GetPermissionInnerAsync(
            requestingUser,
            options,
            dataStore,
            userManager,
            groupManager,
            page.Title,
            page)
            .ConfigureAwait(false);

        if (!permission.HasFlag(WikiPermission.Read)
            || articleUser is null)
        {
            var empty = UserPage.Empty(new(userId, options.UserNamespace));
            empty.Permission = permission;
            if (string.IsNullOrEmpty(empty.Title.Title))
            {
                empty.DisplayTitle ??= options.MainPageTitle;
            }
            return empty;
        }
        page.Permission = permission;

        if (requestingUser is not null
            && (requestingUser.IsWikiAdmin
            || string.Equals(requestingUser.Id, articleUser.Id)))
        {
            page.OwnerObject = articleUser;
        }
        else if (articleUser.IsDeleted)
        {
            page.OwnerObject = new WikiUser
            {
                Id = articleUser.Id,
                IsWikiAdmin = articleUser.IsWikiAdmin,
            };
        }
        else
        {
            page.OwnerObject = new WikiUser
            {
                DisplayName = articleUser.DisplayName,
                Id = articleUser.Id,
                IsWikiAdmin = articleUser.IsWikiAdmin,
            };
        }

        if (articleUser.Groups is not null)
        {
            page.Groups = [];
            foreach (var id in articleUser.Groups)
            {
                var group = await groupManager.FindByIdAsync(id);
                if (group is null
                    || string.IsNullOrEmpty(group.OwnerId))
                {
                    continue;
                }
                page.Groups.Add(group);
            }
        }

        return page;
    }

    /// <summary>
    /// Gets the user page with the given user ID.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of the user.
    /// </para>
    /// <para>
    /// If no user with the given ID is found, an attempt will be made to find a user with a
    /// matching <see cref="IWikiOwner.DisplayName"/>.
    /// </para>
    /// </param>
    /// <param name="requestingUserId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// <para>
    /// A <see cref="UserPage"/> which corresponds to the user ID given.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the user or page does not exist.
    /// </para>
    /// </returns>
    public static async Task<UserPage> GetUserPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string userId,
        string requestingUserId) => await GetUserPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            userId,
            await userManager.FindByIdAsync(requestingUserId));

    /// <summary>
    /// Gets a page of the wiki pages which link to the given title and namespace.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="request">A record with information about the request.</param>
    /// <returns>
    /// A <see cref="PagedList{T}"/> with <see cref="LinkInfo"/> records for all the pages that
    /// link to the given item.
    /// </returns>
    public static async Task<PagedList<LinkInfo>> GetWhatLinksHereAsync(
        this IDataStore dataStore,
        WikiOptions options,
        TitleRequest request)
    {
        var title = request.Title;
        if (title.Title?.Equals(options.MainPageTitle) == true)
        {
            title = title.WithTitle(null);
        }
        var page = await IPage<Page>
            .GetExistingPageAsync<Page>(dataStore, request.Title)
            .ConfigureAwait(false);
        if (page is null
            || (page.References is null
            && page.TransclusionReferences is null
            && page.RedirectReferences is null))
        {
            return new(
                null,
                1,
                request.PageSize,
                0);
        }

        var allReferences = new HashSet<PageTitle>();
        if (page.References is not null)
        {
            allReferences.UnionWith(page.References);
        }
        if (page.TransclusionReferences is not null)
        {
            allReferences.UnionWith(page.TransclusionReferences);
        }
        if (page.RedirectReferences is not null)
        {
            allReferences.UnionWith(page.RedirectReferences);
        }

        var references = !string.IsNullOrWhiteSpace(request.Filter)
            ? allReferences.Where(x => x.Title?.Contains(request.Filter) == true)
            : allReferences;

        var pages = new List<Page>();
        foreach (var reference in references)
        {
            var referringPage = await dataStore.GetExistingWikiPageAsync(options, reference)
                .ConfigureAwait(false);
            if (referringPage is not null)
            {
                pages.Add(referringPage);
            }
        }

        if (string.Equals(request.Sort, "name", StringComparison.OrdinalIgnoreCase))
        {
            pages.Sort((x, y) => (x.Title.Title ?? string.Empty).CompareTo(y.Title.Title ?? string.Empty));
        }
        else if (string.Equals(request.Sort, "timestamp", StringComparison.OrdinalIgnoreCase))
        {
            pages.Sort((x, y) => (x.Revision?.TimestampTicks ?? 0)
                .CompareTo(y.Revision?.TimestampTicks ?? 0));
        }

        if (request.Descending)
        {
            pages.Reverse();
        }

        return new(
            pages
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new LinkInfo(
                    x.Title,
                    x is Category category ? category.Children?.Count ?? 0 : 0,
                    x is WikiFile file1 ? file1.FileSize : 0,
                    x is WikiFile file2 ? file2.FileType : null)),
            request.PageNumber,
            request.PageSize,
            pages.Count);
    }

    /// <summary>
    /// Retrieves a wiki <see cref="Archive"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="domain">
    /// The domain to archive; or an empty string to archive content without a domain; or <see
    /// langword="null"/> to archive the entire wiki.
    /// </param>
    /// <returns>An <see cref="Archive"/> instance.</returns>
    public static async Task<Archive> GetWikiArchiveAsync(
        this IDataStore dataStore,
        WikiOptions options,
        string? domain = null)
    {
        var hasDomain = !string.IsNullOrEmpty(domain);
        var fullWiki = domain is null;

        var archive = new Archive
        {
            Options = options,
        };

        var pages = dataStore.Query<Page>();
        if (hasDomain)
        {
            pages = pages.Where(x => x.Title.Domain == domain);
        }
        else if (!fullWiki)
        {
            pages = pages.Where(x => x.Title.Domain == null);
        }
        await foreach (var page in pages.AsAsyncEnumerable())
        {
            archive.Pages ??= [];
            archive.Pages.Add(page.GetArchiveCopy());
        }

        return archive;
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <param name="exactMatchOnly">
    /// If <see langword="true"/> a case-insensitive match will not be attempted if an exact match
    /// is not found.
    /// </param>
    /// <returns>
    /// The <see cref="IPage{T}"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<T> GetWikiPageAsync<T>(
        this IDataStore dataStore,
        WikiOptions options,
        PageTitle title,
        bool noRedirect = false,
        bool exactMatchOnly = false) where T : class, IIdItem, IPage<T>
    {
        if (title.Title?.Equals(options.MainPageTitle) == true)
        {
            title = title.WithTitle(null);
        }
        return await IPage<T>
            .GetPageAsync<T>(dataStore, title, exactMatchOnly, noRedirect)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <param name="exactMatchOnly">
    /// If <see langword="true"/> a case-insensitive match will not be attempted if an exact match
    /// is not found.
    /// </param>
    /// <returns>
    /// The <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        PageTitle title,
        bool noRedirect = false,
        bool exactMatchOnly = false)
    {
        if (title.Title?.Equals(options.MainPageTitle) == true)
        {
            title = title.WithTitle(null);
        }
        if (string.CompareOrdinal(title.Namespace, options.CategoryNamespace) == 0)
        {
            return await GetWikiPageAsync<Category>(dataStore, options, title, noRedirect, exactMatchOnly)
                .ConfigureAwait(false);
        }
        if (string.CompareOrdinal(title.Namespace, options.FileNamespace) == 0)
        {
            return await GetWikiPageAsync<WikiFile>(dataStore, options, title, noRedirect, exactMatchOnly)
                .ConfigureAwait(false);
        }
        if (string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0)
        {
            return await GetWikiPageAsync<UserPage>(dataStore, options, title, true, true)
                .ConfigureAwait(false);
        }
        if (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0)
        {
            return await GetWikiPageAsync<GroupPage>(dataStore, options, title, true, true)
                .ConfigureAwait(false);
        }
        return await GetWikiPageAsync<Article>(dataStore, options, title, noRedirect, exactMatchOnly)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <param name="time">
    /// If not <see langword="null"/>, the most recent revision as of the given time is retrieved,
    /// rather than the current state of the page.
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        IWikiUser? user = null,
        bool noRedirect = false,
        DateTimeOffset? time = null)
    {
        if (string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            return await GetUserPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }
        else if (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            return await GetGroupPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }

        var page = await dataStore
            .GetWikiPageAsync(options, title, noRedirect)
            .ConfigureAwait(false);

        page.Permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            page)
            .ConfigureAwait(false);

        if (page.Permission.HasFlag(WikiPermission.Read))
        {
            if (time.HasValue)
            {
                page.Html = await page.GetHtmlAsync(options, dataStore, time.Value)
                    .ConfigureAwait(false);
            }
        }
        else
        {
            var permission = page.Permission;
            page = Page.Empty(title);
            page.Permission = permission;
        }

        if (string.IsNullOrEmpty(page.Title.Title))
        {
            page.DisplayTitle ??= options.MainPageTitle;
        }

        return page;
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <param name="time">
    /// If not <see langword="null"/>, the most recent revision as of the given time is retrieved,
    /// rather than the current state of the page.
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        string? userId = null,
        bool noRedirect = false,
        DateTimeOffset? time = null) => await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false),
            noRedirect,
            time);

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="timestamp">
    /// If not <see langword="null"/>, the most recent revision as of the given time is retrieved,
    /// rather than the current state of the page.
    /// </param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        long timestamp,
        IWikiUser? user = null,
        bool noRedirect = false) => await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            user,
            noRedirect,
            new DateTimeOffset(timestamp, TimeSpan.Zero));

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="timestamp">
    /// If not <see langword="null"/>, the most recent revision as of the given time is retrieved,
    /// rather than the current state of the page.
    /// </param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will not be followed. The original matching item will be
    /// returned, even if it is a redirect.
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        long timestamp,
        string? userId = null,
        bool noRedirect = false) => await GetWikiPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false),
            noRedirect,
            new DateTimeOffset(timestamp, TimeSpan.Zero));

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="firstTime">
    /// <para>
    /// The first revision time to compare.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the revision at <paramref name="secondTime"/> will be compared
    /// with the previous version.
    /// </para>
    /// <para>
    /// If both are <see langword="null"/> the current version of the page will be compared with the
    /// previous version.
    /// </para>
    /// </param>
    /// <param name="secondTime">
    /// <para>
    /// The second revision time to compare.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the revision at <paramref name="firstTime"/> will be compared with
    /// the current version.
    /// </para>
    /// <para>
    /// If both are <see langword="null"/> the current version of the page will be compared with the
    /// previous version.
    /// </para>
    /// </param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>.
    /// </returns>
    public static async Task<Page> GetWikiPageDiffAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        DateTimeOffset? firstTime = null,
        DateTimeOffset? secondTime = null,
        IWikiUser? user = null)
    {
        Page? page = null;
        if (string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            page = await GetUserPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }
        else if (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            page = await GetGroupPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }

        if (page is null)
        {
            page = await dataStore
                .GetWikiPageAsync(options, title, true)
                .ConfigureAwait(false);

            page.Permission = await GetPermissionInnerAsync(
                user,
                options,
                dataStore,
                userManager,
                groupManager,
                title,
                page)
                .ConfigureAwait(false);
        }

        if (page.Permission.HasFlag(WikiPermission.Read))
        {
            if (firstTime.HasValue)
            {
                if (secondTime.HasValue)
                {
                    page.Html = await page.GetDiffWithOtherHtmlAsync(
                        options,
                        dataStore,
                        firstTime.Value,
                        secondTime.Value)
                        .ConfigureAwait(false);
                }
                else
                {
                    page.Html = await page.GetDiffWithCurrentHtmlAsync(
                        options,
                        dataStore,
                        firstTime.Value)
                        .ConfigureAwait(false);
                }
            }
            else if (secondTime.HasValue)
            {
                page.Html = await page.GetDiffHtmlAsync(
                    options,
                    dataStore,
                    secondTime.Value)
                    .ConfigureAwait(false);
            }
            else
            {
                page.Html = await page.GetDiffHtmlAsync(
                    options,
                    dataStore,
                    DateTimeOffset.UtcNow)
                    .ConfigureAwait(false);
            }

            page.IsDiff = true;
        }
        else
        {
            var permission = page.Permission;
            page = Page.Empty(title);
            page.Permission = permission;
        }

        if (string.IsNullOrEmpty(page.Title.Title))
        {
            page.DisplayTitle ??= options.MainPageTitle;
        }

        return page;
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>, with additional information
    /// suited to editing the item.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>, with
    /// properties configured for editing.
    /// </returns>
    public static async Task<Page> GetWikiPageForEditingAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        IWikiUser? user = null)
    {
        Page? page = null;
        if (string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            page = await GetUserPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }
        else if (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0
            && !string.IsNullOrEmpty(title.Title))
        {
            page = await GetGroupPageAsync(dataStore, options, userManager, groupManager, title.Title, user);
        }

        if (page is null)
        {
            page = await dataStore
                .GetWikiPageAsync(options, title, true)
                .ConfigureAwait(false);

            page.Permission = await GetPermissionInnerAsync(
                user,
                options,
                dataStore,
                userManager,
                groupManager,
                title,
                page)
                .ConfigureAwait(false);
        }

        if (!page.Permission.HasFlag(WikiPermission.Read))
        {
            var empty = Page.Empty(title);
            empty.Permission = page.Permission;
            if (string.IsNullOrEmpty(empty.Title.Title))
            {
                empty.DisplayTitle ??= options.MainPageTitle;
            }
            return empty;
        }

        if (string.IsNullOrEmpty(page.Title.Title))
        {
            page.DisplayTitle ??= options.MainPageTitle;
        }

        var owner = string.IsNullOrEmpty(page.Owner)
            ? null
            : await userManager.FindByIdAsync(page.Owner)
                .ConfigureAwait(false);
        if (owner is not null)
        {
            if (user is not null
                && (user.IsWikiAdmin
                || string.Equals(user.Id, owner.Id)))
            {
                page.OwnerObject = owner;
            }
            else if (owner.IsDeleted)
            {
                page.OwnerObject = new WikiUser
                {
                    Id = owner.Id,
                    IsWikiAdmin = owner.IsWikiAdmin,
                };
            }
            else
            {
                page.OwnerObject = new WikiUser
                {
                    DisplayName = owner.DisplayName,
                    Id = owner.Id,
                    IsWikiAdmin = owner.IsWikiAdmin,
                };
            }
        }

        if (page.AllowedEditors is not null)
        {
            var allowedEditors = new List<IWikiUser>();
            foreach (var id in page.AllowedEditors)
            {
                var editorUser = await userManager.FindByIdAsync(id)
                    .ConfigureAwait(false);
                if (editorUser is null)
                {
                    continue;
                }

                if (user is not null
                    && (user.IsWikiAdmin
                    || string.Equals(user.Id, editorUser.Id)))
                {
                    allowedEditors.Add(editorUser);
                }
                else if (editorUser.IsDeleted)
                {
                    allowedEditors.Add(new WikiUser
                    {
                        Id = editorUser.Id,
                        IsWikiAdmin = editorUser.IsWikiAdmin,
                    });
                }
                else
                {
                    allowedEditors.Add(new WikiUser
                    {
                        DisplayName = editorUser.DisplayName,
                        Id = editorUser.Id,
                        IsWikiAdmin = editorUser.IsWikiAdmin,
                    });
                }
            }
            if (allowedEditors.Count > 0)
            {
                page.AllowedEditorObjects = allowedEditors.AsReadOnly();
            }
        }

        if (page.AllowedViewers is not null)
        {
            var allowedViewers = new List<IWikiUser>();
            foreach (var id in page.AllowedViewers)
            {
                var viewerUser = await userManager.FindByIdAsync(id)
                    .ConfigureAwait(false);
                if (viewerUser is not null)
                {
                    if (user is not null
                        && (user.IsWikiAdmin
                        || string.Equals(user.Id, viewerUser.Id)))
                    {
                        allowedViewers.Add(viewerUser);
                    }
                    else if (viewerUser.IsDeleted)
                    {
                        allowedViewers.Add(new WikiUser
                        {
                            Id = viewerUser.Id,
                            IsWikiAdmin = viewerUser.IsWikiAdmin,
                        });
                    }
                    else
                    {
                        allowedViewers.Add(new WikiUser
                        {
                            DisplayName = viewerUser.DisplayName,
                            Id = viewerUser.Id,
                            IsWikiAdmin = viewerUser.IsWikiAdmin,
                        });
                    }
                }
            }
            if (allowedViewers.Count > 0)
            {
                page.AllowedViewerObjects = allowedViewers.AsReadOnly();
            }
        }

        if (page.AllowedEditorGroups is not null)
        {
            var allowedEditorGroups = new List<IWikiGroup>();
            foreach (var id in page.AllowedEditorGroups)
            {
                var group = await groupManager.FindByIdAsync(id)
                    .ConfigureAwait(false);
                if (group is not null)
                {
                    if (user is not null
                        && (user.IsWikiAdmin
                        || user.Groups?.Contains(group.Id) == true))
                    {
                        allowedEditorGroups.Add(group);
                    }
                    else
                    {
                        allowedEditorGroups.Add(new WikiGroup
                        {
                            DisplayName = group.DisplayName,
                            Id = group.Id,
                        });
                    }
                }
            }
            if (allowedEditorGroups.Count > 0)
            {
                page.AllowedEditorGroupObjects = allowedEditorGroups.AsReadOnly();
            }
        }

        if (page.AllowedViewerGroups is not null)
        {
            var allowedViewerGroups = new List<IWikiGroup>();
            foreach (var id in page.AllowedViewerGroups)
            {
                var group = await groupManager.FindByIdAsync(id);
                if (group is not null)
                {
                    if (user is not null
                        && (user.IsWikiAdmin
                        || user.Groups?.Contains(group.Id) == true))
                    {
                        allowedViewerGroups.Add(group);
                    }
                    else
                    {
                        allowedViewerGroups.Add(new WikiGroup
                        {
                            DisplayName = group.DisplayName,
                            Id = group.Id,
                        });
                    }
                }
            }
            if (allowedViewerGroups.Count > 0)
            {
                page.AllowedViewerGroupObjects = allowedViewerGroups.AsReadOnly();
            }
        }

        return page;
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="title"/>, with additional information
    /// suited to editing the item.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="Page"/> which corresponds to the given <paramref name="title"/>, with
    /// properties configured for editing.
    /// </returns>
    public static async Task<Page> GetWikiPageForEditingAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        string userId) => await GetWikiPageForEditingAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            await userManager.FindByIdAsync(userId)
                .ConfigureAwait(false));

    /// <summary>
    /// Converts a <see cref="DateTimeOffset"/> to a formatted display string.
    /// </summary>
    /// <param name="timestamp">The <see cref="DateTimeOffset"/> to format.</param>
    /// <returns>A formatted string.</returns>
    public static string ToWikiDisplayString(this DateTimeOffset timestamp)
        => timestamp.ToUniversalTime().LocalDateTime.ToString("F", CultureInfo.CurrentCulture);

    /// <summary>
    /// Converts a <see cref="long"/> representing the number of ticks in a <see
    /// cref="DateTimeOffset"/> to a formatted display string.
    /// </summary>
    /// <param name="timestamp">The <see cref="long"/> to format.</param>
    /// <returns>A formatted string.</returns>
    public static string ToWikiDisplayString(this long timestamp)
        => new DateTimeOffset(timestamp, TimeSpan.Zero).LocalDateTime.ToString("F", CultureInfo.CurrentCulture);

    /// <summary>
    /// Converts the initial character of the specified string to upper case, leaving all others
    /// unchanged. Multi-codepoint Unicode characters are converted as a unit.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The specified string with its initial character converted to upper case.</returns>
    public static string ToWikiTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var si = new StringInfo(value.Trim());
        if (si.LengthInTextElements == 0)
        {
            return value;
        }
        else if (si.LengthInTextElements == 1)
        {
            return value.ToUpper(CultureInfo.CurrentCulture);
        }
        else
        {
            return si.SubstringByTextElements(0, 1).ToUpper(CultureInfo.CurrentCulture)
                + si.SubstringByTextElements(1);
        }
    }

    internal static async Task<Page?> GetExistingWikiPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        PageTitle title,
        bool noRedirect = true)
    {
        if (string.CompareOrdinal(title.Namespace, options.CategoryNamespace) == 0)
        {
            return await IPage<Category>
                .GetExistingPageAsync<Category>(dataStore, title)
                .ConfigureAwait(false);
        }
        if (string.CompareOrdinal(title.Namespace, options.FileNamespace) == 0)
        {
            return await IPage<WikiFile>
                .GetExistingPageAsync<WikiFile>(dataStore, title, noRedirect: noRedirect)
                .ConfigureAwait(false);
        }
        return await IPage<Article>
            .GetExistingPageAsync<Article>(dataStore, title, noRedirect: noRedirect)
            .ConfigureAwait(false);
    }

    internal static int IndexOfUnescaped(this ReadOnlySpan<char> span, char target)
    {
        for (var i = 0; i < span.Length; i++)
        {
            if (span.IsUnescapedMatch(i, target))
            {
                return i;
            }
        }
        return -1;
    }

    internal static int IndexOfUnescaped(this string value, char target)
    {
        for (var i = 0; i < value.Length; i++)
        {
            if (value.IsUnescapedMatch(i, target))
            {
                return i;
            }
        }
        return -1;
    }

    internal static bool IsUnescapedMatch(this ReadOnlySpan<char> span, int index, char target)
    {
        if (index >= span.Length || index < 0 || span[index] != target)
        {
            return false;
        }
        var i = index - 1;
        var escapes = 0;
        while (i > 0 && span[i] == '\\')
        {
            escapes++;
        }
        return escapes == 0 || escapes % 2 == 1;
    }

    internal static bool IsUnescapedMatch(this string value, int index, char target)
    {
        if (index >= value.Length || index < 0 || value[index] != target)
        {
            return false;
        }
        var i = index - 1;
        var escapes = 0;
        while (i > 0 && value[i] == '\\')
        {
            escapes++;
        }
        return escapes == 0 || escapes % 2 == 1;
    }

    private static bool CheckEditPermissions(
        WikiOptions options,
        Page page,
        bool isDeletedOrRenamed = false,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null)
    {
        if (!page.Permission.HasFlag(WikiPermission.Write))
        {
            return false;
        }
        if (!page.Exists)
        {
            if (!page.Permission.HasFlag(WikiPermission.Create))
            {
                return false;
            }
            if (options.ReservedNamespaces.Any(x => string.CompareOrdinal(page.Title.Namespace, x) == 0))
            {
                return false;
            }
        }
        if (!string.IsNullOrEmpty(page.Owner)
            && !page.Permission.HasFlag(WikiPermission.SetOwner))
        {
            return false;
        }

        if (isDeletedOrRenamed
            && !page.Permission.HasFlag(WikiPermission.Delete))
        {
            return false;
        }

        if (!page.Permission.HasFlag(WikiPermission.SetPermissions))
        {
            if (!page.Exists)
            {
                if (allowedEditors is not null
                    || allowedEditorGroups is not null
                    || allowedViewers is not null
                    || allowedViewerGroups is not null)
                {
                    return false;
                }
            }
            else
            {
                if (page.AllowedEditors is null)
                {
                    if (allowedEditors is not null)
                    {
                        return false;
                    }
                }
                else if (allowedEditors is null
                    || !page.AllowedEditors.Order().SequenceEqual(allowedEditors.Order()))
                {
                    return false;
                }

                if (page.AllowedEditorGroups is null)
                {
                    if (allowedEditorGroups is not null)
                    {
                        return false;
                    }
                }
                else if (allowedEditorGroups is null
                    || !page.AllowedEditorGroups.Order().SequenceEqual(allowedEditorGroups.Order()))
                {
                    return false;
                }

                if (page.AllowedViewers is null)
                {
                    if (allowedViewers is not null)
                    {
                        return false;
                    }
                }
                else if (allowedViewers is null
                    || !page.AllowedViewers.Order().SequenceEqual(allowedViewers.Order()))
                {
                    return false;
                }

                if (page.AllowedViewerGroups is null)
                {
                    if (allowedViewerGroups is not null)
                    {
                        return false;
                    }
                }
                else if (allowedViewerGroups is null
                    || !page.AllowedViewerGroups.Order().SequenceEqual(allowedViewerGroups.Order()))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static async Task<IPagedList<T>> GetListAsync<T>(
        IDataStore dataStore,
        int pageNumber = 1,
        int pageSize = 50,
        string? sort = null,
        bool descending = false,
        string? filter = null,
        Expression<Func<T, bool>>? condition = null) where T : Page
    {
        var pageCondition = condition is null
            ? (T x) => x.Exists
            : condition.AndAlso(x => x.Exists);
        if (!string.IsNullOrEmpty(filter))
        {
            pageCondition = pageCondition.AndAlso(x => x.Title.Title != null
                && x.Title.Title.Contains(filter));
        }

        var query = dataStore.Query<T>();
        if (pageCondition is not null)
        {
            query = query.Where(pageCondition);
        }
        if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderBy(x => x.Revision == null ? 0 : x.Revision.TimestampTicks, descending: descending);
        }
        else
        {
            query = query.OrderBy(x => x.Title.Title, descending: descending);
        }

        return await query.GetPageAsync(pageNumber, pageSize);
    }

    private static async Task<PagedList<LinkInfo>> GetMissingPagesAsync(
        IDataStore dataStore,
        SpecialListRequest request)
    {
        Expression<Func<Page, bool>> pageCondition = x => x.IsMissing;
        if (!string.IsNullOrEmpty(request.Filter))
        {
            pageCondition = pageCondition.AndAlso(x => x.Title.Title != null
                && x.Title.Title.Contains(request.Filter));
        }

        var page = await dataStore.Query<Page>()
            .Where(pageCondition)
            .OrderBy(x => x.Title.Title, descending: request.Descending)
            .GetPageAsync(request.PageNumber, request.PageSize)
            .ConfigureAwait(false);
        return new(
            page.Select(x => new LinkInfo(
                x.Title,
                x is Category category ? category.Children?.Count ?? 0 : 0,
                x is WikiFile wikiFile1 ? wikiFile1.FileSize : 0,
                x is WikiFile wikiFile2 ? wikiFile2.FileType : null)),
            request.PageNumber,
            request.PageSize,
            page.TotalCount);
    }

    private static async ValueTask<WikiPermission> GetPermissionInnerAsync(
        IWikiUser? user,
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        PageTitle title,
        Page? page = null)
    {
        var defaultPermission = WikiPermission.None;
        var hasDomain = !string.IsNullOrEmpty(title.Domain);
        if (!hasDomain)
        {
            defaultPermission = user is null
                ? options.DefaultAnonymousPermission
                : options.DefaultRegisteredPermission;
        }

        List<IWikiGroup>? userGroups = null;
        if (hasDomain
            && user is not null
            && !string.IsNullOrEmpty(title.Domain))
        {
            if (options.UserDomains
                && string.CompareOrdinal(title.Domain, user.Id) == 0)
            {
                return WikiPermission.All;
            }
            if (options.GetDomainPermission is not null)
            {
                defaultPermission = await options.GetDomainPermission(user.Id, title.Domain);
            }
            if (user.AllowedViewDomains?.Contains(title.Domain) == true)
            {
                defaultPermission |= WikiPermission.Read;
            }
            if (!defaultPermission.HasFlag(WikiPermission.Read))
            {
                userGroups = [];
                if (user.Groups is not null)
                {
                    foreach (var groupId in user.Groups)
                    {
                        var group = await groupManager.FindByIdAsync(groupId);
                        if (group is not null)
                        {
                            userGroups.Add(group);
                        }
                    }
                }
                if (userGroups.Any(x => x.AllowedViewPages?.Contains(title) == true))
                {
                    defaultPermission |= WikiPermission.Read;
                }
            }
        }

        page ??= await dataStore.GetWikiPageAsync(options, title, true)
            .ConfigureAwait(false);

        var isUserPage = string.CompareOrdinal(page.Title.Namespace, options.UserNamespace) == 0;
        if (isUserPage
            && user is not null
            && !string.IsNullOrEmpty(title.Title)
            && string.Equals(user.Id, title.Title))
        {
            return WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
        }

        var isGroupPage = string.CompareOrdinal(page.Title.Namespace, options.GroupNamespace) == 0;
        if (isGroupPage
            && user is not null
            && !string.IsNullOrEmpty(title.Title))
        {
            var ownerId = await groupManager
                .GetGroupOwnerIdAsync(title.Title)
                .ConfigureAwait(false);
            if (string.Equals(user.Id, ownerId))
            {
                return WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
            }
        }

        if (page is null)
        {
            if (isUserPage)
            {
                return defaultPermission & WikiPermission.Read;
            }
            else if (isGroupPage
                && !string.IsNullOrEmpty(title.Title))
            {
                return user?.Groups?.Contains(title.Title) == true
                    ? WikiPermission.ReadWrite
                    : defaultPermission & WikiPermission.Read;
            }
            return defaultPermission;
        }

        if (user is null)
        {
            return page.AllowedViewers is null
                ? defaultPermission & WikiPermission.Read
                : WikiPermission.None;
        }
        var isReservedNamespace = options
            .ReservedNamespaces
            .Any(x => string.CompareOrdinal(
                x,
                title.Namespace) == 0);

        IWikiUser? owner = null;
        if (!string.IsNullOrEmpty(page.Owner))
        {
            owner = await userManager.FindByIdAsync(page.Owner);

            if (owner is not null
                && string.Equals(user.Id, owner.Id))
            {
                return isReservedNamespace
                    ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions | WikiPermission.SetOwner
                    : WikiPermission.All;
            }

            if (!isGroupPage && user.Groups?.Contains(page.Owner) == true)
            {
                return isReservedNamespace
                    ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions | WikiPermission.SetOwner
                    : WikiPermission.All;
            }
        }

        var isAdminNamespace = options
            .AdminNamespaces
            .Any(x => string.CompareOrdinal(
                x,
                title.Namespace) == 0);
        if (isAdminNamespace
            && user.IsWikiAdmin)
        {
            return isReservedNamespace
                ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions
                : WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
        }

        if (!isUserPage
            && !isGroupPage
            && string.IsNullOrEmpty(page.Owner))
        {
            if (isAdminNamespace)
            {
                return defaultPermission & WikiPermission.ReadWrite;
            }
            else if (isReservedNamespace)
            {
                return defaultPermission & (WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions | WikiPermission.SetOwner);
            }
            else
            {
                return defaultPermission;
            }
        }

        if (page.AllowedViewers?.Contains(user.Id) == false
            && user.AllowedViewPages?.Contains(title) != true
            && (!isGroupPage
                || string.IsNullOrEmpty(title.Title)
                || user.Groups?.Contains(title.Title) != true)
            && (page.AllowedViewerGroups is null
                || user.Groups is null
                || !page.AllowedViewerGroups.Intersect(user.Groups).Any()))
        {
            if (userGroups is null)
            {
                userGroups = [];
                if (user.Groups is not null)
                {
                    foreach (var groupId in user.Groups)
                    {
                        var group = await groupManager.FindByIdAsync(groupId);
                        if (group is not null)
                        {
                            userGroups.Add(group);
                        }
                    }
                }
            }
            if (!userGroups.Any(x => x.AllowedViewPages?.Contains(title) == true))
            {
                return WikiPermission.None;
            }
        }

        if (isUserPage)
        {
            return defaultPermission & WikiPermission.Read;
        }

        if (isGroupPage
            && !string.IsNullOrEmpty(title.Title))
        {
            return user.Groups?.Contains(title.Title) == true
                ? WikiPermission.ReadWrite
                : defaultPermission & WikiPermission.Read;
        }

        var writePermission = page.AllowedEditors?.Contains(user.Id) != false
            || user.AllowedEditPages?.Contains(title) == true
            || (page.AllowedEditorGroups is not null
                && user.Groups is not null
                && page.AllowedEditorGroups.Intersect(user.Groups).Any());
        if (!writePermission)
        {
            if (userGroups is null)
            {
                userGroups = [];
                if (user.Groups is not null)
                {
                    foreach (var groupId in user.Groups)
                    {
                        var group = await groupManager.FindByIdAsync(groupId);
                        if (group is not null)
                        {
                            userGroups.Add(group);
                        }
                    }
                }
            }
            if (userGroups.Any(x => x.AllowedEditPages?.Contains(title) == true))
            {
                writePermission = true;
            }
        }

        return writePermission
            ? WikiPermission.ReadWrite
            : defaultPermission & WikiPermission.Read;
    }

    private static async Task<IPagedList<Page>> GetSpecialListInnerAsync(
        SpecialListRequest request,
        IDataStore dataStore) => request.Type switch
        {
            SpecialListType.All_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter)
                .ConfigureAwait(false),

            SpecialListType.All_Files => await GetListAsync<WikiFile>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter)
                .ConfigureAwait(false),

            SpecialListType.All_Articles => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter)
                .ConfigureAwait(false),

            SpecialListType.All_Redirects => await GetListAsync<Page>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.RedirectTitle.HasValue)
                .ConfigureAwait(false),

            SpecialListType.Broken_Redirects => await GetListAsync<Page>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IsBrokenRedirect)
                .ConfigureAwait(false),

            SpecialListType.Double_Redirects => await GetListAsync<Page>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IsDoubleRedirect)
                .ConfigureAwait(false),

            SpecialListType.Uncategorized_Articles => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.Uncategorized)
                .ConfigureAwait(false),

            SpecialListType.Uncategorized_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.Uncategorized)
                .ConfigureAwait(false),

            SpecialListType.Uncategorized_Files => await GetListAsync<WikiFile>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.Uncategorized)
                .ConfigureAwait(false),

            SpecialListType.Unused_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IsEmpty)
                .ConfigureAwait(false),

            _ => new PagedList<Page>(null, 1, request.PageSize, 0),
        };
}
