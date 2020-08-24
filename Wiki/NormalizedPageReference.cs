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
        public string IdItemTypeName => NormalizedPageReferenceIdItemTypeName;

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
        /// cref="NewAsync(string, string, string)"/> method is expected to be used otherwise, as it
        /// persists instances to the <see cref="WikiConfig.DataStore"/> and assigns the ID
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
        /// Get a new instance of <see cref="NormalizedPageReference"/>.
        /// </summary>
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
        public static async Task<NormalizedPageReference> NewAsync(string id, string title, string wikiNamespace)
        {
            var result = new NormalizedPageReference(
                $"{wikiNamespace.ToLowerInvariant()}:{title.ToLowerInvariant()}:normalizedreference",
                NormalizedPageReferenceIdItemTypeName,
                new List<string> { id }.AsReadOnly());
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Adds a page to this reference.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which is to be assigned to the referenced
        /// full title.
        /// </param>
        public async Task AddReferenceAsync(string id)
        {
            if (References.Contains(id))
            {
                return;
            }

            var result = new NormalizedPageReference(
                Id,
                NormalizedPageReferenceIdItemTypeName,
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
        /// Removes a page from this reference.
        /// </summary>
        /// <param name="id">
        /// The <see cref="IdItem.Id"/> of the wiki page which is to be removed from the referenced
        /// full title.
        /// </param>
        public async Task RemoveReferenceAsync(string id)
        {
            if (!References.Contains(id))
            {
                return;
            }

            if (References.Count == 1)
            {
                await WikiConfig.DataStore.RemoveItemAsync<NormalizedPageReference>(Id).ConfigureAwait(false);
            }
            else
            {
                var result = new NormalizedPageReference(
                    Id,
                    NormalizedPageReferenceIdItemTypeName,
                    References.ToImmutableList().Remove(id));
                await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            }
        }
    }
}
