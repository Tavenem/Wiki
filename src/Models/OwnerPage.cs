using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A user page.
/// </summary>
public abstract class OwnerPage : Article
{
    /// <summary>
    /// Whether this page can be renamed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is <see langword="false"/> for <see cref="OwnerPage"/>, since the title is based on the
    /// owner.
    /// </para>
    /// <para>
    /// Not persisted to storage.
    /// </para>
    /// </remarks>
    [JsonIgnore]
    public override bool CanRename => false;

    /// <summary>
    /// <para>
    /// An optional display title for this page, which would override its <see cref="Page.Title"/>'s
    /// <see cref="PageTitle.Title"/> for display purposes.
    /// </para>
    /// <para>
    /// Set to the <see cref="IWikiOwner.DisplayName"/> for an <see cref="OwnerPage"/>.
    /// </para>
    /// </summary>
    public override string? DisplayTitle => OwnerObject?.DisplayName;

    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string OwnerPageIdItemTypeName = ":Page:Article:OwnerPage:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => OwnerPageIdItemTypeName;

    /// <summary>
    /// Constructs a new instance of <see cref="OwnerPage"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor should only be used by deserializers or for testing purposes.
    /// </para>
    /// <para>
    /// To create a new page as part of a normal user interaction, use the <see
    /// cref="WikiExtensions.AddOrReviseWikiPageAsync(IDataStore, WikiOptions, IWikiUserManager,
    /// IWikiGroupManager, IWikiUser, PageTitle, string?, string?, bool, string?,
    /// IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?,
    /// PageTitle?, PageTitle?)"/> method.
    /// </para>
    /// <para>
    /// To create a page programmatically, you can use a combination of <see
    /// cref="WikiExtensions.GetWikiPageAsync(IDataStore, WikiOptions, PageTitle, bool, bool)"/> (to
    /// get the current page, or a new page if one does not already exist), and <see
    /// cref="Page.UpdateAsync(WikiOptions, IDataStore, string, string?, string?, string?,
    /// IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?,
    /// PageTitle?)"/> to update the result with the intended properties.
    /// </para>
    /// </remarks>
    [JsonConstructor]
    protected OwnerPage(
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
        PageTitle? redirectTitle = null) : base(
        title,
        html,
        preview,
        wikiLinks,
        markdownContent,
        owner,
        revision,
        allowedEditors,
        allowedViewers,
        allowedEditorGroups,
        allowedViewerGroups,
        categories,
        headings,
        redirectReferences,
        references,
        transclusionReferences,
        transclusions,
        isBrokenRedirect,
        isDoubleRedirect,
        redirectTitle)
    { }

    private protected OwnerPage(PageTitle title) : base(title) { }
}
