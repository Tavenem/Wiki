using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
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
        public virtual string FullTitle => string.CompareOrdinal(WikiNamespace, WikiConfig.DefaultNamespace) == 0
            ? Title
            : $"{WikiNamespace}:{Title}";

        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string MissingPageIdItemTypeName = ":MissingPage:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public string IdItemTypeName => MissingPageIdItemTypeName;

        /// <summary>
        /// The IDs of pages which reference this missing page.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </summary>
        public string Title { get; private protected set; }

        /// <summary>
        /// The namespace to which this page should belong.
        /// </summary>
        public string WikiNamespace { get; private protected set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRevision"/>.
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
        /// cref="NewAsync(string, string, string?, string)"/> method is expected to be used
        /// otherwise, as it persists instances to the <see cref="WikiConfig.DataStore"/> and builds
        /// the reference list dynamically.
        /// </remarks>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public MissingPage(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: required for deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string title,
            string wikiNamespace,
            IReadOnlyList<string> references) : base(id)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            Title = title;
            WikiNamespace = wikiNamespace;
            References = references;
        }

        /// <summary>
        /// Get a new instance of <see cref="MissingPage"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="title">
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace to which this page should belong.
        /// </param>
        /// <param name="referenceId">The ID of the initial page which references this missing page.</param>
        public static async Task<MissingPage> NewAsync(
            string id,
            string title,
            string? wikiNamespace,
            string referenceId)
        {
            var result = new MissingPage(id, MissingPageIdItemTypeName, title, wikiNamespace ?? WikiConfig.DefaultNamespace, new List<string> { referenceId }.AsReadOnly());
            await WikiConfig.DataStore.StoreItemAsync(result).ConfigureAwait(false);
            return result;
        }

        private MissingPage(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            MissingPageIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (IReadOnlyList<string>?)info.GetValue(nameof(References), typeof(IReadOnlyList<string>)) ?? new List<string>().AsReadOnly())
        { }

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
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
            info.AddValue(nameof(References), References);
        }
    }
}
