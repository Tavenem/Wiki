using NeverFoundry.DataStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NeverFoundry.Wiki.Converters.NewtonsoftJson
{
    /// <summary>
    /// Converts an <see cref="Article"/> to and from JSON.
    /// </summary>
    public class ArticleConverter : JsonConverter<Article>
    {
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
        /// <param name="hasExistingValue">The existing value has a value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override Article ReadJson(JsonReader reader, Type objectType, [AllowNull] Article existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);

            if (!jObj.TryGetValue(nameof(IIdItem.IdItemTypeName), out var idItemTypeNameToken)
                || idItemTypeNameToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var idItemTypeName = idItemTypeNameToken.Value<string>();
            if (string.IsNullOrEmpty(idItemTypeName))
            {
                throw new JsonException();
            }

            if (string.Equals(idItemTypeName, Category.CategoryIdItemTypeName))
            {
                var category = serializer.Deserialize<Category>(jObj.CreateReader());
                if (category is null)
                {
                    throw new JsonException();
                }
                return category;
            }

            if (string.Equals(idItemTypeName, WikiFile.WikiFileIdItemTypeName))
            {
                var file = serializer.Deserialize<WikiFile>(jObj.CreateReader());
                if (file is null)
                {
                    throw new JsonException();
                }
                return file;
            }

            if (!jObj.TryGetValue(nameof(IIdItem.Id), out var idToken)
                || idToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var id = idToken.Value<string>();
            if (id is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(Article.Title), out var titleToken)
                || titleToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var title = titleToken.Value<string>();
            if (title is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(MarkdownItem.Html), out var htmlToken)
                || htmlToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var html = htmlToken.Value<string>();
            if (html is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(Article.MarkdownContent), out var markdownContentToken)
                || markdownContentToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var markdownContent = markdownContentToken.Value<string>();
            if (markdownContent is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(MarkdownItem.Preview), out var previewToken)
                || previewToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var preview = previewToken.Value<string>();
            if (preview is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(Article.WikiLinks), out var wikiLinksToken)
                || wikiLinksToken.Type != JTokenType.Array
                || !(wikiLinksToken is JArray wikiLinksArray))
            {
                throw new JsonException();
            }
            var wikiLinks = wikiLinksArray.ToObject<IReadOnlyCollection<WikiLink>>();
            if (wikiLinks is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(Article.TimestampTicks), out var timestampTicksToken)
                || timestampTicksToken.Type != JTokenType.Integer)
            {
                throw new JsonException();
            }
            var timestampTicks = timestampTicksToken.Value<long>();

            if (!jObj.TryGetValue(nameof(Article.WikiNamespace), out var wikiNamespaceToken)
                || wikiNamespaceToken.Type != JTokenType.String)
            {
                throw new JsonException();
            }
            var wikiNamespace = wikiNamespaceToken.Value<string>();
            if (wikiNamespace is null)
            {
                throw new JsonException();
            }

            if (!jObj.TryGetValue(nameof(Article.IsDeleted), out var isDeletedToken)
                || isDeletedToken.Type != JTokenType.Boolean)
            {
                throw new JsonException();
            }
            var isDeleted = isDeletedToken.Value<bool>();

            if (!jObj.TryGetValue(nameof(Article.Owner), out var ownerToken))
            {
                throw new JsonException();
            }
            var owner = ownerToken.Value<string>();

            IReadOnlyCollection<string>? allowedEditors;
            if (!jObj.TryGetValue(nameof(Article.AllowedEditors), out var allowedEditorsToken))
            {
                throw new JsonException();
            }
            if (allowedEditorsToken.Type == JTokenType.Null)
            {
                allowedEditors = null;
            }
            else if (allowedEditorsToken.Type != JTokenType.Array
                || !(allowedEditorsToken is JArray allowedEditorsArray))
            {
                throw new JsonException();
            }
            else
            {
                allowedEditors = allowedEditorsArray.ToObject<IReadOnlyCollection<string>>();
            }

            IReadOnlyCollection<string>? allowedViewers;
            if (!jObj.TryGetValue(nameof(Article.AllowedViewers), out var allowedViewersToken))
            {
                throw new JsonException();
            }
            if (allowedViewersToken.Type == JTokenType.Null)
            {
                allowedViewers = null;
            }
            else if (allowedViewersToken.Type != JTokenType.Array
                || !(allowedViewersToken is JArray allowedViewersArray))
            {
                throw new JsonException();
            }
            else
            {
                allowedViewers = allowedViewersArray.ToObject<IReadOnlyCollection<string>>();
            }

            if (!jObj.TryGetValue(nameof(Article.RedirectNamespace), out var redirectNamespaceToken))
            {
                throw new JsonException();
            }
            var redirectNamespace = redirectNamespaceToken.Value<string>();

            if (!jObj.TryGetValue(nameof(Article.RedirectTitle), out var redirectTitleToken))
            {
                throw new JsonException();
            }
            var redirectTitle = redirectTitleToken.Value<string>();

            if (!jObj.TryGetValue(nameof(Article.IsBrokenRedirect), out var isBrokenRedirectToken)
                || isBrokenRedirectToken.Type != JTokenType.Boolean)
            {
                throw new JsonException();
            }
            var isBrokenRedirect = isBrokenRedirectToken.Value<bool>();

            if (!jObj.TryGetValue(nameof(Article.IsDoubleRedirect), out var isDoubleRedirectToken)
                || isDoubleRedirectToken.Type != JTokenType.Boolean)
            {
                throw new JsonException();
            }
            var isDoubleRedirect = isDoubleRedirectToken.Value<bool>();

            if (!jObj.TryGetValue(nameof(Article.Categories), out var categoriesToken)
                || categoriesToken.Type != JTokenType.Array
                || !(categoriesToken is JArray categoriesArray))
            {
                throw new JsonException();
            }
            var categories = categoriesArray.ToObject<IReadOnlyCollection<string>>();
            if (categories is null)
            {
                throw new JsonException();
            }

            IReadOnlyList<Transclusion>? transclusions;
            if (!jObj.TryGetValue(nameof(Article.Transclusions), out var transclusionsToken))
            {
                throw new JsonException();
            }
            if (transclusionsToken.Type == JTokenType.Null)
            {
                transclusions = null;
            }
            else if (transclusionsToken.Type != JTokenType.Array
                || !(transclusionsToken is JArray transclusionsArray))
            {
                throw new JsonException();
            }
            else
            {
                transclusions = transclusionsArray.ToObject<IReadOnlyList<Transclusion>>();
            }

            return new Article(
                id,
                idItemTypeName,
                title,
                html,
                markdownContent,
                preview,
                wikiLinks,
                timestampTicks,
                wikiNamespace,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                redirectNamespace,
                redirectTitle,
                isBrokenRedirect,
                isDoubleRedirect,
                categories,
                transclusions);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, [AllowNull] Article value, JsonSerializer serializer)
        {
            if (value is Category category)
            {
                serializer.Serialize(writer, category, typeof(Category));
                return;
            }
            if (value is WikiFile file)
            {
                serializer.Serialize(writer, file, typeof(WikiFile));
                return;
            }

            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(IIdItem.Id));
            writer.WriteValue(value.Id);

            writer.WritePropertyName(nameof(IIdItem.IdItemTypeName));
            writer.WriteValue(value.IdItemTypeName);

            writer.WritePropertyName(nameof(Article.Title));
            writer.WriteValue(value.Title);

            writer.WritePropertyName(nameof(MarkdownItem.Html));
            writer.WriteValue(value.Html);

            writer.WritePropertyName(nameof(Article.MarkdownContent));
            writer.WriteValue(value.MarkdownContent);

            writer.WritePropertyName(nameof(MarkdownItem.Preview));
            writer.WriteValue(value.Preview);

            writer.WritePropertyName(nameof(Article.WikiLinks));
            serializer.Serialize(writer, value.WikiLinks);

            writer.WritePropertyName(nameof(Article.TimestampTicks));
            writer.WriteValue(value.TimestampTicks);

            writer.WritePropertyName(nameof(Article.WikiNamespace));
            writer.WriteValue(value.WikiNamespace);

            writer.WritePropertyName(nameof(Article.IsDeleted));
            writer.WriteValue(value.IsDeleted);

            writer.WritePropertyName(nameof(Article.Owner));
            if (string.IsNullOrEmpty(value.Owner))
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.Owner);
            }

            writer.WritePropertyName(nameof(Article.AllowedEditors));
            if (value.AllowedEditors is null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, value.AllowedEditors);
            }

            writer.WritePropertyName(nameof(Article.AllowedViewers));
            if (value.AllowedViewers is null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, value.AllowedViewers);
            }

            writer.WritePropertyName(nameof(Article.RedirectNamespace));
            if (string.IsNullOrEmpty(value.RedirectNamespace))
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.RedirectNamespace);
            }

            writer.WritePropertyName(nameof(Article.RedirectTitle));
            if (string.IsNullOrEmpty(value.RedirectTitle))
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.RedirectTitle);
            }

            writer.WritePropertyName(nameof(Article.IsBrokenRedirect));
            writer.WriteValue(value.IsBrokenRedirect);

            writer.WritePropertyName(nameof(Article.IsDoubleRedirect));
            writer.WriteValue(value.IsDoubleRedirect);

            writer.WritePropertyName(nameof(Article.Categories));
            serializer.Serialize(writer, value.Categories);

            writer.WritePropertyName(nameof(Article.Transclusions));
            if (value.Transclusions is null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, value.Transclusions);
            }

            writer.WriteEndObject();
        }
    }
}
