using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// An archive is a storage class used to (de)serialize an entire wiki, or a domain within a wiki.
/// </summary>
/// <remarks>
/// <para>
/// The advantage of an archive over backup and restoration of underlying storage is that not
/// everything needs to be stored in order to reconstitute a wiki. Many of the data objects
/// maintained by the wiki are intended to optimize performance, and can be rebuilt on demand.
/// </para>
/// <para>
/// This represents a tradeoff between storage size and restoration speed. If your goal is to save
/// and restore a wiki as quickly as possible, the native save and restore options for your
/// underlying data source are likely to be far superior. If your aim is to make the size of the
/// storage item as small as possible, an archive object will likely be significantly more compact.
/// </para>
/// <para>
/// Anotehr advantage of an archive is that it can be imported into an existing wiki in-place,
/// without deleting the current contents. This can be used to move domains from one wiki to
/// another, or to move pages from one wiki to another. Archive restoration is "smart" in that it
/// puts articles from the default namespace of the original wiki into the default namespace of the
/// target wiki, even if the defaults of the two wikis is different.
/// </para>
/// </remarks>
public class Archive
{
    /// <summary>
    /// The wiki options used in the source.
    /// </summary>
    /// <remarks>
    /// This property is used to ensure that source pages are correctly mapped to the target wiki,
    /// even if the title for shared pages, or shared namespaces, have been customized differently
    /// in either the source or target.
    /// </remarks>
    public WikiOptions? Options { get; set; }

    /// <summary>
    /// All messages posted to Talk pages in the wiki/domain.
    /// </summary>
    public List<Message>? Messages { get; set; }

    /// <summary>
    /// All the articles, categories, and files in the wiki/domain.
    /// </summary>
    public List<Article>? Pages { get; set; }

    /// <summary>
    /// All the revisions for all articles, categories, and files in the wiki/domain.
    /// </summary>
    public List<Revision>? Revisions { get; set; }

    /// <summary>
    /// Restores this <see cref="Archive"/> to the wiki in the given <see cref="IDataStore"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">
    /// A <see cref="WikiOptions"/> instance describing the target wiki.
    /// </param>
    /// <remarks>
    /// <para>
    /// Pages in the target wiki with the same domain, namespace, and title will be overwritten.
    /// </para>
    /// <para>
    /// The overwritten pages, their history, and their associated talk messages, will not be
    /// deleted from the data source and could still be recovered by whatever means of manually
    /// accessing data the data source provides. They will no longer be accessible from within the
    /// wiki, however.
    /// </para>
    /// <para>
    /// The restore operation is not guaranteed to be transactional. In other words, the process may
    /// fail after some data has been restored, but not all of it. In order to ensure that the
    /// restore operation is fully atomic, you should leverage the transaction mechanisms of your
    /// underlying data source.
    /// </para>
    /// </remarks>
    public async Task RestoreAsync(
        IDataStore dataStore,
        WikiOptions options)
    {
        if (Pages is not null)
        {
            foreach (var page in Pages)
            {
                var revisions = Revisions?
                    .Where(x => x.WikiId == page.Id)
                    .ToList();

                var lastEditor = revisions?
                    .OrderByDescending(x => x.TimestampTicks)
                    .FirstOrDefault()?
                    .Editor;

                if (page is Category category)
                {
                    await category.RestoreAsync(options, dataStore, lastEditor ?? "archive restore");
                }
                else if (page is WikiFile file)
                {
                    await file.RestoreAsync(options, dataStore, lastEditor ?? "archive restore");
                }
                else
                {
                    string? newNamespace = null;
                    if (Options is not null)
                    {
                        if (string.Equals(page.WikiNamespace, Options.CategoryNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.CategoryNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.DefaultNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.DefaultNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.FileNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.FileNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.GroupNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.GroupNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.ScriptNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.ScriptNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.TransclusionNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.TransclusionNamespace;
                        }
                        else if (string.Equals(page.WikiNamespace, Options.UserNamespace, StringComparison.Ordinal))
                        {
                            newNamespace = options.UserNamespace;
                        }
                    }
                    await page.RestoreAsync(options, dataStore, lastEditor ?? "archive restore", newNamespace);
                }

                if (revisions is not null)
                {
                    var first = true;
                    foreach (var revision in revisions.OrderBy(x => x.TimestampTicks))
                    {
                        Revision? newRevision = null;
                        if (first
                            && page is Category
                            && revision.Delta is null
                            && !revision.IsMilestone
                            && !revision.IsDeleted)
                        {
                            var existing = await dataStore
                                .Query<Revision>()
                                .Where(x => x.WikiId == page.Id && x.Delta == null && !x.IsMilestone && !x.IsDeleted)
                                .OrderBy(x => x.TimestampTicks)
                                .FirstOrDefaultAsync();
                            if (existing is not null)
                            {
                                newRevision = new Revision(
                                    existing.Id,
                                    page.Id,
                                    revision.Editor,
                                    revision.Title,
                                    revision.WikiNamespace,
                                    revision.Domain,
                                    null,
                                    false,
                                    false,
                                    revision.Comment,
                                    revision.TimestampTicks);
                            }
                        }
                        newRevision ??= new Revision(
                            revision.Id,
                            page.Id,
                            revision.Editor,
                            revision.Title,
                            revision.WikiNamespace,
                            revision.Domain,
                            revision.Delta,
                            revision.IsDeleted,
                            revision.IsMilestone,
                            revision.Comment,
                            revision.TimestampTicks);
                        await dataStore.StoreItemAsync(newRevision).ConfigureAwait(false);
                        first = false;
                    }
                }
            }
        }

        if (Messages is not null)
        {
            foreach (var message in Messages)
            {
                await dataStore.StoreItemAsync(message).ConfigureAwait(false);
            }
        }
    }
}
