using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// Represents the collection of other wiki pages which redirect to a given wiki page.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class PageRedirects : IdItem, ISerializable
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string PageRedirectsIdItemTypeName = ":PageRedirects:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static: Serialized
        public string IdItemTypeName => PageRedirectsIdItemTypeName;
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// The IDs of other wiki pages which redirect to the primary wiki page.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Initializes a new instance of <see cref="PageRedirects"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="references">
        /// The IDs of other wiki pages which redirect to the primary wiki page.
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers. The static <see
        /// cref="NewAsync(IDataStore, string, string, string)"/> method is expected to be used
        /// otherwise, as it persists instances to the <see cref="IDataStore"/> and builds the
        /// reference list dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public PageRedirects(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            IReadOnlyList<string> references) : base(id)
            => References = references;

        private PageRedirects(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            PageRedirectsIdItemTypeName,
            (IReadOnlyList<string>?)info.GetValue(nameof(References), typeof(IReadOnlyList<string>)) ?? new List<string>().AsReadOnly())
        { }

        /// <summary>
        /// Gets an ID for a <see cref="PageRedirects"/> given the parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The ID which should be used for a <see cref="PageRedirects"/> given the parameters.
        /// </returns>
        public static string GetId(string title, string wikiNamespace)
            => $"{wikiNamespace}:{title}:redirects";

        /// <summary>
        /// Gets the <see cref="PageRedirects"/> that fits the given parameters.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="PageRedirects"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static PageRedirects? GetPageRedirects(IDataStore dataStore, string title, string wikiNamespace)
            => dataStore.GetItem<PageRedirects>(GetId(title, wikiNamespace));

        /// <summary>
        /// Gets the <see cref="PageRedirects"/> that fits the given parameters.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="PageRedirects"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static ValueTask<PageRedirects?> GetPageRedirectsAsync(IDataStore dataStore, string title, string wikiNamespace)
            => dataStore.GetItemAsync<PageRedirects>(GetId(title, wikiNamespace));

        /// <summary>
        /// Get a new instance of <see cref="PageRedirects"/>.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="title">
        /// The title of the linked wiki page.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace of the linked wiki page.
        /// </param>
        /// <param name="referenceId">
        /// The initial <see cref="IdItem.Id"/> of a wiki page which redirects to the primary page.
        /// </param>
        public static async Task<PageRedirects> NewAsync(IDataStore dataStore, string title, string wikiNamespace, string referenceId)
        {
            var result = new PageRedirects(
                GetId(title, wikiNamespace),
                PageRedirectsIdItemTypeName,
                new List<string> { referenceId }.AsReadOnly());
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Adds a page to this collection.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which redirects to the referenced wiki page.
        /// </param>
        public async Task AddReferenceAsync(IDataStore dataStore, string id)
        {
            if (References.Contains(id))
            {
                return;
            }

            var result = new PageLinks(
                Id,
                PageRedirectsIdItemTypeName,
                References.ToImmutableList().Add(id));
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(References), References);
        }

        /// <summary>
        /// Removes a page from this collection.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which no longer redirects to the referenced
        /// wiki page.
        /// </param>
        public async Task RemoveReferenceAsync(IDataStore dataStore, string id)
        {
            if (!References.Contains(id))
            {
                return;
            }

            if (References.Count == 1)
            {
                await dataStore.RemoveItemAsync<PageRedirects>(Id).ConfigureAwait(false);
            }
            else
            {
                var result = new PageRedirects(
                    Id,
                    PageRedirectsIdItemTypeName,
                    References.ToImmutableList().Remove(id));
                await dataStore.StoreItemAsync(result).ConfigureAwait(false);
            }
        }
    }
}
