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
    /// Creates or revises an <see cref="Article"/> or <see cref="Category"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="editor">
    /// The wiki user who is making this revision.
    /// </param>
    /// <param name="title">
    /// The title of the article. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace to which this article belongs.
    /// </para>
    /// <para>
    /// May be omitted to use <see cref="WikiOptions.DefaultNamespace"/>.
    /// </para>
    /// <para>
    /// May not be <see cref="WikiOptions.FileNamespace"/>. File uploads must be handled separately.
    /// </para>
    /// </param>
    /// <param name="domain">The domain to which this article belongs (if any).</param>
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
    /// <param name="isDeleted">Indicates that this article has been marked as deleted.</param>
    /// <param name="owner">
    /// <para>
    /// The new owner of the article.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The users allowed to edit this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The users allowed to view this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the article.
    /// </para>
    /// </param>
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the article.
    /// </para>
    /// </param>
    /// <param name="originalTitle">
    /// The original title of the article, if it is being renamed.
    /// </param>
    /// <param name="originalWikiNamespace">
    /// <para>
    /// The original namespace to which this article belonged, if it is being moved.
    /// </para>
    /// <para>
    /// May be omitted to use <see cref="WikiOptions.DefaultNamespace"/>.
    /// </para>
    /// </param>
    /// <param name="originalDomain">
    /// The original domain to which this article belonged (if any), if it is being moved.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the article was revised; <see langword="false"/> if the article
    /// could not be revised (usually because the editor did not have permission to make an
    /// associated change).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="wikiNamespace"/> was <see cref="WikiOptions.FileNamespace"/>
    /// </exception>
    public static async Task<bool> AddOrReviseWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IWikiUser editor,
        string title,
        string? wikiNamespace = null,
        string? domain = null,
        string? markdown = null,
        string? revisionComment = null,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        string? originalTitle = null,
        string? originalWikiNamespace = null,
        string? originalDomain = null)
    {
        if (string.Equals(
            wikiNamespace,
            options.FileNamespace,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"{nameof(wikiNamespace)} may not be the value assigned to the {nameof(WikiOptions.FileNamespace)} property of {nameof(WikiOptions)}.",
                nameof(wikiNamespace));
        }

        var item = await GetWikiItemAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            originalTitle ?? title,
            originalWikiNamespace ?? wikiNamespace,
            originalDomain ?? domain,
            editor,
            true);
        if (item.Item is null && string.IsNullOrEmpty(title))
        {
            return false;
        }

        if (!item.Permission.HasFlag(WikiPermission.Write))
        {
            return false;
        }

        if (item.Item is null && !item.Permission.HasFlag(WikiPermission.Create))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(owner)
            && !item.Permission.HasFlag(WikiPermission.SetOwner))
        {
            return false;
        }

        if ((isDeleted
            || !string.Equals(title, originalTitle, StringComparison.Ordinal)
            || !string.Equals(wikiNamespace, originalWikiNamespace, StringComparison.Ordinal)
            || !string.Equals(domain, originalDomain, StringComparison.Ordinal))
            && !item.Permission.HasFlag(WikiPermission.Delete))
        {
            return false;
        }

        if (!item.Permission.HasFlag(WikiPermission.SetPermissions))
        {
            if (item.Item is null)
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
                if (item.Item.AllowedEditors is null)
                {
                    if (allowedEditors is not null)
                    {
                        return false;
                    }
                }
                else if (allowedEditors is null
                    || !item.Item.AllowedEditors.Order().SequenceEqual(allowedEditors.Order()))
                {
                    return false;
                }

                if (item.Item.AllowedEditorGroups is null)
                {
                    if (allowedEditorGroups is not null)
                    {
                        return false;
                    }
                }
                else if (allowedEditorGroups is null
                    || !item.Item.AllowedEditorGroups.Order().SequenceEqual(allowedEditorGroups.Order()))
                {
                    return false;
                }

                if (item.Item.AllowedViewers is null)
                {
                    if (allowedViewers is not null)
                    {
                        return false;
                    }
                }
                else if (allowedViewers is null
                    || !item.Item.AllowedViewers.Order().SequenceEqual(allowedViewers.Order()))
                {
                    return false;
                }

                if (item.Item.AllowedViewerGroups is null)
                {
                    if (allowedViewerGroups is not null)
                    {
                        return false;
                    }
                }
                else if (allowedViewerGroups is null
                    || !item.Item.AllowedViewerGroups.Order().SequenceEqual(allowedViewerGroups.Order()))
                {
                    return false;
                }
            }
        }

        if (item.Item is null)
        {
            if (string.Equals(
                wikiNamespace,
                options.CategoryNamespace,
                StringComparison.OrdinalIgnoreCase))
            {
                await Category.NewAsync(
                    options,
                    dataStore,
                    title!,
                    editor.Id,
                    markdown,
                    domain,
                    owner,
                    allowedEditors,
                    allowedViewers,
                    allowedEditorGroups,
                    allowedViewerGroups);
            }
            else
            {
                await Article.NewAsync(
                    options,
                    dataStore,
                    title!,
                    editor.Id,
                    markdown,
                    wikiNamespace,
                    domain,
                    owner,
                    allowedEditors,
                    allowedViewers,
                    allowedEditorGroups,
                    allowedViewerGroups);
            }
        }
        else if (item.Item is Category category)
        {
            await category.ReviseAsync(
                options,
                dataStore,
                editor.Id,
                title,
                markdown,
                revisionComment,
                domain,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups);
        }
        else
        {
            await item.Item.ReviseAsync(
                options,
                dataStore,
                editor.Id,
                title,
                markdown,
                revisionComment,
                wikiNamespace,
                domain,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups);
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
    /// Gets the category page with the given title.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the category.</param>
    /// <param name="domain">The domain of the category (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="CategoryInfo"/> object which corresponds to the <paramref name="title"/> given.
    /// </returns>
    public static async Task<CategoryInfo> GetCategoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string title,
        string? domain = null,
        IWikiUser? user = null)
    {
        var category = await Category.GetCategoryAsync(options, dataStore, title, domain);

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            options.CategoryNamespace,
            domain,
            category);

        if (category?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read))
        {
            return new(null, permission, null, null, null);
        }

        var articles = new List<Article>();
        var files = new List<WikiFile>();
        var subcategories = new List<Category>();
        foreach (var id in category.ChildIds)
        {
            var child = await dataStore.GetItemAsync<Article>(id);
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
            else if (child is Article article)
            {
                articles.Add(article);
            }
        }

        return new(
            category,
            permission,
            articles
                .Select(x => new CategoryPage(
                    x.Title,
                    x.WikiNamespace))
                .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderBy(y => y.Title).ToList()),
            files
                .Select(x => new CategoryFile(x.Title, x.FileSize))
                .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderBy(y => y.Title).ToList()),
            subcategories
                .Select(x => new Subcategory(x.Title, x.ChildIds.Count))
                .GroupBy(x => StringInfo.GetNextTextElement(x.Title, 0))
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderBy(y => y.Title).ToList()));
    }

    /// <summary>
    /// Gets the category page with the given title.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">The title of the category.</param>
    /// <param name="domain">The domain of the category (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="CategoryInfo"/> object which corresponds to the <paramref name="title"/> given.
    /// </returns>
    public static async Task<CategoryInfo> GetCategoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string title,
        string? domain = null,
        string? userId = null) => await GetCategoryAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            domain,
            await userManager.FindByIdAsync(userId));

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
    /// A <see cref="GroupPageInfo"/> which corresponds to the group ID given; or <see
    /// langword="null"/> if no such group exists.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the group <em>page</em> does not exist, but the
    /// group itself does. In this case, the <see cref="GroupPageInfo.Item"/> will be <see
    /// langword="null"/> but the record's other properties will be set appropriately.
    /// </para>
    /// </returns>
    public static async Task<GroupPageInfo?> GetGroupPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string groupId,
        IWikiUser? user = null)
    {
        var articleGroup = await groupManager.FindByIdAsync(groupId);
        articleGroup ??= await groupManager.FindByNameAsync(groupId);

        if (articleGroup is null)
        {
            return null;
        }

        var wikiItem = await GetWikiItemAsync(
            dataStore,
            options,
            articleGroup.Id,
            options.GroupNamespace,
            null,
            true);

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            articleGroup.Id,
            options.GroupNamespace,
            null,
            wikiItem);

        if (wikiItem?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read))
        {
            return new(
                null,
                null,
                permission,
                null);
        }

        IWikiGroup? group = null;
        if (user is not null)
        {
            if (user is not null
                && (user.IsWikiAdmin
                || user.Groups?.Contains(articleGroup.Id) == true))
            {
                group = articleGroup;
            }
            else
            {
                group = new WikiGroup
                {
                    DisplayName = articleGroup.DisplayName,
                    Id = articleGroup.Id,
                };
            }
        }

        var users = await groupManager.GetUsersInGroupAsync(articleGroup);
        List<WikiUserInfo>? userInfo = null;
        if (users.Count > 0)
        {
            userInfo = new List<WikiUserInfo>();
            foreach (var groupUser in users)
            {
                var userPage = await Article.GetArticleAsync(
                    options,
                    dataStore,
                    groupUser.Id,
                    options.UserNamespace,
                    null,
                    true);
                userInfo.Add(new WikiUserInfo(
                    groupUser.Id,
                    groupUser,
                    userPage is not null));
            }
        }

        return new(
            new WikiUserInfo(articleGroup.Id, group, wikiItem is not null),
            wikiItem,
            permission,
            userInfo);
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
    /// A <see cref="GroupPageInfo"/> which corresponds to the group ID given; or <see
    /// langword="null"/> if no such group exists.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the group <em>page</em> does not exist, but the
    /// group itself does. In this case, the <see cref="GroupPageInfo.Item"/> will be <see
    /// langword="null"/> but the record's other properties will be set appropriately.
    /// </para>
    /// </returns>
    public static async Task<GroupPageInfo?> GetGroupPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string groupId,
        string? userId = null) => await GetGroupPageAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            groupId,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets a page of revision information for the wiki page with the given title and namespace.
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
    /// A <see cref="PagedRevisionInfo"/> record with information for the requested wiki page; or
    /// <see langword="null"/> if no such page exists.
    /// </para>
    /// </returns>
    public static async Task<PagedRevisionInfo?> GetHistoryAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        HistoryRequest request,
        IWikiUser? user = null)
    {
        var item = await GetWikiItemAsync(
            dataStore,
            options,
            request.Title,
            request.WikiNamespace,
            request.Domain,
            true);
        if (item is null)
        {
            return null;
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            request.Title,
            request.WikiNamespace,
            request.Domain,
            item);
        if (request.Start > request.End
            || !permission.HasFlag(WikiPermission.Read))
        {
            return new(null, permission, null);
        }

        var history = await item.GetHistoryAsync(
            dataStore,
            request.PageNumber,
            request.PageSize,
            request.Start.HasValue
                ? new DateTimeOffset(request.Start.Value, TimeSpan.Zero)
                : null,
            request.End.HasValue
                ? new DateTimeOffset(request.End.Value, TimeSpan.Zero)
                : null);

        List<WikiUserInfo>? editors = null;
        foreach (var revision in history)
        {
            var editor = await userManager.FindByIdAsync(revision.Editor);
            if (editor?.IsDeleted != false
                || editors?.Any(x => string.Equals(x.Id, editor.Id)) == true)
            {
                continue;
            }

            IWikiUser? editorUser = null;
            if (user is not null
                && (user.IsWikiAdmin
                || string.Equals(user.Id, editor.Id)))
            {
                editorUser = editor;
            }
            else if (editor.IsDeleted)
            {
                editorUser = new WikiUser
                {
                    Id = editor.Id,
                    IsWikiAdmin = editor.IsWikiAdmin,
                };
            }
            else
            {
                editorUser = new WikiUser
                {
                    DisplayName = editor.DisplayName,
                    Id = editor.Id,
                    IsWikiAdmin = editor.IsWikiAdmin,
                };
            }

            var editorPageExists = await Article.GetArticleAsync(
                options,
                dataStore,
                editor.Id,
                options.UserNamespace,
                null,
                true) is not null;

            (editors ??= new List<WikiUserInfo>()).Add(new WikiUserInfo(
                editor.Id,
                editorUser,
                editorPageExists));
        }

        return new PagedRevisionInfo(
            editors,
            permission,
            new PagedListDTO<Revision>(history));
    }

    /// <summary>
    /// Gets a page of revision information for the wiki page with the given title and namespace.
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
    /// A <see cref="PagedRevisionInfo"/> record with information for the requested wiki page; or
    /// <see langword="null"/> if no such page exists.
    /// </para>
    /// </returns>
    public static async Task<PagedRevisionInfo?> GetHistoryAsync(
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
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Determines the permission the user with the given ID has for the wiki page with the given
    /// title and namespace.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="id">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see cref="WikiPermission.None"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static async ValueTask<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        string? id = null,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user?.IsDeleted == true
            || user?.IsDisabled == true)
        {
            return WikiPermission.None;
        }
        if (user?.IsWikiAdmin == true)
        {
            return WikiPermission.All;
        }
        if (string.IsNullOrEmpty(title)
            && string.IsNullOrEmpty(wikiNamespace)
            && !string.Equals(wikiNamespace, options.DefaultNamespace))
        {
            return WikiPermission.None;
        }

        return await GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, title, wikiNamespace, domain);
    }

    /// <summary>
    /// Determines the permission the user with the given ID has for the given article.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="article">The wiki page.</param>
    /// <param name="id">
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
    public static async ValueTask<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        Article article,
        string? id = null)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user?.IsDeleted == true
            || user?.IsDisabled == true)
        {
            return WikiPermission.None;
        }
        if (user?.IsWikiAdmin == true)
        {
            return WikiPermission.All;
        }

        return await GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, article: article);
    }

    /// <summary>
    /// Determines the permission the given user has for the wiki page with the given title and
    /// namespace.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/> instance.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see cref="WikiPermission.None"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// A <see cref="WikiPermission"/> value, which may be a combination of flags indicating various
    /// permissions.
    /// </returns>
    public static ValueTask<WikiPermission> GetPermissionAsync(
        this IWikiUserManager userManager,
        WikiOptions options,
        IDataStore dataStore,
        IWikiGroupManager groupManager,
        IWikiUser user,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null)
    {
        if (user.IsDeleted
            || user.IsDisabled)
        {
            return ValueTask.FromResult(WikiPermission.None);
        }
        if (user.IsWikiAdmin)
        {
            return ValueTask.FromResult(WikiPermission.All);
        }
        if (string.IsNullOrEmpty(title)
            && string.IsNullOrEmpty(wikiNamespace)
            && !string.Equals(wikiNamespace, options.DefaultNamespace))
        {
            return ValueTask.FromResult(WikiPermission.None);
        }

        return GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, title, wikiNamespace, domain);
    }

    /// <summary>
    /// Determines the permission the given user has for the given article.
    /// </summary>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="article">The wiki page.</param>
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
        Article article,
        IWikiUser user)
    {
        if (user.IsDeleted
            || user.IsDisabled)
        {
            return ValueTask.FromResult(WikiPermission.None);
        }
        if (user.IsWikiAdmin)
        {
            return ValueTask.FromResult(WikiPermission.All);
        }

        return GetPermissionInnerAsync(user, options, dataStore, userManager, groupManager, article: article);
    }

    /// <summary>
    /// Gets a page of wiki items which fit one of the special conditions in the <see
    /// cref="SpecialListType"/> enumeration.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="request">A record with information about the request.</param>
    /// <returns>
    /// A <see cref="PagedListDTO{T}"/> with <see cref="LinkInfo"/> records for all the pages that
    /// satisfy the request.
    /// </returns>
    /// <remarks>
    /// This method should not be used for <see cref="SpecialListType.What_Links_Here"/>, which has
    /// its own call that takes a different type of request record (<see
    /// cref="GetWhatLinksHereAsync(IDataStore, WikiOptions, WhatLinksHereRequest)"/>). If this
    /// method is called with <see cref="SpecialListType.What_Links_Here"/> as the value of <see
    /// cref="SpecialListRequest.Type"/>, the result will always be empty.
    /// </remarks>
    public static async Task<PagedListDTO<LinkInfo>> GetSpecialListAsync(
        this IDataStore dataStore,
        WikiOptions options,
        SpecialListRequest request)
    {
        if (request.Type == SpecialListType.Missing_Pages)
        {
            return await GetMissingPagesAsync(dataStore, options, request);
        }
        else if (request.Type == SpecialListType.What_Links_Here)
        {
            return new(new PagedList<LinkInfo>(null, 1, request.PageSize, 0));
        }

        var items = await GetSpecialListInnerAsync(request, options, dataStore);
        return new(new PagedList<LinkInfo>(
            items.Select(x => new LinkInfo(
                    x.Title,
                    x.WikiNamespace,
                    x.Domain,
                    x is Category category ? category.ChildIds.Count : 0,
                    x is WikiFile file1 ? file1.FileSize : 0,
                    x is WikiFile file2 ? file2.FileType : null)),
            items.PageNumber,
            items.PageSize,
            items.TotalCount));
    }

    /// <summary>
    /// Gets the user page with the given group ID.
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
    /// A <see cref="UserPageInfo"/> which corresponds to the user ID given; or <see
    /// langword="null"/> if no such user exists.
    /// </para>
    /// <para>
    /// Note that a result is still returned if the user <em>page</em> does not exist, but the
    /// user itself does. In this case, the <see cref="UserPageInfo.Item"/> will be <see
    /// langword="null"/> but the record's other properties will be set appropriately.
    /// </para>
    /// </returns>
    public static async Task<UserPageInfo?> GetUserPageAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string userId,
        string? requestingUserId = null)
    {
        var articleUser = await userManager.FindByIdAsync(userId);
        articleUser ??= await userManager.FindByNameAsync(userId);
        if (articleUser is null)
        {
            return null;
        }

        var wikiItem = await GetWikiItemAsync(
            dataStore,
            options,
            articleUser.Id,
            options.UserNamespace,
            null,
            true);

        var requestingUser = await userManager.FindByIdAsync(requestingUserId);

        var permission = await GetPermissionInnerAsync(
            requestingUser,
            options,
            dataStore,
            userManager,
            groupManager,
            articleUser.Id,
            options.UserNamespace,
            null,
            wikiItem);

        if (wikiItem?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read))
        {
            return new(
                null,
                null,
                permission,
                null);
        }

        IWikiUser? user;
        if (requestingUser is not null
            && (requestingUser.IsWikiAdmin
            || string.Equals(requestingUser.Id, articleUser.Id)))
        {
            user = articleUser;
        }
        else if (articleUser.IsDeleted)
        {
            user = new WikiUser
            {
                Id = articleUser.Id,
                IsWikiAdmin = articleUser.IsWikiAdmin,
            };
        }
        else
        {
            user = new WikiUser
            {
                DisplayName = articleUser.DisplayName,
                Id = articleUser.Id,
                IsWikiAdmin = articleUser.IsWikiAdmin,
            };
        }

        List<WikiUserInfo>? groupInfo = null;
        if (articleUser.Groups is not null)
        {
            groupInfo = new List<WikiUserInfo>();
            foreach (var id in articleUser.Groups)
            {
                var group = await groupManager.FindByIdAsync(id);
                if (group is null
                    || string.IsNullOrEmpty(group.OwnerId))
                {
                    continue;
                }
                var groupPage = await Article.GetArticleAsync(
                    options,
                    dataStore,
                    id,
                    options.GroupNamespace,
                    null,
                    true);
                groupInfo.Add(new WikiUserInfo(
                    id,
                    group,
                    groupPage is not null));
            }
        }

        return new(
            groupInfo,
            wikiItem,
            permission,
            new WikiUserInfo(articleUser.Id, user, wikiItem is not null));
    }

    /// <summary>
    /// Gets a page of the wiki pages which link to the given title and namespace.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="request">A record with information about the request.</param>
    /// <returns>
    /// A <see cref="PagedListDTO{T}"/> with <see cref="LinkInfo"/> records for all the pages that
    /// link to the given item; or <see langword="null"/> if no <see
    /// cref="WhatLinksHereRequest.Title"/> is given for a namespace which is not <see
    /// cref="WikiOptions.DefaultNamespace"/>.
    /// </returns>
    public static async Task<PagedListDTO<LinkInfo>?> GetWhatLinksHereAsync(
        this IDataStore dataStore,
        WikiOptions options,
        WhatLinksHereRequest request)
    {
        if (string.IsNullOrEmpty(request.Title)
            && (string.IsNullOrEmpty(request.WikiNamespace)
            || string.Equals(request.WikiNamespace, options.DefaultNamespace, StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        var allReferences = new HashSet<string>();
        var references = await PageLinks.GetPageLinksAsync(
            dataStore,
            request.Title ?? options.MainPageTitle,
            request.WikiNamespace ?? options.DefaultNamespace,
            request.Domain);
        if (references is not null)
        {
            foreach (var reference in references.References)
            {
                allReferences.Add(reference);
            }
        }

        var transclusions = await PageTransclusions.GetPageTransclusionsAsync(
            dataStore,
            request.Title ?? options.MainPageTitle,
            request.WikiNamespace ?? options.DefaultNamespace,
            request.Domain);
        if (transclusions is not null)
        {
            foreach (var reference in transclusions.References)
            {
                allReferences.Add(reference);
            }
        }

        if (!string.Equals(request.WikiNamespace, options.CategoryNamespace, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(request.WikiNamespace, options.FileNamespace, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(request.WikiNamespace, options.UserNamespace, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(request.WikiNamespace, options.GroupNamespace, StringComparison.OrdinalIgnoreCase))
        {
            var redirects = await PageRedirects.GetPageRedirectsAsync(
                dataStore,
                request.Title ?? options.MainPageTitle,
                request.WikiNamespace ?? options.DefaultNamespace,
                request.Domain);
            if (redirects is not null)
            {
                foreach (var reference in redirects.References)
                {
                    allReferences.Add(reference);
                }
            }
        }

        var articles = new List<Article>();
        var hasFilter = !string.IsNullOrWhiteSpace(request.Filter);
        foreach (var reference in allReferences)
        {
            var article = await dataStore.GetItemAsync<Article>(reference);
            if (article is not null
                && !article.IsDeleted
                && (!hasFilter
                || article.Title.Contains(request.Filter!)))
            {
                articles.Add(article);
            }
        }
        if (string.Equals(request.Sort, "timestamp", StringComparison.OrdinalIgnoreCase))
        {
            articles.Sort((x, y) => request.Descending
                ? -x.TimestampTicks.CompareTo(y.TimestampTicks)
                : x.TimestampTicks.CompareTo(y.TimestampTicks));
        }
        else
        {
            articles.Sort((x, y) => request.Descending
                ? -x.Title.CompareTo(y.Title)
                : x.Title.CompareTo(y.Title));
        }

        return new(new PagedList<LinkInfo>(
            articles
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new LinkInfo(
                    x.Title,
                    x.WikiNamespace,
                    x.Domain,
                    x is Category category ? category.ChildIds.Count : 0,
                    x is WikiFile file1 ? file1.FileSize : 0,
                    x is WikiFile file2 ? file2.FileType : null)),
            request.PageNumber,
            request.PageSize,
            articles.Count));
    }

    /// <summary>
    /// Gets the wiki page with the given title and namespace.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will no be followed. The original matching item will be
    /// returned, potentially with a redirect as its content.
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given; or
    /// <see langword="null"/> if no such item exists.
    /// </returns>
    public static async Task<Article?> GetWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        bool noRedirect = false)
    {
        if (string.Equals(wikiNamespace, options.CategoryNamespace, StringComparison.OrdinalIgnoreCase))
        {
            return await Category.GetCategoryAsync(options, dataStore, title);
        }
        else if (string.Equals(wikiNamespace, options.FileNamespace, StringComparison.OrdinalIgnoreCase))
        {
            return await WikiFile.GetFileAsync(options, dataStore, title);
        }
        else
        {
            return await Article.GetArticleAsync(
                options,
                dataStore,
                title ?? options.MainPageTitle,
                wikiNamespace,
                domain,
                noRedirect);
        }
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="id">The ID of the wiki page.</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the given <paramref name="id"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string id,
        IWikiUser? user = null)
    {
        var wikiItem = await dataStore.GetItemAsync<Article>(id);

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            wikiItem?.Title,
            wikiItem?.WikiNamespace,
            wikiItem?.Domain,
            wikiItem);

        var html = wikiItem?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read)
            ? null
            : wikiItem.Html;

        IWikiUser? articleUser = null;
        if (wikiItem is not null
            && string.Equals(wikiItem.WikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(wikiItem.Title);
            articleUser ??= await userManager.FindByNameAsync(wikiItem.Title);
        }

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            false,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission);
    }

    /// <summary>
    /// Gets the wiki page with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="id">The ID of the wiki page.</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the given <paramref name="id"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string id,
        string? userId = null) => await GetWikiItemAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            id,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets the wiki page with the given title, namespace, and domain.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will no be followed. The original matching item will be
    /// returned, potentially with a redirect as its content.
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null,
        bool noRedirect = false)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, noRedirect);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, options.UserNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        var html = wikiItem?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read)
            ? null
            : wikiItem.Html;

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            false,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission);
    }

    /// <summary>
    /// Gets the wiki page with the given title, namespace, and domain.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will no be followed. The original matching item will be
    /// returned, potentially with a redirect as its content.
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null,
        bool noRedirect = false) => await GetWikiItemAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId),
            noRedirect);

    /// <summary>
    /// Gets the most recent revision of the wiki page with the given title and namespace at the
    /// specified <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">The time of the requested revision.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given, at the given <paramref name="time"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAtTimeAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset time,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, true);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, wikiNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        var html = wikiItem is null
            || !permission.HasFlag(WikiPermission.Read)
            ? null
            : await wikiItem.GetHtmlAsync(options, dataStore, time);

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            wikiItem is not null,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission & WikiPermission.Read);
    }

    /// <summary>
    /// Gets the most recent revision of the wiki page with the given title and namespace at the
    /// specified <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">The time of the requested revision.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given, at the given <paramref name="time"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAtTimeAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset time,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => await GetWikiItemAtTimeAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            time,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets the most recent revision of the wiki page with the given title and namespace at the
    /// specified <paramref name="timestamp"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// The number of ticks in the timestamp of the final revision.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given, at the given <paramref name="timestamp"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAtTimeAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long timestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null) => await GetWikiItemAtTimeAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(timestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            user);

    /// <summary>
    /// Gets the most recent revision of the wiki page with the given title and namespace at the
    /// specified <paramref name="timestamp"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// The number of ticks in the timestamp of the final revision.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// A <see cref="WikiItemInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given, at the given <paramref name="timestamp"/>.
    /// </returns>
    public static async Task<WikiItemInfo> GetWikiItemAtTimeAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long timestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => await GetWikiItemAtTimeAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(timestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="time"/> and the current
    /// version of the text.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">The time of the final revision.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static async Task<WikiItemInfo> GetWikiItemDiffWithCurrentAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset time,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, true);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, wikiNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        var html = wikiItem is null
            || !permission.HasFlag(WikiPermission.Read)
            ? null
            : await wikiItem.GetDiffWithCurrentHtmlAsync(options, dataStore, time);

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            wikiItem is not null,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission & WikiPermission.Read);
    }

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="time"/> and the current
    /// version of the text.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">The time of the final revision.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static async Task<WikiItemInfo> GetWikiItemDiffWithCurrentAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset time,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => await GetWikiItemDiffWithCurrentAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            time,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="timestamp"/> and the current
    /// version of the text.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// The number of ticks in the timestamp of the final revision.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static Task<WikiItemInfo> GetWikiItemDiffWithCurrentAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long timestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null) => GetWikiItemDiffWithCurrentAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(timestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            user);

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="timestamp"/> and the current
    /// version of the text.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// The number of ticks in the timestamp of the final revision.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static Task<WikiItemInfo> GetWikiItemDiffWithCurrentAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long timestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => GetWikiItemDiffWithCurrentAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(timestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            userId);

    /// <summary>
    /// Gets a diff between the text at two given times.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="firstTime">
    /// The first revision time to compare.
    /// </param>
    /// <param name="secondTime">
    /// The second revision time to compare.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values
    /// are swapped. In other words, the diff is always from an earlier version to a later version.
    /// </remarks>
    public static async Task<WikiItemInfo> GetWikiItemDiffAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset firstTime,
        DateTimeOffset secondTime,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, true);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, wikiNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        var html = wikiItem is null
            || !permission.HasFlag(WikiPermission.Read)
            ? null
            : await wikiItem.GetDiffWithOtherHtmlAsync(options, dataStore, firstTime, secondTime);

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            wikiItem is not null,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission & WikiPermission.Read);
    }

    /// <summary>
    /// Gets a diff between the text at two given times.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="firstTime">
    /// The first revision time to compare.
    /// </param>
    /// <param name="secondTime">
    /// The second revision time to compare.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values
    /// are swapped. In other words, the diff is always from an earlier version to a later version.
    /// </remarks>
    public static async Task<WikiItemInfo> GetWikiItemDiffAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset firstTime,
        DateTimeOffset secondTime,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => await GetWikiItemDiffAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            firstTime,
            secondTime,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets a diff between the text at two given times.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="firstTimestamp">
    /// The number of ticks in the timestamp of the first revision time to compare.
    /// </param>
    /// <param name="secondTimestamp">
    /// The number of ticks in the timestamp of the second revision time to compare.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTimestamp"/> is before <paramref name="firstTimestamp"/>, their
    /// values are swapped. In other words, the diff is always from an earlier version to a later
    /// version.
    /// </remarks>
    public static Task<WikiItemInfo> GetWikiItemDiffAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long firstTimestamp,
        long secondTimestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null) => GetWikiItemDiffAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(firstTimestamp, TimeSpan.Zero),
            new DateTimeOffset(secondTimestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            user);

    /// <summary>
    /// Gets a diff between the text at two given times.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="firstTimestamp">
    /// The number of ticks in the timestamp of the first revision time to compare.
    /// </param>
    /// <param name="secondTimestamp">
    /// The number of ticks in the timestamp of the second revision time to compare.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTimestamp"/> is before <paramref name="firstTimestamp"/>, their
    /// values are swapped. In other words, the diff is always from an earlier version to a later
    /// version.
    /// </remarks>
    public static Task<WikiItemInfo> GetWikiItemDiffAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long firstTimestamp,
        long secondTimestamp,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => GetWikiItemDiffAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            new DateTimeOffset(firstTimestamp, TimeSpan.Zero),
            new DateTimeOffset(secondTimestamp, TimeSpan.Zero),
            title,
            wikiNamespace,
            domain,
            userId);

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">
    /// <para>
    /// The time of the final revision.
    /// </para>
    /// <para>
    /// If omitted, the current time is used.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static async Task<WikiItemInfo> GetWikiItemDiffWithPreviousAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset? time = null,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, true);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, wikiNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        string? html = null;
        if (wikiItem is not null
            && permission.HasFlag(WikiPermission.Read))
        {
            html = time.HasValue
                ? await wikiItem.GetDiffHtmlAsync(options, dataStore, time.Value)
                : await wikiItem.GetDiffHtmlAsync(options, dataStore, DateTimeOffset.UtcNow);
        }

        return new(
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            html,
            wikiItem is not null,
            permission.HasFlag(WikiPermission.Read) ? wikiItem : null,
            permission & WikiPermission.Read);
    }

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="time">
    /// <para>
    /// The time of the final revision.
    /// </para>
    /// <para>
    /// If omitted, the current time is used.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static async Task<WikiItemInfo> GetWikiItemDiffWithPreviousAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        DateTimeOffset? time = null,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => await GetWikiItemDiffWithPreviousAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            time,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId));

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="timestamp"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// <para>
    /// The number of ticks in the timestamp of the final revision.
    /// </para>
    /// <para>
    /// If omitted, the current time is used.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static Task<WikiItemInfo> GetWikiItemDiffWithPreviousAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long? timestamp = null,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null) => GetWikiItemDiffWithPreviousAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            timestamp.HasValue ? new DateTimeOffset(timestamp.Value, TimeSpan.Zero) : null,
            title,
            wikiNamespace,
            domain,
            user);

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="timestamp"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="timestamp">
    /// <para>
    /// The number of ticks in the timestamp of the final revision.
    /// </para>
    /// <para>
    /// If omitted, the current time is used.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <returns>
    /// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/> which
    /// corresponds to the <paramref name="title"/> and <paramref name="wikiNamespace"/> given.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a
    /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
    /// order given.
    /// </exception>
    public static Task<WikiItemInfo> GetWikiItemDiffWithPreviousAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        long? timestamp = null,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null) => GetWikiItemDiffWithPreviousAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            timestamp.HasValue ? new DateTimeOffset(timestamp.Value, TimeSpan.Zero) : null,
            title,
            wikiNamespace,
            domain,
            userId);

    /// <summary>
    /// Gets the wiki page with the given title and namespace, with additional information suited to
    /// editing the item.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="user">
    /// <para>
    /// An <see cref="IWikiUser"/>.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will no be followed. The original matching item will be
    /// returned, potentially with a redirect as its content.
    /// </param>
    /// <returns>
    /// A <see cref="WikiEditInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given.
    /// </returns>
    public static async Task<WikiEditInfo> GetWikiItemForEditingAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        IWikiUser? user = null,
        bool noRedirect = false)
    {
        IWikiUser? articleUser = null;
        if (string.Equals(wikiNamespace, options.UserNamespace))
        {
            articleUser = await userManager.FindByIdAsync(title);
            articleUser ??= await userManager.FindByNameAsync(title);
        }

        var wikiItem = await GetWikiItemAsync(dataStore, options, title, wikiNamespace, domain, noRedirect);
        if (wikiItem is null
            && articleUser is not null
            && !string.Equals(title, articleUser.Id))
        {
            wikiItem = await GetWikiItemAsync(dataStore, options, articleUser.Id, options.UserNamespace, null, true);
        }

        var permission = await GetPermissionInnerAsync(
            user,
            options,
            dataStore,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            wikiItem);

        if (wikiItem?.IsDeleted != false
            || !permission.HasFlag(WikiPermission.Read))
        {
            return new(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                permission);
        }

        var owner = string.IsNullOrEmpty(wikiItem?.Owner)
            ? null
            : await userManager.FindByIdAsync(wikiItem.Owner);
        IWikiUser? ownerUser = null;
        var ownerPageExists = false;
        if (owner is not null)
        {
            ownerPageExists = !owner.IsDeleted
                && await Article.GetArticleAsync(
                    options,
                    dataStore,
                    owner.Id,
                    options.UserNamespace,
                    null,
                    true) is not null;

            if (user is not null
                && (user.IsWikiAdmin
                || string.Equals(user.Id, owner.Id)))
            {
                ownerUser = owner;
            }
            else if (owner.IsDeleted)
            {
                ownerUser = new WikiUser
                {
                    Id = owner.Id,
                    IsWikiAdmin = owner.IsWikiAdmin,
                };
            }
            else
            {
                ownerUser = new WikiUser
                {
                    DisplayName = owner.DisplayName,
                    Id = owner.Id,
                    IsWikiAdmin = owner.IsWikiAdmin,
                };
            }
        }
        WikiUserInfo? ownerInfo = null;
        if (ownerUser is null)
        {
            if (!string.IsNullOrEmpty(wikiItem?.Owner))
            {
                ownerInfo = new WikiUserInfo(wikiItem.Owner, null, false);
            }
        }
        else
        {
            ownerInfo = new WikiUserInfo(ownerUser.Id, ownerUser, ownerPageExists);
        }

        List<WikiUserInfo>? allowedEditors = null;
        if (wikiItem?.AllowedEditors is not null)
        {
            foreach (var id in wikiItem.AllowedEditors)
            {
                allowedEditors ??= new();

                var editorUser = await userManager.FindByIdAsync(id);
                IWikiUser? userItem = null;
                var userPageExists = false;
                if (editorUser is not null)
                {
                    userPageExists = !editorUser.IsDeleted
                        && await Article.GetArticleAsync(
                            options,
                            dataStore,
                            editorUser.Id,
                            options.UserNamespace,
                            null,
                            true) is not null;

                    if (user is not null
                        && (user.IsWikiAdmin
                        || string.Equals(user.Id, editorUser.Id)))
                    {
                        userItem = editorUser;
                    }
                    else if (editorUser.IsDeleted)
                    {
                        userItem = new WikiUser
                        {
                            Id = editorUser.Id,
                            IsWikiAdmin = editorUser.IsWikiAdmin,
                        };
                    }
                    else
                    {
                        userItem = new WikiUser
                        {
                            DisplayName = editorUser.DisplayName,
                            Id = editorUser.Id,
                            IsWikiAdmin = editorUser.IsWikiAdmin,
                        };
                    }
                }
                if (userItem is null)
                {
                    allowedEditors.Add(new(id, null, false));
                }
                else
                {
                    allowedEditors.Add(new(userItem.Id, userItem, userPageExists));
                }
            }
        }

        List<WikiUserInfo>? allowedViewers = null;
        if (wikiItem?.AllowedViewers is not null)
        {
            foreach (var id in wikiItem.AllowedViewers)
            {
                allowedViewers ??= new();

                var viewerUser = await userManager.FindByIdAsync(id);
                IWikiUser? userItem = null;
                var userPageExists = false;
                if (viewerUser is not null)
                {
                    userPageExists = !viewerUser.IsDeleted
                        && await Article.GetArticleAsync(
                            options,
                            dataStore,
                            viewerUser.Id,
                            options.UserNamespace,
                            null,
                            true) is not null;

                    if (user is not null
                        && (user.IsWikiAdmin
                        || string.Equals(user.Id, viewerUser.Id)))
                    {
                        userItem = viewerUser;
                    }
                    else if (viewerUser.IsDeleted)
                    {
                        userItem = new WikiUser
                        {
                            Id = viewerUser.Id,
                            IsWikiAdmin = viewerUser.IsWikiAdmin,
                        };
                    }
                    else
                    {
                        userItem = new WikiUser
                        {
                            DisplayName = viewerUser.DisplayName,
                            Id = viewerUser.Id,
                            IsWikiAdmin = viewerUser.IsWikiAdmin,
                        };
                    }
                }
                if (userItem is null)
                {
                    allowedViewers.Add(new(id, null, false));
                }
                else
                {
                    allowedViewers.Add(new(userItem.Id, userItem, userPageExists));
                }
            }
        }

        List<WikiUserInfo>? allowedEditorGroups = null;
        if (wikiItem?.AllowedEditorGroups is not null)
        {
            foreach (var id in wikiItem.AllowedEditorGroups)
            {
                allowedEditorGroups ??= new();

                var group = await groupManager.FindByIdAsync(id);
                IWikiGroup? groupItem = null;
                var groupPageExists = false;
                if (group is not null)
                {
                    groupPageExists = await Article.GetArticleAsync(
                        options,
                        dataStore,
                        group.Id,
                        options.GroupNamespace,
                        null,
                        true) is not null;

                    if (user is not null
                        && (user.IsWikiAdmin
                        || user.Groups?.Contains(group.Id) == true))
                    {
                        groupItem = group;
                    }
                    else
                    {
                        groupItem = new WikiGroup
                        {
                            DisplayName = group.DisplayName,
                            Id = group.Id,
                        };
                    }
                }

                if (groupItem is null)
                {
                    allowedEditorGroups.Add(new(id, null, false));
                }
                else
                {
                    allowedEditorGroups.Add(new(groupItem.Id, groupItem, groupPageExists));
                }
            }
        }

        List<WikiUserInfo>? allowedViewerGroups = null;
        if (wikiItem?.AllowedViewerGroups is not null)
        {
            foreach (var id in wikiItem.AllowedViewerGroups)
            {
                allowedViewerGroups ??= new();

                var group = await groupManager.FindByIdAsync(id);
                IWikiGroup? groupItem = null;
                var groupPageExists = false;
                if (group is not null)
                {
                    groupPageExists = await Article.GetArticleAsync(
                        options,
                        dataStore,
                        group.Id,
                        options.GroupNamespace,
                        null,
                        true) is not null;

                    if (user is not null
                        && (user.IsWikiAdmin
                        || user.Groups?.Contains(group.Id) == true))
                    {
                        groupItem = group;
                    }
                    else
                    {
                        groupItem = new WikiGroup
                        {
                            DisplayName = group.DisplayName,
                            Id = group.Id,
                        };
                    }
                }

                if (groupItem is null)
                {
                    allowedViewerGroups.Add(new(id, null, false));
                }
                else
                {
                    allowedViewerGroups.Add(new(groupItem.Id, groupItem, groupPageExists));
                }
            }
        }

        return new(
            allowedEditors,
            allowedEditorGroups,
            allowedViewers,
            allowedViewerGroups,
            articleUser?.DisplayName ?? wikiItem?.Title ?? title ?? (wikiItem is null ? null : options.MainPageTitle),
            wikiItem,
            ownerInfo,
            permission);
    }

    /// <summary>
    /// Gets the wiki page with the given title and namespace, with additional information suited to
    /// editing the item.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
    /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
    /// <param name="title">
    /// <para>
    /// The title of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted if the <paramref name="wikiNamespace"/> is also omitted, or equal to the <see
    /// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
    /// will be used.
    /// </para>
    /// <para>
    /// If this parameter is <see langword="null"/> or empty, but <paramref name="wikiNamespace"/>
    /// is <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>,
    /// the result will always be <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the wiki page.
    /// </para>
    /// <para>
    /// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <param name="userId">
    /// <para>
    /// The <see cref="IWikiOwner.Id"/> of a wiki user.
    /// </para>
    /// <para>
    /// May be <see langword="null"/>, in which case permission is determined for an anonymous user.
    /// </para>
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/> redirects will no be followed. The original matching item will be
    /// returned, potentially with a redirect as its content.
    /// </param>
    /// <returns>
    /// A <see cref="WikiEditInfo"/> which corresponds to the <paramref name="title"/> and <paramref
    /// name="wikiNamespace"/> given.
    /// </returns>
    public static async Task<WikiEditInfo> GetWikiItemForEditingAsync(
        this IDataStore dataStore,
        WikiOptions options,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        string? userId = null,
        bool noRedirect = false) => await GetWikiItemForEditingAsync(
            dataStore,
            options,
            userManager,
            groupManager,
            title,
            wikiNamespace,
            domain,
            await userManager.FindByIdAsync(userId),
            noRedirect);

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

    private static async Task<IPagedList<T>> GetListAsync<T>(
        IDataStore dataStore,
        int pageNumber = 0,
        int pageSize = 50,
        string? sort = null,
        bool descending = false,
        string? filter = null,
        Expression<Func<T, bool>>? condition = null) where T : Article
    {
        var pageCondition = condition is null
            ? (T x) => !x.IsDeleted
            : condition.AndAlso(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(filter))
        {
            pageCondition = pageCondition.AndAlso(x => x.Title.Contains(filter));
        }

        var query = dataStore.Query<T>();
        if (pageCondition is not null)
        {
            query = query.Where(pageCondition);
        }
        if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
        {
            query = query.OrderBy(x => x.TimestampTicks, descending: descending);
        }
        else
        {
            query = query.OrderBy(x => x.Title, descending: descending);
        }

        return await query.GetPageAsync(pageNumber, pageSize);
    }

    private static async Task<PagedListDTO<LinkInfo>> GetMissingPagesAsync(
        IDataStore dataStore,
        WikiOptions options,
        SpecialListRequest request)
    {
        var query = dataStore.Query<MissingPage>()
            .Where(x => x.WikiNamespace != options.UserNamespace && x.WikiNamespace != options.GroupNamespace);
        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x => x.Id
                .Substring(0, x.Id.Length - 8)
                .Contains(request.Filter));
        }

        var results = await query
            .OrderBy(x => x.Title, descending: request.Descending)
            .GetPageAsync(request.PageNumber, request.PageSize);

        return new(new PagedList<LinkInfo>(
            results.Select(x => new LinkInfo(
                x.Title,
                x.WikiNamespace,
                x.Domain,
                0,
                0,
                null)),
            results.PageNumber,
            results.PageSize,
            results.TotalCount));
    }

    private static async ValueTask<WikiPermission> GetPermissionInnerAsync(
        IWikiUser? user,
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        string? title = null,
        string? wikiNamespace = null,
        string? domain = null,
        Article? article = null)
    {
        title ??= options.MainPageTitle;
        article ??= await Article.GetArticleAsync(
            options,
            dataStore,
            title,
            wikiNamespace,
            domain,
            true);
        if (article is not null)
        {
            title = article.Title;
            wikiNamespace = article.WikiNamespace;
            domain = article.Domain;
        }

        var isUserPage = string.Equals(
                wikiNamespace,
                options.UserNamespace,
                StringComparison.OrdinalIgnoreCase);
        if (isUserPage
            && user is not null
            && string.Equals(user.Id, title))
        {
            return WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
        }

        var isGroupPage = string.Equals(
                wikiNamespace,
                options.GroupNamespace,
                StringComparison.OrdinalIgnoreCase);
        if (isGroupPage && user is not null)
        {
            var ownerId = await groupManager.GetGroupOwnerIdAsync(title);
            if (string.Equals(user.Id, ownerId))
            {
                return WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
            }
        }

        var defaultPermission = WikiPermission.None;
        var isDomain = !string.IsNullOrEmpty(domain);
        if (!isDomain)
        {
            defaultPermission = user is null
                ? options.DefaultAnonymousPermission
                : options.DefaultRegisteredPermission;
        }

        if (isDomain
            && user is not null
            && !string.IsNullOrEmpty(domain))
        {
            if (options.GetDomainPermission is not null)
            {
                defaultPermission = await options.GetDomainPermission(user.Id, domain);
            }
            if (user.AllowedViewDomains?.Contains(domain) == true)
            {
                defaultPermission |= WikiPermission.Read;
            }
        }

        if (article is null)
        {
            if (isUserPage)
            {
                return defaultPermission & WikiPermission.Read;
            }
            else if (isGroupPage)
            {
                return user?.Groups?.Contains(title) == true
                    ? WikiPermission.ReadWrite
                    : defaultPermission & WikiPermission.Read;
            }
            return defaultPermission;
        }

        if (user is null)
        {
            return article.AllowedViewers is null
                ? defaultPermission & WikiPermission.Read
                : WikiPermission.None;
        }
        var isReservedNamespace = options
            .ReservedNamespaces
            .Any(x => string.Equals(
                x,
                article.WikiNamespace,
                StringComparison.OrdinalIgnoreCase));

        IWikiUser? owner = null;
        if (!string.IsNullOrEmpty(article.Owner))
        {
            owner = await userManager.FindByIdAsync(article.Owner);

            if (owner is not null
                && string.Equals(user.Id, owner.Id))
            {
                return isReservedNamespace
                    ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions | WikiPermission.SetOwner
                    : WikiPermission.All;
            }

            if (!isGroupPage && user.Groups?.Contains(article.Owner) == true)
            {
                return isReservedNamespace
                    ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions | WikiPermission.SetOwner
                    : WikiPermission.All;
            }
        }

        var isAdminNamespace = options
            .AdminNamespaces
            .Any(x => string.Equals(
                x,
                article.WikiNamespace,
                StringComparison.OrdinalIgnoreCase));
        if (isAdminNamespace
            && user.IsWikiAdmin)
        {
            return isReservedNamespace
                ? WikiPermission.ReadWrite | WikiPermission.Delete | WikiPermission.SetPermissions
                : WikiPermission.ReadWrite | WikiPermission.Create | WikiPermission.Delete | WikiPermission.SetPermissions;
        }

        if (!isUserPage
            && !isGroupPage
            && string.IsNullOrEmpty(article.Owner))
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

        List<IWikiGroup>? userGroups = null;
        if (article.AllowedViewers is not null
            && !article.AllowedViewers.Contains(user.Id)
            && user.AllowedViewArticles?.Contains(article.Id) != true
            && (!isGroupPage
                || user.Groups?.Contains(title) != true)
            && (article.AllowedViewerGroups is null
                || user.Groups is null
                || !article.AllowedViewerGroups.Intersect(user.Groups).Any()))
        {
            userGroups = new();
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
            if (!userGroups.Any(x => x.AllowedViewArticles?.Contains(article.Id) == true))
            {
                return WikiPermission.None;
            }
        }

        if (isUserPage)
        {
            return defaultPermission & WikiPermission.Read;
        }

        if (isGroupPage)
        {
            return user.Groups?.Contains(title) == true
                ? WikiPermission.ReadWrite
                : defaultPermission & WikiPermission.Read;
        }

        var writePermission = article.AllowedEditors is null
            || article.AllowedEditors.Contains(user.Id)
            || user.AllowedEditArticles?.Contains(article.Id) == true
            || (article.AllowedEditorGroups is not null
                && user.Groups is not null
                && article.AllowedEditorGroups.Intersect(user.Groups).Any());
        if (!writePermission)
        {
            if (userGroups is null)
            {
                userGroups = new();
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
            if (userGroups.Any(x => x.AllowedEditArticles?.Contains(article.Id) == true))
            {
                writePermission = true;
            }
        }

        return writePermission
            ? WikiPermission.ReadWrite
            : defaultPermission & WikiPermission.Read;
    }

    private static async Task<IPagedList<Article>> GetSpecialListInnerAsync(
        SpecialListRequest request,
        WikiOptions options,
        IDataStore dataStore) => request.Type switch
        {
            SpecialListType.All_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter),

            SpecialListType.All_Files => await GetListAsync<WikiFile>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter),

            SpecialListType.All_Pages => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IdItemTypeName == Article.ArticleIdItemTypeName)
        ,

