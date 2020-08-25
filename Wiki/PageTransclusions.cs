using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// Represents the collection of other wiki pages which transclude a given wiki page.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class PageTransclusions : IdItem, ISerializable
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string PageTransclusionsIdItemTypeName = ":PageTransclusions:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public string IdItemTypeName => PageTransclusionsIdItemTypeName;

        /// <summary>
        /// <para>
        /// The IDs of other wiki pages which transclude the primary wiki page.
        /// </para>
        /// <para>
        /// Does not include transclusions in discussion messages.
        /// </para>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Initializes a new instance of <see cref="PageTransclusions"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="references">
        /// The IDs of other wiki pages which transclude the primary wiki page.
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers. The static <see
        /// cref="NewAsync(string, string, string)"/> method is expected to be used otherwise, as it
        /// persists instances to the <see cref="WikiConfig.DataStore"/> and builds the reference
        /// list dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public PageTransclusions(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            IReadOnlyList<string> references) : base(id)
            => References = references;

        private PageTransclusions(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            PageTransclusionsIdItemTypeName,
            (IReadOnlyList<string>?)info.GetValue(nameof(References), typeof(IReadOnlyList<string>)) ?? new List<string>().AsReadOnly())
        { }

        /// <summary>
        /// Gets an ID for a <see cref="PageTransclusions"/> given the parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The ID which should be used for a <see cref="PageTransclusions"/> given the parameters.
        /// </returns>
        public static string GetId(string title, string? wikiNamespace = null)
            => $"{wikiNamespace ?? WikiConfig.DefaultNamespace}:{title}:transclusions";

        /// <summary>
        /// Gets the <see cref="PageTransclusions"/> that fits the given parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="PageTransclusions"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static PageTransclusions? GetPageTransclusions(string title, string? wikiNamespace = null)
            => WikiConfig.DataStore.GetItem<PageTransclusions>(GetId(title, wikiNamespace));

        /// <summary>
        /// Gets the <see cref="PageTransclusions"/> that fits the given parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="PageTransclusions"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static ValueTask<PageTransclusions?> GetPageTransclusionsAsync(string title, string? wikiNamespace = null)
            => WikiConfig.DataStore.GetItemAsync<PageTransclusions>(GetId(title, wikiNamespace));

        /// <summary>
        /// Get a new instance of <see cref="PageTransclusions"/>.
        /// </summary>
        /// <param name="title">
        /// The title of the transcluded wiki page.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace of the transcluded wiki page.
        /// </param>
        /// <param name="referenceId">
        /// The initial <see cref="IdItem.Id"/> of a wiki page which transcludes the primary page.
        /// </param>
        public static async Task<PageTransclusions> NewAsync(string title, string wikiNamespace, string referenceId)
        {
            var result = new PageTransclusions(
                GetId(title, wikiNamespace),
                PageTransclusionsIdItemTypeName,
                new List<string> { referenceId }.AsReadOnly());
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Adds a page to this collection.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which transcludes the referenced wiki page.
        /// </param>
        public async Task AddReferenceAsync(string id)
        {
            if (References.Contains(id))
            {
                return;
            }

            var result = new PageTransclusions(
                Id,
                PageTransclusionsIdItemTypeName,
                References.ToImmutableList().Add(id));
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(References), References);
        }

        /// <summary>
        /// Removes a page from this collection.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which no longer transcludes to the
        /// referenced wiki page.
        /// </param>
        public async Task RemoveReferenceAsync(string id)
        {
            if (!References.Contains(id))
            {
                return;
            }

            if (References.Count == 1)
            {
                await WikiConfig.DataStore.RemoveItemAsync<PageTransclusions>(Id).ConfigureAwait(false);
            }
            else
            {
                var result = new PageTransclusions(
                    Id,
                    PageTransclusionsIdItemTypeName,
                    References.ToImmutableList().Remove(id));
                await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            }
        }
    }
}
