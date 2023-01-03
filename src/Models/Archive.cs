using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// An archive is a storage class used to (de)serialize all the pages in a wiki, or a domain within
/// a wiki.
/// </summary>
/// <remarks>
/// <para>
/// The advantage of an archive over backup and restoration of underlying storage is that not
/// everything needs to be stored in order to reconstitute a wiki's pages. Many of the data objects
/// maintained by the wiki are intended to optimize performance, and can be rebuilt on demand. In
/// addition, an archive discards all history, capturing only the most recent version of every page.
/// An archive also does not record messages, only page content.
/// </para>
/// <para>
/// Using archives is a tradeoff between storage size and restoration speed. If your goal is to save
/// and restore a wiki as quickly as possible, the native save and restore options for your
/// underlying data source are likely to be far superior. If your aim is to make the size of the
/// storage item as small as possible, an archive object will likely be significantly more compact.
/// </para>
/// <para>
/// Another advantage of an archive is that it can be imported into an existing wiki in-place,
/// without deleting the current contents. This can be used to move domains from one wiki to
/// another, or to move pages from one wiki to another. Archive restoration is also "smart" in that
/// it puts articles from well-known namespaces of the original wiki into the same namespaces of the
/// target wiki, even if the options of the two wikis have been set differently.
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
    /// All the pages in the wiki/domain.
    /// </summary>
    public List<Page>? Pages { get; set; }

    /// <summary>
    /// All the topics for pages in the wiki/domain.
    /// </summary>
    public List<Topic>? Topics { get; set; }

    /// <summary>
    /// Restores this <see cref="Archive"/> to the wiki in the given <see cref="IDataStore"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="options">
    /// A <see cref="WikiOptions"/> instance describing the target wiki.
    /// </param>
    /// <param name="editor">
    /// <para>
    /// The ID of the user who is restoring this archive.
    /// </para>
    /// <para>
    /// This will be listed as the editor of all restored pages.
    /// </para>
    /// </param>
    /// <remarks>
    /// <para>
    /// Pages in the target wiki with the same domain, namespace, and title will be overwritten.
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
        WikiOptions options,
        string editor)
    {
        if (Pages is null)
        {
            return;
        }

        foreach (var page in Pages)
        {
            if (page is Category category)
            {
                await Page.RestoreAsync(options, dataStore, category, editor, null, options.CategoryNamespace)
                    .ConfigureAwait(false);
            }
            else if (page is WikiFile file)
            {
                await Page.RestoreAsync(options, dataStore, file, editor, null, options.FileNamespace)
                    .ConfigureAwait(false);
            }
            else if (page is Article article)
            {
                string? newTitle = null;
                string? newNamespace = null;
                if (Options is not null)
                {
                    if (string.CompareOrdinal(page.Title.Namespace, Options.GroupNamespace) == 0)
                    {
                        newNamespace = options.GroupNamespace;
                    }
                    else if (string.CompareOrdinal(page.Title.Namespace, Options.ScriptNamespace) == 0)
                    {
                        newNamespace = options.ScriptNamespace;
                    }
                    else if (string.CompareOrdinal(page.Title.Namespace, Options.TransclusionNamespace) == 0)
                    {
                        newNamespace = options.TransclusionNamespace;
                    }
                    else if (string.CompareOrdinal(page.Title.Namespace, Options.UserNamespace) == 0)
                    {
                        newNamespace = options.UserNamespace;
                    }
                    else if (string.IsNullOrEmpty(page.Title.Domain)
                        && string.CompareOrdinal(page.Title.Namespace, Options.SystemNamespace) == 0)
                    {
                        newNamespace = options.SystemNamespace;

                        if (string.CompareOrdinal(page.Title.Title, Options.AboutPageTitle) == 0)
                        {
                            newTitle = options.AboutPageTitle;
                        }
                        else if (string.CompareOrdinal(page.Title.Title, Options.ContactPageTitle) == 0)
                        {
                            newTitle = options.ContactPageTitle;
                        }
                        else if (string.CompareOrdinal(page.Title.Title, Options.ContentsPageTitle) == 0)
                        {
                            newTitle = options.ContentsPageTitle;
                        }
                        else if (string.CompareOrdinal(page.Title.Title, Options.CopyrightPageTitle) == 0)
                        {
                            newTitle = options.CopyrightPageTitle;
                        }
                        else if (string.CompareOrdinal(page.Title.Title, Options.HelpPageTitle) == 0)
                        {
                            newTitle = options.HelpPageTitle;
                        }
                        else if (string.CompareOrdinal(page.Title.Title, Options.PolicyPageTitle) == 0)
                        {
                            newTitle = options.PolicyPageTitle;
                        }
                    }
                }
                await Page.RestoreAsync(options, dataStore, article, editor, newTitle, newNamespace)
                    .ConfigureAwait(false);
            }
            else
            {
                await Page.RestoreAsync(options, dataStore, page, editor, null, null)
                    .ConfigureAwait(false);
            }

            var topic = await Topic.GetTopicAsync(dataStore, page.Title)
                .ConfigureAwait(false);
            if (topic is not null)
            {
                await topic.RestoreAsync(dataStore)
                    .ConfigureAwait(false);
            }
        }
    }
}
