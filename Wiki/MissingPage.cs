using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A persisted reference to a missing page on the wiki. Used for efficient enumeration of
    /// broken links.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class MissingPage : IdItem, ISerializable
    {
        /// <summary>
        /// Gets the full title of this missing page (including namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual string FullTitle => Article.GetFullTitle(Title, WikiNamespace);

        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string MissingPageIdItemTypeName = ":MissingPage:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static: property is serialized.
        public string IdItemTypeName => MissingPageIdItemTypeName;
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// The IDs of pages which reference this missing page.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The namespace to which this page should belong.
        /// </summary>
        public string WikiNamespace { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Revision"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="title">
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace to which this page should belong.
        /// </param>
        /// <param name="references">
        /// The IDs of pages which reference this missing page.
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers. The static <see
        /// cref="NewAsync(string, string?, string[])"/> method is expected to be used
        /// otherwise, as it persists instances to the <see cref="WikiConfig.DataStore"/> and builds
        /// the reference list dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public MissingPage(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
            string title,
            string wikiNamespace,
#pragma warning restore IDE0060 // Remove unused parameter
            IReadOnlyList<string> references) : base(id)
        {
            Title = title;
            WikiNamespace = wikiNamespace;
            References = references;
        }

        private MissingPage(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            MissingPageIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (IReadOnlyList<string>?)info.GetValue(nameof(References), typeof(IReadOnlyList<string>)) ?? new List<string>().AsReadOnly())
        { }

        /// <summary>
        /// Gets an ID for a <see cref="MissingPage"/> given the parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The ID which should be used for a <see cref="MissingPage"/> given the parameters.
        /// </returns>
        public static string GetId(string title, string? wikiNamespace = null)
            => $"{wikiNamespace ?? WikiConfig.DefaultNamespace}:{title}:missing";

        /// <summary>
        /// Gets the <see cref="MissingPage"/> that fits the given parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="MissingPage"/> that fits the given parameters; or <see langword="null"/>,
        /// if there is no such item.
        /// </returns>
        public static MissingPage? GetMissingPage(string title, string? wikiNamespace = null)
            => WikiConfig.DataStore.GetItem<MissingPage>(GetId(title, wikiNamespace));

        /// <summary>
        /// Gets the <see cref="MissingPage"/> that fits the given parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="MissingPage"/> that fits the given parameters; or <see langword="null"/>,
        /// if there is no such item.
        /// </returns>
        public static ValueTask<MissingPage?> GetMissingPageAsync(string title, string? wikiNamespace = null)
            => WikiConfig.DataStore.GetItemAsync<MissingPage>(GetId(title, wikiNamespace));

        /// <summary>
        /// Get a new instance of <see cref="MissingPage"/>.
        /// </summary>
        /// <param name="title">
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace to which this page should belong.
        /// </param>
        /// <param name="referenceIds">The IDs of the initial pages which references this missing page.</param>
        public static async Task<MissingPage> NewAsync(
            string title,
            string? wikiNamespace,
            params string[] referenceIds)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            var result = new MissingPage(
                GetId(title, wikiNamespace),
                MissingPageIdItemTypeName,
                title,
                wikiNamespace ?? WikiConfig.DefaultNamespace,
                referenceIds);
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Get a new instance of <see cref="MissingPage"/>.
        /// </summary>
        /// <param name="title">
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace to which this page should belong.
        /// </param>
        /// <param name="referenceIds">The IDs of the initial pages which references this missing page.</param>
        public static Task<MissingPage> NewAsync(
            string title,
            string? wikiNamespace,
            IEnumerable<string> referenceIds)
            => NewAsync(title, wikiNamespace, referenceIds.ToArray());

        /// <summary>
        /// Adds a page to this collection.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which links to the referenced wiki page.
        /// </param>
        public async Task AddReferenceAsync(string id)
        {
            if (References.Contains(id))
            {
                return;
            }

            var result = new MissingPage(
                Id,
                MissingPageIdItemTypeName,
                Title,
                WikiNamespace,
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
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
        }

        /// <summary>
        /// Removes a page from this collection.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which no longer links to the referenced
        /// wiki page.
        /// </param>
        public async Task RemoveReferenceAsync(string id)
        {
            if (!References.Contains(id))
            {
                return;
            }

            if (References.Count == 1)
            {
                await WikiConfig.DataStore.RemoveItemAsync<MissingPage>(Id).ConfigureAwait(false);
            }
            else
            {
                var result = new MissingPage(
                    Id,
                    MissingPageIdItemTypeName,
                    Title,
                    WikiNamespace,
                    References.ToImmutableList().Remove(id));
                await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            }
        }
    }
}