#pragma warning disable RCS1113 // Use 'string.IsNullOrEmpty' method: not necessarily supported by all data providers
            SpecialListType.All_Redirects => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.RedirectTitle != null && x.RedirectTitle != string.Empty)
        ,
#pragma warning restore RCS1113 // Use 'string.IsNullOrEmpty' method.

            SpecialListType.Broken_Redirects => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IsBrokenRedirect),

            SpecialListType.Double_Redirects => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IsDoubleRedirect),

            SpecialListType.Uncategorized_Articles => await GetListAsync<Article>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.IdItemTypeName == Article.ArticleIdItemTypeName
                    && x.RedirectTitle == null
                    && x.WikiNamespace != options.ScriptNamespace
                    && (x.Categories == null || x.Categories.Count() == 0))
        ,

            SpecialListType.Uncategorized_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.Categories == null || x.Categories.Count() == 0)
        ,

            SpecialListType.Uncategorized_Files => await GetListAsync<WikiFile>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.Categories == null || x.Categories.Count() == 0)
        ,

            SpecialListType.Unused_Categories => await GetListAsync<Category>(
                dataStore,
                request.PageNumber,
                request.PageSize,
                request.Sort,
                request.Descending,
                request.Filter,
                x => x.ChildIds.Count() == 0)
        ,
            _ => new PagedList<Article>(null, 1, request.PageSize, 0),
        };
}
