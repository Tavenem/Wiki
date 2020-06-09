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
        /// The IDs of pages which reference this missing page.
        /// </summary>
        public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

        /// <summary>
        /// The title of this missing page. Must be unique within its namespace, and non-empty.
        /// </summary>
        public string Title { get; private protected set; }

        /// <summary>
        /// The namespace to which this page should belong.
        /// </summary>
        public string WikiNamespace { get; private protected set; }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        private MissingPage(
            string id,
            string title,
            string? wikiNamespace,
            List<string> references) : base(id)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            Title = title;
            WikiNamespace = wikiNamespace ?? WikiConfig.DefaultNamespace;
            References = references.AsReadOnly();
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
            var result = new MissingPage(id, title, wikiNamespace, new List<string> { referenceId });
            await result.SaveAsync().ConfigureAwait(false);
            return result;
        }

        private MissingPage(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (List<string>?)info.GetValue(nameof(References), typeof(List<string>)) ?? new List<string>())
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
