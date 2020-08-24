using NeverFoundry.DataStorage;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A reference from a full wiki page title to the current page ID assigned to that title.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public class PageReference : IdItem, ISerializable
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string PageReferenceIdItemTypeName = ":PageReference:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public string IdItemTypeName => PageReferenceIdItemTypeName;

        /// <summary>
        /// The ID of the wiki page which is currently assigned to the referenced full title.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="PageReference"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="reference">
        /// The ID of the wiki page which is currently assigned to the referenced full title.
        /// </param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers. The static <see
        /// cref="NewAsync(string, string, string)"/> method is expected to be used otherwise, as it
        /// persists instances to the <see cref="WikiConfig.DataStore"/> and assigns the ID
        /// dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public PageReference(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string reference) : base(id)
            => Reference = reference;

        private PageReference(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            PageReferenceIdItemTypeName,
            (string?)info.GetValue(nameof(Reference), typeof(string)) ?? string.Empty)
        { }

        /// <summary>
        /// Get a new instance of <see cref="PageReference"/>.
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
        public static async Task<PageReference> NewAsync(string id, string title, string wikiNamespace)
        {
            var result = new PageReference(
                $"{wikiNamespace}:{title}:reference",
                PageReferenceIdItemTypeName,
                id);
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
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
            info.AddValue(nameof(Reference), Reference);
        }
    }
}
