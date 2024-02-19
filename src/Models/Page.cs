using AngleSharp.Dom;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.DiffPatchMerge;
using Tavenem.Wiki.MarkdownExtensions.TableOfContents;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A common base class for wiki pages.
/// </summary>
[JsonDerivedType(typeof(Article), Article.ArticleIdItemTypeName)]
[JsonDerivedType(typeof(Category), Category.CategoryIdItemTypeName)]
[JsonDerivedType(typeof(WikiFile), WikiFile.WikiFileIdItemTypeName)]
public abstract class Page : MarkdownItem, IPage<Page>
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string PageIdItemTypeName = ":Page:";

    /// <summary>
    /// A list of the <see cref="IWikiOwner.Id"/> values of groups allowed to edit this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This list complements <see cref="AllowedEditors"/>. A user may edit a page if they are a
    /// member of a group in this list, <em>or</em> if they have permission to edit the page
    /// according to the rules determined by <see cref="AllowedEditors"/>.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot edit a page is still able to see that a page by that title exists (to
    /// avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedEditPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to edit them, and users may also have a
    /// separate list of articles which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit a page, if either the page indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the page, then
    /// permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<string>? AllowedEditorGroups { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiGroup"/>s allowed to edit this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This list complements <see cref="AllowedEditors"/>. A user may edit a page if they are a
    /// member of a group in this list, <em>or</em> if they have permission to edit the page
    /// according to the rules determined by <see cref="AllowedEditors"/>.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot edit a page is still able to see that a page by that title exists (to
    /// avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedEditPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to edit them, and users may also have a
    /// separate list of articles which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit a page, if either the page indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the page, then
    /// permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// <para>
    /// Note: this property is not persisted. It is dynamically built when the page is retrieved for
    /// editing.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<IWikiGroup>? AllowedEditorGroupObjects { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiOwner.Id"/> values of users allowed to edit this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone, including anonymous users.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to edit the page.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot edit a page is still able to see that a page by that title exists
    /// (to avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedEditPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to edit them, and users may also have
    /// a separate list of articles which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit a page, if either the page indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<string>? AllowedEditors { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiUser"/>s allowed to edit this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone, including anonymous users.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to edit the page.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot edit a page is still able to see that a page by that title exists
    /// (to avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedEditPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to edit them, and users may also have
    /// a separate list of articles which they may edit.
    /// </para>
    /// <para>
    /// When a user attempts to edit a page, if either the page indicates that the editor has
    /// permission to edit it, or the user indicates that it has permission to edit the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// <para>
    /// Note: this property is not persisted. It is dynamically built when the page is retrieved for
    /// editing.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<IWikiUser>? AllowedEditorObjects { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiOwner.Id"/> values of groups allowed to view this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This list complements <see cref="AllowedViewers"/>. A user may view a page if they are a
    /// member of a group in this list, <em>or</em> if they have permission to view the page
    /// according to the rules determined by <see cref="AllowedViewers"/>.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedViewPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list groups with permission to view them, and groups may also
    /// have a separate list of articles which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view a page, if either the page indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<string>? AllowedViewerGroups { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiGroup"/>s allowed to view this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This list complements <see cref="AllowedViewers"/>. A user may view a page if they are a
    /// member of a group in this list, <em>or</em> if they have permission to view the page
    /// according to the rules determined by <see cref="AllowedViewers"/>.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedViewPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list groups with permission to view them, and groups may also
    /// have a separate list of articles which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view a page, if either the page indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// <para>
    /// Note: this property is not persisted. It is dynamically built when the page is retrieved for
    /// editing.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<IWikiGroup>? AllowedViewerGroupObjects { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiOwner.Id"/> values of users allowed to view this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone, including anonymous users.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot view a page is still able to see that a page by that title exists
    /// (to avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedViewPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to view them, and users may also have
    /// a separate list of articles which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view a page, if either the page indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<string>? AllowedViewers { get; set; }

    /// <summary>
    /// A list of the <see cref="IWikiUser"/>s allowed to view this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone, including anonymous users.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// <para>
    /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
    /// </para>
    /// <para>
    /// A user who cannot view a page is still able to see that a page by that title exists
    /// (to avoid confusion about creating a new page with that title).
    /// </para>
    /// <para>
    /// Note that this list is not intended to have duplicate information for the <see
    /// cref="IWikiOwner.AllowedViewPages"/> list. Rather, the two lists are expected to be
    /// complementary. Pages may list users with permission to view them, and users may also have
    /// a separate list of articles which they may view.
    /// </para>
    /// <para>
    /// When a user attempts to view a page, if either the page indicates that the viewer has
    /// permission to view it, or the user indicates that it has permission to view the page,
    /// then permission is granted.
    /// </para>
    /// <para>
    /// A particular implementation of <c>Tavenem.Wiki</c> may use only one of these systems, or
    /// both, depending on the best fit for the implementation's access control use case.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// <para>
    /// Note: this property is not persisted. It is dynamically built when the page is retrieved for
    /// editing.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<IWikiUser>? AllowedViewerObjects { get; set; }

    /// <summary>
    /// Whether this page can be renamed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Defaults to <see langword="true"/>, and is read-only. Intended for use by inheriting classes
    /// which use special values for titles (and therefore should not be renamed by users).
    /// </para>
    /// <para>
    /// Not persisted to storage.
    /// </para>
    /// </remarks>
    [JsonIgnore]
    public virtual bool CanRename => true;

    /// <summary>
    /// The categories to which this page belongs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is a cache that is filled automatically when the page is rendered. It
    /// may be inaccurate during the page creation and/or revision process prior to rendering.
    /// </para>
    /// <para>
    /// Updates to this list which are not due to changes in page content (e.g. due to changes in
    /// transcluded pages) do not count as revisions.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? Categories { get; set; }

    /// <summary>
    /// <para>
    /// The HTML content to display.
    /// </para>
    /// <para>
    /// This is either <see cref="RevisionHtml"/> (if set) or <see cref="MarkdownItem.Html"/>.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public string DisplayHtml => RevisionHtml ?? Html;

    /// <summary>
    /// An optional display title for this page, which would override its <see cref="Title"/>'s <see
    /// cref="PageTitle.Title"/> for display purposes.
    /// </summary>
    public virtual string? DisplayTitle { get; set; }

    /// <summary>
    /// Whether this page exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A page exists if its most recent revision is not marked as deleted. It must have at least
    /// one revision for this to be true.
    /// </para>
    /// <para>
    /// Pages which do not exist include those which have not yet had content added, as well as
    /// those which have been explicitly deleted.
    /// </para>
    /// <para>
    /// Categories, user pages, and group pages are always considered to exist implicitly. They are
    /// enumerations of their members (even when they have no current members), even if they have no
    /// other content.
    /// </para>
    /// <para>
    /// Note that a page which redirects to another page does <em>not</em> exist. A revision which
    /// makes a page a redirect will also mark it as deleted.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but setting it has no effect.
    /// </para>
    /// </remarks>
    public virtual bool Exists
    {
        get => Revision?.IsDeleted == false;
        set { }
    }

    /// <summary>
    /// The headings on this wiki page, as defined by the table of contents options set for the
    /// wiki, and within the markup of the page.
    /// </summary>
    public IReadOnlyCollection<Heading>? Headings { get; set; }

    /// <summary>
    /// <para>
    /// Indicates whether this is a redirect to a page which does not exist.
    /// </para>
    /// <para>
    /// Updates to this property do not constitute a revision.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public bool IsBrokenRedirect { get; set; }

    /// <summary>
    /// Whether this represents a diff between two versions of the page.
    /// </summary>
    public bool IsDiff { get; set; }

    /// <summary>
    /// <para>
    /// Indicates whether this is a redirect to a page which is also a redirect.
    /// </para>
    /// <para>
    /// Updates to this property do not constitute a revision.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public bool IsDoubleRedirect { get; set; }

    /// <summary>
    /// Whether this page is considered missing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A missing page is one to which at least one other page has a link, but which either has no
    /// revisions (i.e. has not yet been created), or has been explicitly deleted but is not a
    /// redirect.
    /// </para>
    /// <para>
    /// Categories, user pages, and group pages are never missing; they are considered to exist
    /// implicitly as enumerations of their members, even if they have no content.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public bool IsMissing { get; set; }

    /// <summary>
    /// The ID of the owner of this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Should be the <see cref="IWikiOwner.Id"/> of a user or group, or <see langword="null"/>.
    /// </para>
    /// <para>
    /// Only the owner of an page (if one has been set) may delete, move, or rename it,
    /// regardless of who else may have been granted edit permission.
    /// </para>
    /// <para>
    /// An page may only be assigned allowed editors and viewers if it is owned, and only the
    /// owner may manage those permissions.
    /// </para>
    /// <para>
    /// Only the current owner of an page may change its ownership.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public string? Owner { get; set; }

    /// <summary>
    /// The <see cref="IWikiOwner"/> associated with this page.
    /// </summary>
    public IWikiOwner? OwnerObject { get; set; }

    /// <summary>
    /// The permission that the current user has to access this page.
    /// </summary>
    public WikiPermission Permission { get; set; }

    /// <summary>
    /// Other pages which redirect to this one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Updates to this list (due to changes in redirecting pages) do not count as revisions.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? RedirectReferences { get; set; }

    /// <summary>
    /// If this is a redirect, contains the title of the destination.
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public PageTitle? RedirectTitle { get; set; }

    /// <summary>
    /// <para>
    /// Other pages which link to this one.
    /// </para>
    /// <para>
    /// Does not include category listings or links from <see cref="Message"/>s.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Updates to this list (due to changes in linking pages) do not count as revisions.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? References { get; set; }

    /// <summary>
    /// The latest revision of this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this is <see langword="null"/> the page has never been edited. This can be the case for
    /// pages which do not exist, but also for automatically generated pages such as categories
    /// which do not (yet) have manually-added content.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public Revision? Revision { get; set; }

    /// <summary>
    /// The HTML of the requested revision at time of retrieval.
    /// </summary>
    public string? RevisionHtml { get; set; }

    /// <summary>
    /// The timestamp of this revision, in UTC.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset Timestamp => new(Revision?.TimestampTicks ?? 0, TimeSpan.Zero);

    /// <summary>
    /// The title of this page.
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public PageTitle Title { get; set; }

    /// <summary>
    /// <para>
    /// Other pages which transclude this one.
    /// </para>
    /// <para>
    /// Does not include transclusions in discussion messages.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? TransclusionReferences { get; set; }

    /// <summary>
    /// The transclusions within this page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this list is a cache that is filled automatically when the page is rendered. It
    /// may be inaccurate during the page creation and/or revision process prior to rendering.
    /// </para>
    /// <para>
    /// Updates to this list which are not due to changes in page content (e.g. due to changes in
    /// transcluded pages) do not count as revisions.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? Transclusions { get; set; }

    /// <summary>
    /// Whether this page is uncategorized. Read-only.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A redirect does not count as uncategorized.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but setting it has no effect.
    /// </para>
    /// </remarks>
    public bool Uncategorized
    {
        get => !RedirectTitle.HasValue
            && (Categories is null
            || Categories.Count == 0);
        set { } // empty setter to allow source-generated (de)serialization
    }

    /// <summary>
    /// Constructs a new instance of <see cref="Page"/>.
    /// </summary>
    protected Page(
        PageTitle title,
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        string? markdownContent = null,
        string? owner = null,
        Revision? revision = null,
        IReadOnlyCollection<string>? allowedEditors = null,
        IReadOnlyCollection<string>? allowedViewers = null,
        IReadOnlyCollection<string>? allowedEditorGroups = null,
        IReadOnlyCollection<string>? allowedViewerGroups = null,
        IReadOnlyCollection<PageTitle>? categories = null,
        IReadOnlyCollection<Heading>? headings = null,
        IReadOnlyCollection<PageTitle>? redirectReferences = null,
        IReadOnlyCollection<PageTitle>? references = null,
        IReadOnlyCollection<PageTitle>? transclusionReferences = null,
        IReadOnlyCollection<PageTitle>? transclusions = null,
        bool isBrokenRedirect = false,
        bool isDoubleRedirect = false,
        PageTitle? redirectTitle = null) : base(IPage<Page>.GetId(title), markdownContent, html, preview, wikiLinks)
    {
        if (!string.IsNullOrEmpty(owner))
        {
            AllowedEditors = allowedEditors;
            AllowedViewers = allowedViewers;
            AllowedEditorGroups = allowedEditorGroups;
            AllowedViewerGroups = allowedViewerGroups;
        }
        Categories = categories;
        Headings = headings;
        IsBrokenRedirect = isBrokenRedirect;
        IsDoubleRedirect = isDoubleRedirect;
        Owner = owner;
        RedirectReferences = redirectReferences;
        RedirectTitle = redirectTitle;
        References = references;
        Revision = revision;
        Title = title;
        TransclusionReferences = transclusionReferences;
        Transclusions = transclusions;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="Page"/>.
    /// </summary>
    protected Page(PageTitle title) : this(
        title,
        string.Empty,
        string.Empty,
        new List<WikiLink>().AsReadOnly())
    { }

    /// <summary>
    /// <para>
    /// Always throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// <para>
    /// <see cref="Page"/> is an abstract type, and cannot create an instance of itself.
    /// </para>
    /// <para>
    /// This method exists only to fulfill the implementation requirements of <see
    /// cref="IPage{TSelf}"/>.
    /// </para>
    /// <para>
    /// You should only call this method on a subclass, never on <see cref="Page"/> itself.
    /// </para>
    /// </summary>
    /// <returns>
    /// Never returns. Always throws an <see cref="InvalidOperationException"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Always thrown.
    /// </exception>
    [DoesNotReturn]
    public static Page Empty(PageTitle title)
        => throw new InvalidOperationException();

    /// <summary>
    /// <para>
    /// Always throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// <para>
    /// <see cref="Page"/> is an abstract type, and cannot create an instance of itself.
    /// </para>
    /// <para>
    /// This method exists only to fulfill the implementation requirements of <see
    /// cref="IPage{TSelf}"/>.
    /// </para>
    /// <para>
    /// You should only call this method on a subclass, never on <see cref="Page"/> itself.
    /// </para>
    /// </summary>
    /// <returns>
    /// Never returns. Always throws an <see cref="InvalidOperationException"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Always thrown.
    /// </exception>
    public virtual Page Copy(string? newNamespace = null)
        => throw new InvalidOperationException();

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <param name="format">
    /// <para>
    /// The format used.
    /// </para>
    /// <para>
    /// Can be either "delta" (the default), "gnu", or "md" (case insensitive).
    /// </para>
    /// <para>
    /// The "delta" format (the default, used if an empty string or whitespace is passed) renders a
    /// compact, encoded string which describes each diff operation. The first character is '=' for
    /// unchanged text, '+' for an insertion, and '-' for deletion. Unchanged text and deletions are
    /// followed by their length only; insertions are followed by a compressed version of their full
    /// text. Each diff is separated by a tab character ('\t').
    /// </para>
    /// <para>
    /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
    /// nothing if the text was unchanged. Each diff is separated by a newline.
    /// </para>
    /// <para>
    /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
    /// nothing if the text was unchanged. Diffs are concatenated without separators.
    /// </para>
    /// </param>
    /// <returns>
    /// A string representing the final revision at the given time, as rendered HTML.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    /// <remarks>
    /// The <paramref name="time"/> does not need to be exact. All revisions up to and including the
    /// given time will be included.
    /// </remarks>
    public async Task<string> GetDiffAsync(
        IDataStore dataStore,
        DateTimeOffset time,
        string format = "md")
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        if (revisions.Count == 0)
        {
            return string.Empty;
        }
        return Revision.GetDiff(revisions, format);
    }

    /// <summary>
    /// Gets a diff between the <see cref="MarkdownItem.MarkdownContent"/> of this item and the
    /// given one, as rendered HTML.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="other">The other <see cref="MarkdownItem"/> instance.</param>
    /// <returns>
    /// A string representing the diff between this instance and the <paramref name="other"/>
    /// instance, as rendered HTML.
    /// </returns>
    public override async ValueTask<string> GetDiffHtmlAsync(
        WikiOptions options,
        IDataStore dataStore,
        MarkdownItem other) => RenderHtml(
            options,
            dataStore,
            await PostprocessMarkdownAsync(options, dataStore, GetDiff(other, "html")),
            Title);

    /// <summary>
    /// Gets a diff which represents the final revision at the given <paramref name="time"/>, as
    /// rendered HTML.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <returns>
    /// <para>
    /// A string representing the final revision at the given time, as rendered HTML.
    /// </para>
    /// <para>
    /// This format renders the text surrounded by a span with class "diff-deleted" for deletion,
    /// "diff-inserted" for addition, or without a wrapping span if the text was unchanged. Diffs
    /// are concatenated without separators.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    /// <remarks>
    /// The <paramref name="time"/> does not need to be exact. All revisions up to and including the
    /// given time will be included.
    /// </remarks>
    public async Task<string> GetDiffHtmlAsync(
        WikiOptions options,
        IDataStore dataStore,
        DateTimeOffset time)
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        if (revisions.Count == 0)
        {
            return string.Empty;
        }
        return RenderHtml(options, dataStore, await TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            Title.WithDefaultTitle(options.MainPageTitle),
            Revision.GetDiff(revisions, "html")),
            Title);
    }

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="time"/> and the current version of
    /// the text.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <param name="format">
    /// <para>
    /// The format used.
    /// </para>
    /// <para>
    /// Can be either "delta" (the default), "gnu", or "md" (case insensitive).
    /// </para>
    /// <para>
    /// The "delta" format (the default, used if an empty string or whitespace is passed) renders a
    /// compact, encoded string which describes each diff operation. The first character is '=' for
    /// unchanged text, '+' for an insertion, and '-' for deletion. Unchanged text and deletions are
    /// followed by their length only; insertions are followed by a compressed version of their full
    /// text. Each diff is separated by a tab character ('\t').
    /// </para>
    /// <para>
    /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
    /// nothing if the text was unchanged. Each diff is separated by a newline.
    /// </para>
    /// <para>
    /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
    /// nothing if the text was unchanged. Diffs are concatenated without separators.
    /// </para>
    /// </param>
    /// <returns>
    /// A string representing the difference between the resulting text after the given sequence of
    /// revisions and the current version of the text.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    public async Task<string> GetDiffWithCurrentAsync(
        IDataStore dataStore,
        DateTimeOffset time,
        string format = "md")
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        return Diff
            .GetWordDiff(Revision.GetText(revisions), MarkdownContent)
            .ToString(format);
    }

    /// <summary>
    /// Gets a diff between the text at the given <paramref name="time"/> and the current version of
    /// the text, as rendered HTML.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <returns>
    /// <para>
    /// A string representing the difference between the resulting text after the given sequence of
    /// revisions and the current version of the text, as rendered HTML.
    /// </para>
    /// <para>
    /// This format renders the text surrounded by a span with class "diff-deleted" for deletion,
    /// "diff-inserted" for addition, or without a wrapping span if the text was unchanged. Diffs
    /// are concatenated without separators.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    public async Task<string> GetDiffWithCurrentHtmlAsync(
        WikiOptions options,
        IDataStore dataStore,
        DateTimeOffset time)
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        var diff = Diff
            .GetWordDiff(Revision.GetText(revisions), MarkdownContent)
            .ToString("html");
        return RenderHtml(options, dataStore, await TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            Title.WithDefaultTitle(options.MainPageTitle),
            diff),
            Title);
    }

    /// <summary>
    /// Gets a diff between the text at two given times.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="firstTime">
    /// The first revision time to compare.
    /// </param>
    /// <param name="secondTime">
    /// The second revision time to compare.
    /// </param>
    /// <param name="format">
    /// <para>
    /// The format used.
    /// </para>
    /// <para>
    /// Can be either "delta" (the default), "gnu", or "md" (case insensitive).
    /// </para>
    /// <para>
    /// The "delta" format (the default, used if an empty string or whitespace is passed) renders a
    /// compact, encoded string which describes each diff operation. The first character is '=' for
    /// unchanged text, '+' for an insertion, and '-' for deletion. Unchanged text and deletions are
    /// followed by their length only; insertions are followed by a compressed version of their full
    /// text. Each diff is separated by a tab character ('\t').
    /// </para>
    /// <para>
    /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
    /// nothing if the text was unchanged. Each diff is separated by a newline.
    /// </para>
    /// <para>
    /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
    /// nothing if the text was unchanged. Diffs are concatenated without separators.
    /// </para>
    /// </param>
    /// <returns>
    /// A string representing the difference between the text at two given times.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values are
    /// swapped. In other words, the diff is always from an earlier version to a later version.
    /// </remarks>
    public async Task<string> GetDiffWithOtherAsync(
        IDataStore dataStore,
        DateTimeOffset firstTime,
        DateTimeOffset secondTime,
        string format = "md")
    {
        if (secondTime < firstTime)
        {
            (firstTime, secondTime) = (secondTime, firstTime);
        }
        var firstRevisions = await GetRevisionsUntilAsync(dataStore, firstTime)
            .ConfigureAwait(false);
        var secondRevisions = await GetRevisionsUntilAsync(dataStore, secondTime)
            .ConfigureAwait(false);
        return Diff.GetWordDiff(
            Revision.GetText(firstRevisions),
            Revision.GetText(secondRevisions))
            .ToString(format);
    }

    /// <summary>
    /// Gets a diff between the text at two given times, as rendered HTML.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="firstTime">
    /// The first revision time to compare.
    /// </param>
    /// <param name="secondTime">
    /// The second revision time to compare.
    /// </param>
    /// <returns>
    /// <para>
    /// A string representing the difference between the text at two given times, as rendered HTML.
    /// </para>
    /// <para>
    /// This format renders the text surrounded by a span with class "diff-deleted" for deletion,
    /// "diff-inserted" for addition, or without a wrapping span if the text was unchanged. Diffs
    /// are concatenated without separators.
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// A revision was incorrectly formatted; or, the sequence of revisions is not a well-ordered
    /// set of revisions which start with a milestone and apply seamlessly in the order given.
    /// </exception>
    /// <remarks>
    /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values are
    /// swapped. In other words, the diff is always from an earlier version to a later version.
    /// </remarks>
    public async Task<string> GetDiffWithOtherHtmlAsync(
        WikiOptions options,
        IDataStore dataStore,
        DateTimeOffset firstTime,
        DateTimeOffset secondTime)
    {
        if (secondTime < firstTime)
        {
            (firstTime, secondTime) = (secondTime, firstTime);
        }
        var firstRevisions = await GetRevisionsUntilAsync(dataStore, firstTime)
            .ConfigureAwait(false);
        var secondRevisions = await GetRevisionsUntilAsync(dataStore, secondTime)
            .ConfigureAwait(false);
        var diff = Diff.GetWordDiff(
            Revision.GetText(firstRevisions),
            Revision.GetText(secondRevisions))
            .ToString("html");
        return RenderHtml(options, dataStore, await TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            Title.WithDefaultTitle(options.MainPageTitle),
            diff),
            Title);
    }

    /// <summary>
    /// Gets a subset of the revision history of this item, in reverse chronological order (most
    /// recent first).
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="pageNumber">The current page number. The first page is 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="start">The earliest timestamp to retrieve.</param>
    /// <param name="end">The most recent timestamp to retrieve.</param>
    /// <param name="condition">An optional condition which the items must satisfy.</param>
    /// <returns>
    /// An <see cref="IPagedList{T}"/> of <see cref="Wiki.Revision"/> instances.
    /// </returns>
    public async Task<PagedList<Revision>> GetHistoryAsync(
        IDataStore dataStore,
        int pageNumber = 1,
        int pageSize = 50,
        DateTimeOffset? start = null,
        DateTimeOffset? end = null,
        Expression<Func<Revision, bool>>? condition = null)
    {
        var history = await PageHistory
            .GetPageHistoryAsync(dataStore, Title)
            .ConfigureAwait(false);
        if (history?.Revisions is null)
        {
            return new(null, pageNumber, pageSize, 0);
        }

        Expression<Func<Revision, bool>>? exp = null;
        if (start.HasValue)
        {
            exp = x => x.TimestampTicks >= start.Value.UtcTicks;
        }
        if (end.HasValue)
        {
            exp = exp is null
                ? x => x.TimestampTicks <= end.Value.UtcTicks
                : exp.AndAlso(x => x.TimestampTicks <= end.Value.UtcTicks);
        }
        if (condition is not null)
        {
            exp = exp is null
                ? condition
                : exp.AndAlso(condition);
        }
        var revisions = exp is null
            ? history.Revisions
            : history
                .Revisions
                .Where(exp.Compile())
                .ToList();
        return revisions
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsPagedList(pageNumber, pageSize, revisions.Count);
    }

    /// <summary>
    /// Gets this item's content rendered as HTML.
    /// </summary>
    /// <returns>The rendered HTML.</returns>
    public override async ValueTask<string> GetHtmlAsync(WikiOptions options, IDataStore dataStore)
        => RenderHtml(
            options,
            dataStore,
            await PostprocessMarkdownAsync(options, dataStore, MarkdownContent),
            Title);

    /// <summary>
    /// Gets this page's content at the given <paramref name="time"/>, rendered as HTML.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <returns>The rendered HTML.</returns>
    public async Task<string> GetHtmlAsync(
        WikiOptions options,
        IDataStore dataStore,
        DateTimeOffset time)
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        if (revisions.Count == 0)
        {
            return string.Empty;
        }
        return RenderHtml(options, dataStore, await TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            Title.WithDefaultTitle(options.MainPageTitle),
            Revision.GetText(revisions)),
            Title);
    }

    /// <summary>
    /// Gets this page's content at the given <paramref name="time"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="time">
    /// The time of the final revision.
    /// </param>
    /// <returns>The markdown as it was at the given <paramref name="time"/>.</returns>
    public async Task<string> GetMarkdownAsync(IDataStore dataStore, DateTimeOffset time)
    {
        var revisions = await GetRevisionsUntilAsync(dataStore, time)
            .ConfigureAwait(false);
        if (revisions.Count == 0)
        {
            return string.Empty;
        }
        return Revision.GetText(revisions);
    }

    /// <summary>
    /// Gets the given markdown content as plain text (i.e. strips all formatting).
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="markdown">The markdown content.</param>
    /// <param name="characterLimit">The maximum number of characters to return.</param>
    /// <param name="singleParagraph">
    /// If true, stops after the first paragraph break, even still under the allowed character limit.
    /// </param>
    /// <returns>The plain text.</returns>
    public override async ValueTask<string> GetPlainTextAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? markdown,
        int? characterLimit = 200,
        bool singleParagraph = true) => FormatPlainText(
            options,
            dataStore,
            await PostprocessMarkdownAsync(options, dataStore, markdown),
            characterLimit,
            singleParagraph,
            Title);

    /// <summary>
    /// Gets this item's content as plain text (i.e. strips all formatting).
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="characterLimit">The maximum number of characters to return.</param>
    /// <param name="singleParagraph">
    /// If true, stops after the first paragraph break, even still under the allowed character limit.
    /// </param>
    /// <returns>The plain text.</returns>
    public override async ValueTask<string> GetPlainTextAsync(
        WikiOptions options,
        IDataStore dataStore,
        int? characterLimit = 200,
        bool singleParagraph = true) => FormatPlainText(
            options,
            dataStore,
            await PostprocessMarkdownAsync(options, dataStore, MarkdownContent),
            characterLimit,
            singleParagraph,
            Title);

    /// <summary>
    /// Renames a <see cref="Page"/> instance, turns the previous title into a redirect to the new
    /// one, updates the wiki accordingly, and saves both pages to the data store.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The new title of the page.</param>
    /// <param name="editor">
    /// The ID of the user who made this edit.
    /// </param>
    /// <param name="markdown">The raw markdown content for the renamed page.</param>
    /// <param name="comment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The ID of the intended owner of both pages.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The users allowed to edit either page.
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
    /// The users allowed to view either page.
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
    /// The groups allowed to edit either page.
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
    /// The groups allowed to view either page.
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
    /// If the new page will redirect to another, this indicates the title of the destination.
    /// </param>
    /// <remarks>
    /// Note: any redirects which point to this page will be updated to point to the new, renamed
    /// page instead.
    /// </remarks>
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
    public async Task RenameAsync<T>(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title,
        string editor,
        string? markdown = null,
        string? comment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null) where T : Page, IPage<T>
    {
        if (!CanRename)
        {
            throw new InvalidOperationException("This type of page cannot be renamed");
        }
        if ((string.CompareOrdinal(title.Namespace, options.CategoryNamespace) == 0
            && this is not Category)
            || (string.CompareOrdinal(title.Namespace, options.FileNamespace) == 0
            && this is not WikiFile)
            || (string.CompareOrdinal(title.Namespace, options.GroupNamespace) == 0
            && string.CompareOrdinal(Title.Namespace, options.GroupNamespace) != 0)
            || (string.CompareOrdinal(title.Namespace, options.ScriptNamespace) == 0
            && string.CompareOrdinal(Title.Namespace, options.ScriptNamespace) != 0)
            || (string.CompareOrdinal(title.Namespace, options.UserNamespace) == 0
            && string.CompareOrdinal(Title.Namespace, options.UserNamespace) != 0))
        {
            throw new InvalidOperationException($"Cannot move a page to the {title.Namespace} namespace");
        }
        if ((this is Category
            && string.CompareOrdinal(title.Namespace, options.CategoryNamespace) != 0)
            || (this is WikiFile
            && string.CompareOrdinal(title.Namespace, options.FileNamespace) != 0)
            || (string.CompareOrdinal(Title.Namespace, options.GroupNamespace) == 0
            && string.CompareOrdinal(title.Namespace, options.GroupNamespace) != 0)
            || (string.CompareOrdinal(Title.Namespace, options.ScriptNamespace) == 0
            && string.CompareOrdinal(title.Namespace, options.ScriptNamespace) != 0)
            || (string.CompareOrdinal(Title.Namespace, options.UserNamespace) == 0
            && string.CompareOrdinal(title.Namespace, options.UserNamespace) != 0))
        {
            throw new InvalidOperationException($"Cannot move a page from the {Title.Namespace} namespace");
        }

        var newPage = T.Empty(title);
        await newPage.UpdateAsync(
            options,
            dataStore,
            editor,
            markdown,
            comment,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            redirectTitle)
            .ConfigureAwait(false);

        await UpdateAsync(
            options,
            dataStore,
            editor,
            null,
            comment,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            title)
            .ConfigureAwait(false);
        if (RedirectReferences is not null)
        {
            foreach (var reference in RedirectReferences)
            {
                var redirect = await IPage<T>
                    .GetExistingPageAsync<T>(dataStore, reference)
                    .ConfigureAwait(false);
                if (redirect is not null)
                {
                    await redirect.UpdateRedirectAsync(options, dataStore, title)
                        .ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// <para>
    /// Always throws an <see cref="InvalidOperationException"/>.
    /// </para>
    /// <para>
    /// <see cref="Page"/> is an abstract type, and cannot create an instance of itself.
    /// </para>
    /// <para>
    /// This method exists only to fulfill the implementation requirements of <see
    /// cref="IPage{TSelf}"/>.
    /// </para>
    /// <para>
    /// You should only call this method on a subclass, never on <see cref="Page"/> itself.
    /// </para>
    /// </summary>
    /// <returns>
    /// Never returns. Always throws an <see cref="InvalidOperationException"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Always thrown.
    /// </exception>
    public virtual Task RenameAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title,
        string editor,
        string? markdown = null,
        string? comment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null)
        => throw new InvalidOperationException();

    /// <summary>
    /// Configures a <see cref="Page"/> instance, updates the wiki accordingly, and saves the page
    /// to the data store.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="editor">
    /// The ID of the user who made this edit.
    /// </param>
    /// <param name="markdown">The raw markdown content.</param>
    /// <param name="comment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The ID of the intended owner of the page.
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
    public async Task UpdateAsync(
        WikiOptions options,
        IDataStore dataStore,
        string editor,
        string? markdown = null,
        string? comment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null)
    {
        if (!CanRename)
        {
            throw new InvalidOperationException("Cannot redirect this type of page");
        }
        if (redirectTitle.HasValue
            && (string.CompareOrdinal(Title.Namespace, options.CategoryNamespace) == 0
            || string.CompareOrdinal(Title.Namespace, options.GroupNamespace) == 0
            || string.CompareOrdinal(Title.Namespace, options.ScriptNamespace) == 0
            || string.CompareOrdinal(Title.Namespace, options.UserNamespace) == 0))
        {
            throw new ArgumentException($"Cannot redirect a page in the {Title.Namespace} namespace", nameof(redirectTitle));
        }
        var md = redirectTitle.HasValue ? null : markdown;
        var previousRevision = Revision;
        Revision = new Revision(
            editor,
            MarkdownContent,
            md,
            comment);
        await AddRevisionAsync(dataStore, Revision)
            .ConfigureAwait(false);

        if (Revision.IsDeleted)
        {
            if (this is not Category
                && string.CompareOrdinal(Title.Namespace, options.GroupNamespace) != 0
                && string.CompareOrdinal(Title.Namespace, options.UserNamespace) != 0)
            {
                await NormalizedPageReference
                    .RemoveReferenceAsync(dataStore, Title, Id)
                    .ConfigureAwait(false);
            }
        }
        else
        {
            await NormalizedPageReference
                .AddReferenceAsync(dataStore, Title, Id)
                .ConfigureAwait(false);
        }

        var isScript = string.CompareOrdinal(Title.Namespace, options.ScriptNamespace) == 0;

        List<PageTitle> transclusions;
        List<Heading> headings;
        if (isScript
            || redirectTitle.HasValue
            || string.IsNullOrEmpty(md))
        {
            transclusions = [];
            headings = [];
        }
        else
        {
            (md, transclusions) = await TransclusionParser.TranscludeInnerAsync(
                options,
                dataStore,
                Title.WithDefaultTitle(options.MainPageTitle),
                md);
            headings = TableOfContentsParser.Parse(options, md);
        }

        if (Transclusions is not null)
        {
            await RemoveTransclusionReferencesAsync(dataStore, transclusions)
                .ConfigureAwait(false);
        }
        Transclusions = transclusions.Count > 0
            ? transclusions.AsReadOnly()
            : null;

        Headings = headings.Count > 0
            ? headings.AsReadOnly()
            : null;

        WikiLinks = (isScript || redirectTitle.HasValue
            ? []
            : GetWikiLinks(options, dataStore, md, Title))
            .AsReadOnly();
        if (References is not null)
        {
            await RemoveReferencesAsync(dataStore)
                .ConfigureAwait(false);
        }

        if (redirectTitle.HasValue)
        {
            Html = string.Empty;
            MarkdownContent = string.Empty;
            Preview = string.Empty;
        }
        else if (isScript)
        {
            Html = MarkdownContent;
            MarkdownContent = markdown ?? string.Empty;
            Preview = GetScriptPreview(md);
        }
        else
        {
            Html = RenderHtml(options, dataStore, md, Title);
            MarkdownContent = markdown ?? string.Empty;
            Preview = RenderPreview(
                options,
                dataStore,
                await PostprocessPageMarkdownAsync(
                    options,
                    dataStore,
                    Title,
                    markdown,
                    true),
                Title);
        }

        await UpdateCategoriesAsync(dataStore)
            .ConfigureAwait(false);

        var oldOwner = Owner;
        Owner = owner;
        if (!string.IsNullOrEmpty(owner))
        {
            AllowedEditors = allowedEditors?.ToList().AsReadOnly();
            AllowedViewers = allowedViewers?.ToList().AsReadOnly();
            AllowedEditorGroups = allowedEditorGroups?.ToList().AsReadOnly();
            AllowedViewerGroups = allowedViewerGroups?.ToList().AsReadOnly();
        }

        await UpdateRedirectAsync(options, dataStore, redirectTitle)
            .ConfigureAwait(false);

        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);

        await AddTransclusionReferencesAsync(options, dataStore)
            .ConfigureAwait(false);

        await AddReferencesAsync(options, dataStore)
            .ConfigureAwait(false);

        await UpdateReferencesAsync(options, dataStore)
            .ConfigureAwait(false);

        if (Revision.IsDeleted)
        {
            if (previousRevision?.IsDeleted == false)
            {
                if (options.OnDeleted is not null
                    && this is not Category
                    && string.CompareOrdinal(Title.Namespace, options.GroupNamespace) != 0
                    && string.CompareOrdinal(Title.Namespace, options.UserNamespace) != 0)
                {
                    await options.OnDeleted
                        .Invoke(this, oldOwner, Owner)
                        .ConfigureAwait(false);
                }
                else if (options.OnEdited is not null)
                {
                    await options.OnEdited
                        .Invoke(this, Revision, oldOwner, Owner)
                        .ConfigureAwait(false);
                }
            }
        }
        else if (previousRevision?.IsDeleted != false)
        {
            if (options.OnCreated is not null)
            {
                await options.OnCreated
                    .Invoke(this, editor)
                    .ConfigureAwait(false);
            }
        }
        else if (options.OnEdited is not null)
        {
            await options.OnEdited
                .Invoke(this, Revision, oldOwner, Owner)
                .ConfigureAwait(false);
        }

        if (Revision.IsMilestone)
        {
            Revision = new Revision(
                Revision.Editor,
                null,
                Revision.IsDeleted,
                true,
                Revision.Comment,
                Revision.TimestampTicks);
        }
    }

    internal static async Task RestoreAsync<T>(
        WikiOptions options,
        IDataStore dataStore,
        T page,
        string editor,
        string? newTitle,
        string? newNamespace) where T : Page, IPage<T>
    {
        var title = new PageTitle(
            newTitle ?? page.Title.Title,
            newNamespace ?? page.Title.Namespace,
            page.Title.Domain);

        var newPage = await dataStore
            .GetWikiPageAsync(options, title, true, true)
            .ConfigureAwait(false);

        await newPage.UpdateAsync(
            options,
            dataStore,
            editor,
            page.MarkdownContent,
            "archive restored",
            page.Owner,
            page.AllowedEditors,
            page.AllowedViewers,
            page.AllowedEditorGroups,
            page.AllowedViewerGroups,
            page.RedirectTitle)
            .ConfigureAwait(false);
    }

    internal virtual Page GetArchiveCopy()
    {
        var page = Copy();
        page.Categories = null;
        page.Headings = null;
        page.Html = string.Empty;
        page.Preview = string.Empty;
        page.RedirectReferences = null;
        page.References = null;
        page.Revision = null;
        page.TransclusionReferences = null;
        page.Transclusions = null;
        page.WikiLinks = new List<WikiLink>().AsReadOnly();
        return page;
    }

    /// <summary>
    /// Gets a copy of this instance.
    /// </summary>
    /// <param name="newNamespace">A new namespace to assign to the copied page.</param>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> with the same properties as this instance.
    /// </returns>
    /// <remarks>
    /// References and calculated properties are not copied, nor is the latest <see
    /// cref="Revision"/>.
    /// </remarks>
    protected T Copy<T>(string? newNamespace = null) where T : Page, IPage<T>
    {
        var newPage = T.Empty(string.IsNullOrEmpty(newNamespace)
            ? Title
            : Title.WithNamespace(newNamespace));
        newPage.MarkdownContent = MarkdownContent;
        newPage.Owner = Owner;
        newPage.AllowedEditors = AllowedEditors?.ToList().AsReadOnly();
        newPage.AllowedViewers = AllowedViewers?.ToList().AsReadOnly();
        newPage.AllowedEditorGroups = AllowedEditorGroups?.ToList().AsReadOnly();
        newPage.AllowedViewerGroups = AllowedViewerGroups?.ToList().AsReadOnly();
        newPage.RedirectTitle = RedirectTitle;
        return newPage;
    }

    private static string GetScriptPreview(string? markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return string.Empty;
        }
        if (markdown.Length <= 100)
        {
            return markdown;
        }
        var newline = markdown.IndexOf(Environment.NewLine, 100);
        return newline == -1
            ? markdown[Math.Min(markdown.Length, 500)..]
            : markdown[newline..];
    }

    private protected static ValueTask<string> PostprocessPageMarkdownAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title,
        string? markdown,
        bool isPreview = false)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return ValueTask.FromResult(string.Empty);
        }

        return TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            title.WithDefaultTitle(options.MainPageTitle),
            markdown,
            isPreview: isPreview);
    }

    /// <summary>
    /// Adds a page to this one's redirect reference collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which redirects to this one.
    /// </param>
    private async Task AddRedirectReferenceAsync(IDataStore dataStore, PageTitle title)
    {
        if (RedirectReferences?.Contains(title) == true)
        {
            return;
        }

        var references = RedirectReferences?.ToList() ?? [];
        references.Add(title);
        RedirectReferences = references.AsReadOnly();
        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a page to this one's reference collection.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which links to this one.
    /// </param>
    private async Task AddReferenceAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title)
    {
        if (References?.Contains(title) == true)
        {
            return;
        }

        var references = References?.ToList() ?? [];
        references.Add(title);
        References = references.AsReadOnly();

        if (!IsMissing
            && Revision?.IsDeleted != false
            && !RedirectTitle.HasValue
            && this is not Category
            && string.CompareOrdinal(Title.Namespace, options.UserNamespace) != 0
            && string.CompareOrdinal(Title.Namespace, options.GroupNamespace) != 0)
        {
            IsMissing = true;
        }

        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    private protected async Task AddReferencesAsync(
        WikiOptions options,
        IDataStore dataStore)
    {
        foreach (var link in WikiLinks.Where(x => !x.IsCategory))
        {
            var reference = await dataStore.GetWikiPageAsync(options, link.Title, true)
                .ConfigureAwait(false);
            await reference.AddReferenceAsync(options, dataStore, Title)
                .ConfigureAwait(false);
        }
    }

    private protected async Task AddRevisionAsync(IDataStore dataStore, Revision revision)
    {
        var history = await PageHistory
            .GetPageHistoryAsync(dataStore, Title)
            .ConfigureAwait(false);
        if (history is null)
        {
            await PageHistory
                .NewAsync(dataStore, Title, [revision])
                .ConfigureAwait(false);
        }
        else
        {
            var revisions = history.Revisions?.ToList() ?? [];
            revisions.Insert(0, revision);
            await dataStore.StoreItemAsync(history)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Adds a page to this one's transclusion reference collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which transcludes this one.
    /// </param>
    private async Task AddTransclusionReferenceAsync(IDataStore dataStore, PageTitle title)
    {
        if (TransclusionReferences?.Contains(title) == true)
        {
            return;
        }

        var references = TransclusionReferences?.ToList() ?? [];
        references.Add(title);
        TransclusionReferences = references.AsReadOnly();
        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    private protected async Task AddTransclusionReferencesAsync(
        WikiOptions options,
        IDataStore dataStore)
    {
        if (Transclusions is null)
        {
            return;
        }
        foreach (var transclusion in Transclusions)
        {
            var reference = await dataStore.GetWikiPageAsync(options, transclusion, true, true)
                .ConfigureAwait(false);
            await reference.AddTransclusionReferenceAsync(dataStore, Title)
                .ConfigureAwait(false);
        }
    }

    private (
        HashSet<PageTitle> titlesToUpdate,
        HashSet<PageTitle> titlesToUpdateRecursively,
        HashSet<PageTitle> redirectsToUpdate) GetIdsToUpdate()
    {
        var titlesToUpdate = new HashSet<PageTitle>();
        var titlesToUpdateRecursively = new HashSet<PageTitle>();
        var redirectsToUpdate = new HashSet<PageTitle>();

        if (References is not null)
        {
            titlesToUpdate.UnionWith(References);
        }
        if (TransclusionReferences is not null)
        {
            titlesToUpdate.UnionWith(TransclusionReferences);
            titlesToUpdateRecursively.UnionWith(TransclusionReferences);
        }
        if (RedirectReferences is not null)
        {
            titlesToUpdate.UnionWith(RedirectReferences);
            titlesToUpdateRecursively.UnionWith(RedirectReferences);
            redirectsToUpdate.UnionWith(RedirectReferences);
        }

        return (
            titlesToUpdate,
            titlesToUpdateRecursively,
            redirectsToUpdate);
    }

    private async Task<IReadOnlyList<Revision>> GetRevisionsUntilAsync(IDataStore dataStore, DateTimeOffset time)
    {
        var ticks = time.UtcTicks;
        var history = await PageHistory
            .GetPageHistoryAsync(dataStore, Title)
            .ConfigureAwait(false);
        var lastMilestone = history?
            .Revisions?
            .FirstOrDefault(x => x.TimestampTicks <= ticks && x.IsMilestone);
        if (lastMilestone is null)
        {
            return [];
        }
        return history!
            .Revisions!
            .Where(x => x.TimestampTicks >= lastMilestone.TimestampTicks && x.TimestampTicks <= ticks)
            .Reverse()
            .ToList();
    }

    private protected override ValueTask<string> PostprocessMarkdownAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? markdown,
        bool isPreview = false) => PostprocessPageMarkdownAsync(
            options,
            dataStore,
            Title,
            markdown,
            isPreview);

    /// <summary>
    /// Removes a page from this one's redirect reference collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which no longer redirects to this one.
    /// </param>
    private async ValueTask RemoveRedirectReferenceAsync(IDataStore dataStore, PageTitle title)
    {
        if (RedirectReferences?.Contains(title) != true)
        {
            return;
        }

        RedirectReferences = RedirectReferences.ToImmutableList().Remove(title);
        if (RedirectReferences.Count == 0)
        {
            RedirectReferences = null;
        }

        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a page from this one's reference collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which no longer links to this one.
    /// </param>
    private async ValueTask RemoveReferenceAsync(IDataStore dataStore, PageTitle title)
    {
        if (References?.Contains(title) != true)
        {
            return;
        }

        References = References.ToImmutableList().Remove(title);
        if (References.Count == 0)
        {
            References = null;

            if (IsMissing)
            {
                IsMissing = false;
            }
        }

        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    private async ValueTask RemoveReferencesAsync(IDataStore dataStore)
    {
        if (References is null)
        {
            return;
        }
        foreach (var title in References
            .Except(WikiLinks
                .Where(x => !x.IsCategory)
                .Select(x => x.Title)))
        {
            var reference = await IPage<Page>
                .GetExistingPageAsync<Page>(dataStore, title)
                .ConfigureAwait(false);
            if (reference is not null)
            {
                await reference.RemoveReferenceAsync(dataStore, Title)
                    .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Removes a page from this one's transclusion reference collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of a page which no longer transcludes this one.
    /// </param>
    private async ValueTask RemoveTransclusionReferenceAsync(IDataStore dataStore, PageTitle title)
    {
        if (TransclusionReferences?.Contains(title) != true)
        {
            return;
        }

        TransclusionReferences = TransclusionReferences.ToImmutableList().Remove(title);
        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    private async ValueTask RemoveTransclusionReferencesAsync(
        IDataStore dataStore,
        IReadOnlyCollection<PageTitle> newTransclusions)
    {
        if (Transclusions is null)
        {
            return;
        }
        foreach (var title in Transclusions.Except(newTransclusions))
        {
            var reference = await IPage<Page>
                .GetExistingPageAsync<Page>(dataStore, title)
                .ConfigureAwait(false);
            if (reference is not null)
            {
                await reference.RemoveTransclusionReferenceAsync(dataStore, Title)
                    .ConfigureAwait(false);
            }
        }
    }

    private async Task UpdateCategoriesAsync(IDataStore dataStore)
    {
        // An article can only be categorized by categories in its own domain, or none
        var currentCategories = WikiLinks
            .Where(x => x.IsCategory
                && !x.IsEscaped
                && (string.IsNullOrEmpty(x.Title.Domain)
                || string.Equals(x.Title.Domain, Title.Domain)))
            .Select(x => x.Title);

        var newCategories = new HashSet<PageTitle>();
        var retainedCategories = new HashSet<PageTitle>();
        foreach (var category in currentCategories)
        {
            if (Categories?.Contains(category) == true)
            {
                retainedCategories.Add(category);
            }
            else
            {
                newCategories.Add(category);
            }
        }
        foreach (var categoryTitle in newCategories)
        {
            var category = await IPage<Category>
                .GetPageAsync<Category>(dataStore, categoryTitle, true, true)
                .ConfigureAwait(false);
            await category.AddPageAsync(dataStore, categoryTitle)
                .ConfigureAwait(false);
        }

        if (Categories is not null)
        {
            foreach (var categoryTitle in Categories)
            {
                if (!retainedCategories.Contains(categoryTitle))
                {
                    var category = await IPage<Category>
                        .GetExistingPageAsync<Category>(dataStore, categoryTitle)
                        .ConfigureAwait(false);
                    if (category is not null)
                    {
                        await category.RemovePageAsync(dataStore, categoryTitle)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        retainedCategories.UnionWith(newCategories);
        Categories = retainedCategories.ToList().AsReadOnly();
    }

    private async Task UpdateRedirectAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle? newRedirectTitle)
    {
        if (!RedirectTitle.HasValue
            && !newRedirectTitle.HasValue)
        {
            return;
        }

        if (RedirectTitle.HasValue
            && newRedirectTitle.HasValue
            && newRedirectTitle.Equals(RedirectTitle.Value))
        {
            return;
        }

        var previousRedirect = RedirectTitle.HasValue
            ? await IPage<Page>
                .GetExistingPageAsync<Page>(dataStore, RedirectTitle.Value)
                .ConfigureAwait(false)
            : null;
        if (previousRedirect is not null)
        {
            await previousRedirect
                .RemoveRedirectReferenceAsync(dataStore, Title)
                .ConfigureAwait(false);
        }

        RedirectTitle = newRedirectTitle;

        if (!newRedirectTitle.HasValue)
        {
            IsBrokenRedirect = false;
            IsDoubleRedirect = false;
            return;
        }

        // Redirect to a category is not valid.
        if (string.CompareOrdinal(RedirectTitle!.Value.Namespace, options.CategoryNamespace) == 0)
        {
            IsBrokenRedirect = true;
            IsDoubleRedirect = false;
            return;
        }

        Page? redirect;
        var count = 0;
        var ids = new HashSet<string>();
        var finalRedirectTitle = RedirectTitle.Value;
        do
        {
            var id = IPage<Page>.GetId(finalRedirectTitle);
            if (ids.Contains(id)) // abort if a cycle is detected
            {
                IsBrokenRedirect = true;
                IsDoubleRedirect = true;
                return;
            }

            redirect = await dataStore
                .GetExistingWikiPageAsync(options, finalRedirectTitle)
                .ConfigureAwait(false);
            if (redirect?.Exists != true)
            {
                IsBrokenRedirect = true;
                IsDoubleRedirect = count > 0;
                break;
            }

            ids.Add(id);

            if (redirect.RedirectTitle.HasValue)
            {
                finalRedirectTitle = redirect.RedirectTitle.Value;
            }

            count++;
        } while (redirect.RedirectTitle.HasValue && count < 100); // abort if redirection nests 100 levels

        if (redirect?.RedirectTitle.HasValue != true)
        {
            RedirectTitle = finalRedirectTitle;
        }

        if (redirect?.Title != RedirectTitle)
        {
            redirect = await dataStore
                .GetWikiPageAsync(options, finalRedirectTitle, true, true)
                .ConfigureAwait(false);
        }
        IsBrokenRedirect = redirect.Exists;
        IsDoubleRedirect = redirect.RedirectTitle.HasValue;

        await redirect.AddRedirectReferenceAsync(dataStore, Title)
            .ConfigureAwait(false);
    }

    private protected async Task UpdateReferencesAsync(
        WikiOptions options,
        IDataStore dataStore)
    {
        var titlesUpdated = new HashSet<PageTitle>();
        var titlesUpdatedRecursively = new HashSet<PageTitle>();

        var (titlesToUpdate,
            titlesToUpdateRecursively,
            redirectsToUpdate) = GetIdsToUpdate();

        while (titlesToUpdate.Count > 0)
        {
            var titleToUpdate = titlesToUpdate.First();
            titlesToUpdate.Remove(titleToUpdate);

            var referringArticle = await IPage<Page>
                .GetExistingPageAsync<Page>(dataStore, titleToUpdate)
                .ConfigureAwait(false);
            if (referringArticle is null)
            {
                titlesUpdated.Add(titleToUpdate);
                titlesUpdatedRecursively.Add(titleToUpdate);
                continue;
            }

            if (!titlesUpdated.Contains(titleToUpdate))
            {
                if (redirectsToUpdate.Contains(titleToUpdate))
                {
                    referringArticle.IsBrokenRedirect = Exists;
                    referringArticle.IsDoubleRedirect = RedirectTitle.HasValue;
                    redirectsToUpdate.Remove(titleToUpdate);
                }

                if (string.CompareOrdinal(
                    referringArticle.Title.Namespace,
                    options.ScriptNamespace) == 0)
                {
                    await referringArticle.UpdateAsync(options, dataStore);
                }
                else
                {
                    await referringArticle.UpdateContentAsync(
                        options,
                        dataStore,
                        referringArticle.Title);
                }
                await dataStore.StoreItemAsync(referringArticle)
                    .ConfigureAwait(false);

                titlesUpdated.Add(titleToUpdate);
            }

            if (titlesToUpdateRecursively.Remove(titleToUpdate))
            {
                titlesUpdatedRecursively.Add(titleToUpdate);

                var (childTitlesToUpdate,
                    childTitlesToUpdateRecursively,
                    _) = referringArticle.GetIdsToUpdate();
                foreach (var id in childTitlesToUpdate.Except(titlesUpdated))
                {
                    titlesToUpdate.Add(id);
                }
                foreach (var id in childTitlesToUpdateRecursively.Except(titlesUpdatedRecursively))
                {
                    titlesToUpdate.Add(id);
                    titlesToUpdateRecursively.Add(id);
                }
            }
        }
    }
}
