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
    /// A reference from a normalized (case-insensitive) full wiki page title to the current page
    /// IDs assigned to that title.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class NormalizedPageReference : IdItem, ISerializable
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string NormalizedPageReferenceIdItemTypeName = ":NormalizedPageReference:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static: Serialized
        public string IdItemTypeName => NormalizedPageReferenceIdItemTypeName;
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// The IDs of the wiki pages which are currently assigned to the referenced full title.
        /// </summary>
        public IReadOnlyList<string> References { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="NormalizedPageReference"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="references">
        /// The IDs of the wiki pages which are currently assigned to the referenced full title.
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers. The static <see
        /// cref="NewAsync(IDataStore, string, string, string)"/> method is expected to be used
        /// otherwise, as it persists instances to the <see cref="IDataStore"/> and assigns the ID
        /// dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public NormalizedPageReference(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            IReadOnlyList<string> references) : base(id)
            => References = references;

        private NormalizedPageReference(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            NormalizedPageReferenceIdItemTypeName,
            (IReadOnlyList<string>?)info.GetValue(nameof(References), typeof(IReadOnlyList<string>)) ?? new List<string>().AsReadOnly())
        { }

        /// <summary>
        /// Gets an ID for a <see cref="NormalizedPageReference"/> given the parameters.
        /// </summary>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The ID which should be used for a <see cref="NormalizedPageReference"/> given the
        /// parameters.
        /// </returns>
        public static string GetId(string title, string wikiNamespace)
            => $"{wikiNamespace.ToLowerInvariant()}:{title.ToLowerInvariant()}:normalizedreference";

        /// <summary>
        /// Gets the <see cref="NormalizedPageReference"/> that fits the given parameters.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="NormalizedPageReference"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static NormalizedPageReference? GetNormalizedPageReference(IDataStore dataStore, string title, string wikiNamespace)
            => dataStore.GetItem<NormalizedPageReference>(GetId(title, wikiNamespace));

        /// <summary>
        /// Gets the <see cref="NormalizedPageReference"/> that fits the given parameters.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="title">The title of the wiki page.</param>
        /// <param name="wikiNamespace">The namespace of the wiki page.</param>
        /// <returns>
        /// The <see cref="NormalizedPageReference"/> that fits the given parameters; or <see
        /// langword="null"/>, if there is no such item.
        /// </returns>
        public static ValueTask<NormalizedPageReference?> GetNormalizedPageReferenceAsync(IDataStore dataStore, string title, string wikiNamespace)
            => dataStore.GetItemAsync<NormalizedPageReference>(GetId(title, wikiNamespace));

        /// <summary>
        /// Get a new instance of <see cref="NormalizedPageReference"/>.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which is currently assigned to the
        /// referenced full title.
        /// </param>
        /// <param name="title">
        /// The title of the wiki page which is currently assigned to the referenced full title.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace of the wiki page which is currently assigned to the referenced full title.
        /// </param>
        public static async Task<NormalizedPageReference> NewAsync(IDataStore dataStore, string id, string title, string wikiNamespace)
        {
            var result = new NormalizedPageReference(
                GetId(title, wikiNamespace),
                NormalizedPageReferenceIdItemTypeName,
                new List<string> { id }.AsReadOnly());
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Adds a page to this reference.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which is to be assigned to the referenced
        /// full title.
        /// </param>
        public async Task AddReferenceAsync(IDataStore dataStore, string id)
        {
            if (References.Contains(id))
            {
                return;
            }

            var result = new NormalizedPageReference(
                Id,
                NormalizedPageReferenceIdItemTypeName,
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
        /// Removes a page from this reference.
        /// </summary>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which is to be removed from the referenced
        /// full title.
        /// </param>
        public async Task RemoveReferenceAsync(IDataStore dataStore, string id)
        {
            if (!References.Contains(id))
            {
                return;
            }

            if (References.Count == 1)
            {
                await dataStore.RemoveItemAsync<NormalizedPageReference>(Id).ConfigureAwait(false);
            }
            else
            {
                var result = new NormalizedPageReference(
                    Id,
                    NormalizedPageReferenceIdItemTypeName,
                    References.ToImmutableList().Remove(id));
                await dataStore.StoreItemAsync(result).ConfigureAwait(false);
            }
        }
    }
}
